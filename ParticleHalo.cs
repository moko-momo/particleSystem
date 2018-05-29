using UnityEngine;


public class ParticleHalo : MonoBehaviour
{

    private ParticleSystem particleSys; 
    private ParticleSystem.Particle[] particleArr; 
    private Status[] StatusArr; 
    public int particleNum = 10000; 
    public float minRadius = 8.0f;
    public float maxRadius = 12.0f; 
    public float maxRadiusChange = 0.02f;
    public bool clockwise = true;  
    public float rotateSpeed = 0.3f;  
    public int speedLevel = 5; 
    private NormalDistribution normalGenerator; 
    public Gradient colorGradient;  


    void Start()
    {
        particleSys = GetComponent<ParticleSystem>();
        particleArr = new ParticleSystem.Particle[particleNum];
        StatusArr = new Status[particleNum];

        var ma = particleSys.main; 
        ma.maxParticles = particleNum;

        particleSys.Emit(particleNum);  
        particleSys.GetParticles(particleArr);  
        normalGenerator = new NormalDistribution(); 

        
        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[5];
        alphaKeys[0].time = 0.0f; alphaKeys[0].alpha = 1.0f;
        alphaKeys[1].time = 0.4f; alphaKeys[1].alpha = 0.4f;
        alphaKeys[2].time = 0.6f; alphaKeys[2].alpha = 1.0f;
        alphaKeys[3].time = 0.9f; alphaKeys[3].alpha = 0.4f;
        alphaKeys[4].time = 1.0f; alphaKeys[4].alpha = 0.9f;
        GradientColorKey[] colorKeys = new GradientColorKey[2];
        colorKeys[0].time = 0.0f; colorKeys[0].color = Color.white;
        colorKeys[1].time = 1.0f; colorKeys[1].color = Color.white;
        colorGradient.SetKeys(colorKeys, alphaKeys);
		initParticle();
    }

    
	void initParticle()
    {
        for (int i = 0; i < particleNum; i++)
        {
            float midRadius = (maxRadius + minRadius) / 2;
            float radius = (float)normalGenerator.NextGaussian(midRadius, 0.7);

            float angle = Random.Range(0.0f, 360.0f);
            float theta = angle / 180 * Mathf.PI;
            float time = Random.Range(0.0f, 360.0f);   
            float radiusChange = Random.Range(0.0f, maxRadiusChange); 
            StatusArr[i] = new Status(radius, angle, time, radiusChange);
            particleArr[i].position = computePos(radius, theta);
        }
        particleSys.SetParticles(particleArr, particleArr.Length);
    }

    Vector3 computePos(float radius, float theta)
    {
        return new Vector3(radius * Mathf.Cos(theta), 0f, radius * Mathf.Sin(theta));
    }

    void Update()
    {
        for (int i = 0; i < particleNum; i++)
        {
          
            if (!clockwise)
            {
                StatusArr[i].angle += (i % speedLevel + 1) * (rotateSpeed / speedLevel);
            }
            else
            {
                StatusArr[i].angle -= (i % speedLevel + 1) * (rotateSpeed / speedLevel);
            }

            // angle range guarantee
            StatusArr[i].angle = (360.0f + StatusArr[i].angle) % 360.0f;
            float theta = StatusArr[i].angle / 180 * Mathf.PI;

            StatusArr[i].time += Time.deltaTime; 
            StatusArr[i].radius += Mathf.PingPong(StatusArr[i].time / maxRadius / maxRadius, StatusArr[i].radiusChange) - StatusArr[i].radiusChange / 2.0f; // 根据粒子的进度，给粒子的半径赋予不同的值，这个值在0与StatusArr[i].radiusChange之间来回摆动

            particleArr[i].position = computePos(StatusArr[i].radius, theta);
            particleArr[i].color = colorGradient.Evaluate(StatusArr[i].angle / 360.0f);
        }

        particleSys.SetParticles(particleArr, particleArr.Length);
    }
}

using UnityEngine;

[System.Serializable]
public class Attributes
{
    public float hydration = 0.0f;
    public float fun = 0.0f;
    public float energy = 0.0f;
    public float confidence = 0.0f;

    public void Randomise()
    {
        hydration = Random.Range(0.0f,1.0f);
        fun = Random.Range(0.0f,1.0f);
        energy = Random.Range(0.0f,1.0f);
        confidence = Random.Range(0.0f,1.0f);
    }
}

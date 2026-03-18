using UnityEngine;

public class FirstUI : MonoBehaviour
{
    // Tid i sekunder innan UI:t förstörs
    public float lifetime = 20f;

    void Start()
    {
        // Förstör detta GameObject efter 'lifetime' sekunder
        Destroy(gameObject, lifetime);
    }
}

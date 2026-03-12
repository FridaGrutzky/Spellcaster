using UnityEngine;

public class FlyingCreature : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    private void OnParticleCollision(GameObject other)
    {
        Debug.Log("a particle touched me");
        Die();
    }
}

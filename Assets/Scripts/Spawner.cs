using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject prefab;          // Det som ska spawna
    public Vector3 boxSize = new Vector3(5, 3, 5);
    public float interval = 10f;       // Tid mellan spawns
    public Transform center;           // Mittpunkten för området

    Transform player = Camera.main.transform;
    float timer;

    /*
    private void Awake()
    {
        center = transform;
    }
    */

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= interval)
        {
            SpawnInBox();
            timer = 0f;
        }
    }

    void SpawnInBox()
    {
        float x = Random.Range(-boxSize.x / 2, boxSize.x / 2);
        float y = Random.Range(-boxSize.y / 2, boxSize.y / 2);
        float z = Random.Range(-boxSize.z / 2, boxSize.z / 2);

        Vector3 pos = center.position + new Vector3(x, y, z);

        GameObject obj = Instantiate(prefab, pos, Quaternion.identity);

        // Vänd mot kameran (spelaren)
        obj.transform.LookAt(Camera.main.transform);
    }

    // Bara för att se området i editorn
    void OnDrawGizmos()
    {
        if (center == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(center.position, new Vector3(boxSize.x, boxSize.y, boxSize.z));
    }
}

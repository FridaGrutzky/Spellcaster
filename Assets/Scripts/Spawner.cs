using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject prefab;
    public float interval = 10f;

    public Transform center;

    public float innerRadius = 1f;   // Ingen spawn här
    public float outerRadius = 5f;   // Spawn sker här
    public float height = 2f;        // Höjdvariation

    bool activated = false;
    Transform player;
    float timer;

    private void Awake()
    {
        player = Camera.main.transform;
    }

    public void ActivateSpawner()
    {
        activated = true;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= interval && activated)
        {
            SpawnInDonut();
            timer = 0f;
        }
    }

    void SpawnInDonut()
    {
        // Slumpa riktning i horisontalplanet
        Vector2 dir = Random.insideUnitCircle.normalized;

        // Slumpa avstånd mellan inner och outer radius
        float dist = Random.Range(innerRadius, outerRadius);

        // Gör om till 3D?offset
        Vector3 offset = new Vector3(dir.x, 0, dir.y) * dist;

        // Slumpa höjd
        float y = Random.Range(-height / 2, height / 2);

        Vector3 pos = center.position + offset + new Vector3(0, y, 0);

        // Skapa objektet
        GameObject obj = Instantiate(prefab, pos, Quaternion.identity);

        // Rotera mot spelaren
        Vector3 lookPos = player.position;
        lookPos.y = obj.transform.position.y;
        obj.transform.LookAt(lookPos);
    }

    // Gizmos för att rita donut?området
    void OnDrawGizmos()
    {
        if (center == null) return;

        Gizmos.color = Color.yellow;
        DrawCircle(center.position, outerRadius);

        Gizmos.color = Color.red;
        DrawCircle(center.position, innerRadius);
    }

    // Hjälpfunktion för att rita cirklar
    void DrawCircle(Vector3 centerPos, float radius)
    {
        int segments = 64;
        float angle = 0f;

        Vector3 prevPoint = centerPos + new Vector3(Mathf.Cos(0), 0, Mathf.Sin(0)) * radius;

        for (int i = 1; i <= segments; i++)
        {
            angle = i * Mathf.PI * 2f / segments;
            Vector3 newPoint = centerPos + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
            Gizmos.DrawLine(prevPoint, newPoint);
            prevPoint = newPoint;
        }
    }
}

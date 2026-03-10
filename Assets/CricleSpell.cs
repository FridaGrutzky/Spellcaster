using UnityEngine;

public class CircleSpell : MonoBehaviour
{
    [Header("Refs")]
    public TipToPlane2D tracker;
    public Transform head;                 // CenterEyeAnchor
    public GameObject successSpherePrefab; // prefab att spawna vid success

    [Header("Circle template (2D on plane)")]
    public float radius = 0.25f;

    // MINDRE KÄNSLIG (testvärden)
    public float radialTolerance = 0.35f;  // 0.10 -> 0.35 (mycket enklare)
    public float maxPlaneDepth = 0.75f;    // 0.08 -> 0.75 (behöver inte vara nära planet)

    [Header("Completion")]
    public int sectors = 16;               // 32 -> 16 (färre "bitar" runt cirkeln)
    public int requiredSectors = 5;        // 26 -> 5 (mycket enklare)
    public float maxTime = 10.0f;          // 4  -> 10 (mer tid)
    public float cooldown = 0.0f;          // 1  -> 0 (så du kan testa snabbt)

    bool[] _visited;
    int _visitedCount;
    float _t0;
    float _cooldownUntil;

    void Awake()
    {
        EnsureVisitedArray();
        ResetProgress();
    }

    void OnValidate()
    {
        // Om du ändrar sectors i Inspector, bygg om arrayen
        EnsureVisitedArray();
        // (valfritt) resetta när man ändrar värden
        if (!Application.isPlaying)
            ResetProgress();
    }

    void EnsureVisitedArray()
    {
        int n = Mathf.Max(8, sectors);
        if (_visited == null || _visited.Length != n)
            _visited = new bool[n];
    }

    void Update()
    {
        if (tracker == null) return;
        if (Time.time < _cooldownUntil) return;

        // timeout -> reset
        if (Time.time - _t0 > maxTime)
            ResetProgress();

        // måste vara "hyfsat" nära planet (men nu väldigt generöst)
        //if (Mathf.Abs(tracker.depth) > maxPlaneDepth)
          //  return;

        Vector2 p = tracker.P;

        // radie-check (nu mycket generös)
        float r = p.magnitude;
        if (Mathf.Abs(r - radius) > radialTolerance)
            return;

        // vinkel 0..360
        float ang = Mathf.Atan2(p.y, p.x) * Mathf.Rad2Deg;
        if (ang < 0f) ang += 360f;

        int s = Mathf.FloorToInt(ang / 360f * _visited.Length);
        s = Mathf.Clamp(s, 0, _visited.Length - 1);

        if (!_visited[s])
        {
            _visited[s] = true;
            _visitedCount++;

            if (_visitedCount >= requiredSectors)
                OnCircleCompleted();
        }
    }

    void OnCircleCompleted()
    {
        if (!head)
        {
            Debug.Log("No head assigned");
            return;
        }

        Vector3 pos = head.position + head.forward * 1.0f;

        GameObject g;
        if (successSpherePrefab)
        {
            g = Instantiate(successSpherePrefab, pos, Quaternion.identity);
        }
        else
        {
            g = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Destroy(g.GetComponent<Collider>());
            g.transform.position = pos;
            g.transform.localScale = Vector3.one * 0.25f;
        }

        _cooldownUntil = Time.time + cooldown;
        ResetProgress();
    }


    void ResetProgress()
    {
        EnsureVisitedArray();

        for (int i = 0; i < _visited.Length; i++)
            _visited[i] = false;

        _visitedCount = 0;
        _t0 = Time.time;
    }
}
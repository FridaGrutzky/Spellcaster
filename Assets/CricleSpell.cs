using UnityEngine;

public class CircleSpell : MonoBehaviour
{
    [Header("Refs")]
    public TipToPlane2D tracker;
    public Transform head;                 // CenterEyeAnchor
    public GameObject successSpherePrefab; // prefab att spawna vid success
    public Transform tipSphere; // Dra in din TipSphere här i Inspectorn!

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

    /*  void OnCircleCompleted()
      {
          // Vi kollar så att tipSphere finns
          if (!tipSphere)
          {
              Debug.Log("No TipSphere assigned!");
              return;
          }

          // Nu sätter vi positionen exakt där din TipSphere är just nu
          Vector3 pos = tipSphere.position + tipSphere.forward * 0.1f;

          GameObject g;
          if (successSpherePrefab)
          {
              // Vi skapar effekten vid spetsen och låter den ha samma rotation som spetsen
              g = Instantiate(successSpherePrefab, tipSphere.position, tipSphere.rotation);
              Destroy(g, 3f); // Kortare tid om det är små gnistor
          }
          else
          {
              g = GameObject.CreatePrimitive(PrimitiveType.Sphere);
              Destroy(g.GetComponent<Collider>());
              g.transform.position = pos;
              g.transform.localScale = Vector3.one * 0.05f; // Gör även reserv-bollen liten
              Destroy(g, 1f);
          }

          _cooldownUntil = Time.time + cooldown;
          ResetProgress();
      }
    */



    void OnCircleCompleted()
    {
        if (!tipSphere || !head) // Vi behöver 'head' (kameran) för att veta var DU är
        {
            Debug.Log("Missing TipSphere or Head!");
            return;
        }

        Vector3 pos = tipSphere.position + tipSphere.forward * 0.1f;

        GameObject g;
        if (successSpherePrefab)
        {
            // 1. Skapa den vid spetsen
            g = Instantiate(successSpherePrefab, tipSphere.position, tipSphere.rotation);

            // 2. FIXEN: Räkna ut riktningen från ditt huvud till spetsen
            // Det här gör att den ALLTID flyger bort från dig, oavsett rotation
            Vector3 directionAwayFromYou = (tipSphere.position - head.position).normalized;

            // 3. Tvinga partiklarna att peka i den riktningen
            g.transform.forward = directionAwayFromYou;

            // 4. Om du har en Rigidbody, skjut iväg den i samma riktning
            Rigidbody rb = g.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = directionAwayFromYou * 15f;
            }

            Destroy(g, 3f);
        }
        else
        {
            g = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Destroy(g.GetComponent<Collider>());
            g.transform.position = pos;
            g.transform.localScale = Vector3.one * 0.05f;
            Destroy(g, 1f);
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
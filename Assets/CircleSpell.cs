using UnityEngine;

public class CircleSpell : MonoBehaviour
{
    [Header("Refs")]
    public TipToPlane2D tracker;
    public Transform head;
    public GameObject successSpherePrefab;
    public Transform tipSphere;
    public TutorialChecklist checklist; // Link this in the Inspector

    [Header("Checklist Settings")]
    public bool isFireSpell; // Toggle this in the Inspector for your Fire version

    [Header("Circle template")]
    public float radius = 0.25f;
    public float radialTolerance = 0.35f;
    public float maxPlaneDepth = 0.75f;

    [Header("Completion")]
    public int sectors = 16;
    public int requiredSectors = 5;
    public float maxTime = 10.0f;
    public float cooldown = 0.0f;

    bool[] _visited;
    int _visitedCount;
    float _t0;
    float _cooldownUntil;

    void Awake() { EnsureVisitedArray(); ResetProgress(); }

    void EnsureVisitedArray()
    {
        int n = Mathf.Max(8, sectors);
        if (_visited == null || _visited.Length != n) _visited = new bool[n];
    }

    void Update()
    {
        if (tracker == null) return;
        if (Time.time < _cooldownUntil) return;
        if (Time.time - _t0 > maxTime) ResetProgress();

        Vector2 p = tracker.P;
        float r = p.magnitude;
        if (Mathf.Abs(r - radius) > radialTolerance) return;

        float ang = Mathf.Atan2(p.y, p.x) * Mathf.Rad2Deg;
        if (ang < 0f) ang += 360f;

        int s = Mathf.FloorToInt(ang / 360f * _visited.Length);
        s = Mathf.Clamp(s, 0, _visited.Length - 1);

        if (!_visited[s])
        {
            _visited[s] = true;
            _visitedCount++;
            if (_visitedCount >= requiredSectors) OnCircleCompleted();
        }
    }

    void OnCircleCompleted()
    {
        if (!tipSphere || !head) return;

        if (successSpherePrefab)
        {
            GameObject g = Instantiate(successSpherePrefab, tipSphere.position, tipSphere.rotation);
            Vector3 directionAwayFromYou = (tipSphere.position - head.position).normalized;
            g.transform.forward = directionAwayFromYou;

            Rigidbody rb = g.GetComponent<Rigidbody>();
            if (rb != null) rb.linearVelocity = directionAwayFromYou * 15f;
            Destroy(g, 3f);
        }

        // TRIGGER CHECKLIST
        if (checklist != null)
        {
            if (isFireSpell) checklist.CompleteStep4();
            else checklist.CompleteStep3();
        }

        _cooldownUntil = Time.time + cooldown;
        ResetProgress();
    }

    void ResetProgress()
    {
        EnsureVisitedArray();
        for (int i = 0; i < _visited.Length; i++) _visited[i] = false;
        _visitedCount = 0;
        _t0 = Time.time;
    }
}
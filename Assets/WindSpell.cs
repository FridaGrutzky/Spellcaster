using UnityEngine;

public class WindSpell : MonoBehaviour
{
    [Header("Refs")]
    public TipToPlane2D tracker;
    public Transform head;
    public Transform tipSphere;
    public TutorialChecklist checklist; // Link this in the Inspector

    [Header("Triangle shape")]
    public float width = 0.28f;
    public float height = 0.28f;
    public float tolerance = 0.22f;
    public float maxTime = 8f;
    public float cooldown = 0.25f;

    [Header("Spawn")]
    public GameObject windPrefab;

    int _currentPoint = 0;
    float _t0;
    float _cooldownUntil;

    Vector2[] _points = { new Vector2(0.00f, 0.50f), new Vector2(-0.45f, -0.35f), new Vector2(0.45f, -0.35f) };

    void Start() { ResetProgress(); }

    void Update()
    {
        if (tracker == null || head == null) return;
        if (Time.time < _cooldownUntil) return;
        if (Time.time - _t0 > maxTime) ResetProgress();

        Vector2 p = tracker.P;
        Vector2 target = new Vector2(_points[_currentPoint].x * width, _points[_currentPoint].y * height);

        if (Vector2.Distance(p, target) <= tolerance)
        {
            _currentPoint++;
            if (_currentPoint >= _points.Length) OnWindCompleted();
        }
    }

    void OnWindCompleted()
    {
        if (!tipSphere || !head) return;

        if (windPrefab)
        {
            GameObject g = Instantiate(windPrefab, tipSphere.position, tipSphere.rotation);
            Vector3 directionAwayFromYou = (tipSphere.position - head.position).normalized;
            g.transform.forward = directionAwayFromYou;

            Rigidbody rb = g.GetComponent<Rigidbody>();
            if (rb != null) rb.linearVelocity = directionAwayFromYou * 15f;
            Destroy(g, 3f);
        }

        // TRIGGER CHECKLIST (Using Step 3 as an example)
        if (checklist != null) checklist.CompleteStep3();

        _cooldownUntil = Time.time + cooldown;
        ResetProgress();
    }

    void ResetProgress() { _currentPoint = 0; _t0 = Time.time; }
}
using UnityEngine;

public class LightningSpell : MonoBehaviour
{
    [Header("Refs")]
    public TipToPlane2D tracker;
    public Transform head;
    public Transform tipSphere;
    public TutorialChecklist checklist; // Link this in the Inspector

    [Header("Lightning shape")]
    public float width = 0.18f;
    public float height = 0.28f;
    public float tolerance = 0.18f;
    public float maxTime = 10f;
    public float cooldown = 0f;

    [Header("Spawn")]
    public GameObject lightningPrefab;

    int _currentPoint = 0;
    float _t0;
    float _cooldownUntil;

    Vector2[] _points =
    {
        new Vector2(-0.10f,  0.50f),
        new Vector2( 0.12f,  0.12f),
        new Vector2(-0.25f,  0.12f),
        new Vector2( 0.10f, -0.50f),
        new Vector2( 0.00f, -0.05f),
    };

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
            if (_currentPoint >= _points.Length) OnLightningCompleted();
        }
    }

    void OnLightningCompleted()
    {
        if (!tipSphere || !head) return;

        if (lightningPrefab)
        {
            GameObject g = Instantiate(lightningPrefab, tipSphere.position, tipSphere.rotation);
            Vector3 directionAwayFromYou = (tipSphere.position - head.position).normalized;
            g.transform.forward = directionAwayFromYou;

            Rigidbody rb = g.GetComponent<Rigidbody>();
            if (rb != null) rb.linearVelocity = directionAwayFromYou * 15f;
            Destroy(g, 3f);
        }

        // TRIGGER CHECKLIST
        if (checklist != null) checklist.CompleteStep2();

        _cooldownUntil = Time.time + cooldown;
        ResetProgress();
    }

    void ResetProgress() { _currentPoint = 0; _t0 = Time.time; }
}
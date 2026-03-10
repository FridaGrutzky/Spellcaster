using UnityEngine;

public class WindSpell : MonoBehaviour
{
    [Header("Refs")]
    public TipToPlane2D tracker;
    public Transform head;

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

    Vector2[] _points =
    {
        new Vector2( 0.00f,  0.50f),   // topp
        new Vector2(-0.45f, -0.35f),   // vänster
        new Vector2( 0.45f, -0.35f),   // höger
    };

    void Start()
    {
        ResetProgress();
    }

    void Update()
    {
        if (tracker == null || head == null) return;
        if (Time.time < _cooldownUntil) return;

        if (Time.time - _t0 > maxTime)
            ResetProgress();

        Vector2 p = tracker.P;

        Vector2 target = new Vector2(
            _points[_currentPoint].x * width,
            _points[_currentPoint].y * height
        );

        if (Vector2.Distance(p, target) <= tolerance)
        {
            _currentPoint++;

            if (_currentPoint >= _points.Length)
            {
                OnWindCompleted();
            }
        }
    }

    void OnWindCompleted()
    {
        if (!head)
        {
            Debug.Log("No head assigned");
            return;
        }

        Vector3 pos = head.position + head.forward * 1.0f;

        if (windPrefab)
        {
            Instantiate(windPrefab, pos, Quaternion.identity);
        }

        _cooldownUntil = Time.time + cooldown;
        ResetProgress();
    }

    void ResetProgress()
    {
        _currentPoint = 0;
        _t0 = Time.time;
    }
}
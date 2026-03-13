using UnityEngine;

public class LightningSpell : BaseSpellGesture
{
    [Header("Lightning shape")]
    public float width = 0.18f;
    public float height = 0.28f;
    public float tolerance = 0.18f;
    public float maxTime = 10f;

    private int currentPoint = 0;
    private float t0;
    private bool complete;
    private float score;

    private Vector2[] points =
{
    new Vector2(-0.10f,  0.50f),
    new Vector2( 0.05f,  0.25f),
    new Vector2( 0.12f,  0.12f),
    new Vector2(-0.10f,  0.12f),
    new Vector2(-0.25f,  0.12f),
    new Vector2(-0.05f, -0.15f),
    new Vector2( 0.10f, -0.50f),
    new Vector2( 0.00f, -0.05f),
};

    public override float Score => score;
    public override bool IsComplete => complete;

    void Awake()
    {
        ResetGesture();
    }

    public override void Process()
    {
        if (tracker == null || complete)
            return;

        if (Time.time - t0 > maxTime)
        {
            ResetGesture();
            return;
        }

        Vector2 p = tracker.P;

        Vector2 target = new Vector2(
            points[currentPoint].x * width,
            points[currentPoint].y * height
        );

        float dist = Vector2.Distance(p, target);
        float pointQuality = 1f - Mathf.Clamp01(dist / tolerance);

        score = currentPoint + pointQuality;

        if (dist <= tolerance)
        {
            currentPoint++;

            if (currentPoint >= points.Length)
            {
                complete = true;
                score = points.Length + 1f;
            }
        }
    }

    public override void ResetGesture()
    {
        currentPoint = 0;
        t0 = Time.time;
        complete = false;
        score = 0f;
    }
}
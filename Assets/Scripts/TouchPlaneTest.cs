using UnityEngine;

public class TouchPlaneTest : MonoBehaviour
{
    public TipToPlane2D tracker;
    public Transform head;

    float accumulatedAngle = 0f;
    float lastAngle;
    bool hasLast = false;

    void Update()
    {
        if (!tracker || !head) return;

        Vector2 p = tracker.P;

        float angle = Mathf.Atan2(p.y, p.x) * Mathf.Rad2Deg;

        if (!hasLast)
        {
            lastAngle = angle;
            hasLast = true;
            return;
        }

        float delta = Mathf.DeltaAngle(lastAngle, angle);
        accumulatedAngle += Mathf.Abs(delta);

        lastAngle = angle;

        // om vi snurrat ungefär ett varv
        if (accumulatedAngle > 300f)
        {
            Spawn();
            accumulatedAngle = 0f;
        }
    }

    void Spawn()
    {
        Vector3 pos = head.position + head.forward * 1.0f;

        GameObject g = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Destroy(g.GetComponent<Collider>());

        g.transform.position = pos;
        g.transform.localScale = Vector3.one * 0.25f;
    }
}
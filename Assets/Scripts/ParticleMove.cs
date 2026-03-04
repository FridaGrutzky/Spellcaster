using UnityEngine;

public class ThreeClickMoveToTarget : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Click Settings")]
    public int clicksNeeded = 3;

    [Header("Movement")]
    public float speed = 2.5f;
    public float stopDistance = 0.05f;

    private int clickCount = 0;
    private bool moving = false;

    void Update()
    {
        // Count right mouse button presses
        if (Input.GetMouseButtonDown(1))
        {
            clickCount++;

            if (clickCount >= clicksNeeded)
            {
                clickCount = 0;     // reset so you can do it again
                moving = true;
            }
        }

        if (!moving || target == null) return;

        Vector3 toTarget = target.position - transform.position;
        float dist = toTarget.magnitude;

        if (dist <= stopDistance)
        {
            moving = false;
            return;
        }

        Vector3 dir = toTarget / dist; // normalize
        transform.position += dir * speed * Time.deltaTime;

        // Optional: face the direction of travel
        transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
    }
}

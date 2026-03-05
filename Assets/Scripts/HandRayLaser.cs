using UnityEngine;

public class HandRayLaser : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public float maxDistance = 10f;

    void Update()
    {
        RaycastHit hit;
        Vector3 endPoint;

        if (Physics.Raycast(transform.position, transform.forward, out hit, maxDistance))
        {
            endPoint = hit.point;
        }
        else
        {
            endPoint = transform.position + transform.forward * maxDistance;
        }

        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, endPoint);
    }
}
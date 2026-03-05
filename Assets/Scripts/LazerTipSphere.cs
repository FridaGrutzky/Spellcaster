using UnityEngine;

public class LaserTipSphere : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Transform tipSphere;

    // Om du vill styra längden här (utan raycast)
    public Transform rayStart;
    public float length = 2f;

    void Reset()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        if (!lineRenderer || !tipSphere) return;

        // (Valfritt) Om du vill att lasern alltid ska vara "length" lång:
        if (rayStart)
        {
            if (lineRenderer.positionCount < 2) lineRenderer.positionCount = 2;

            Vector3 start = rayStart.position;
            Vector3 end = start + rayStart.forward * length;

            if (lineRenderer.useWorldSpace)
            {
                lineRenderer.SetPosition(0, start);
                lineRenderer.SetPosition(1, end);
            }
            else
            {
                lineRenderer.SetPosition(0, lineRenderer.transform.InverseTransformPoint(start));
                lineRenderer.SetPosition(1, lineRenderer.transform.InverseTransformPoint(end));
            }
        }

        // Hämta laser-toppen (sista punkten) och flytta klotet dit
        int last = lineRenderer.positionCount - 1;
        Vector3 tipPos = lineRenderer.GetPosition(last);

        if (!lineRenderer.useWorldSpace)
            tipPos = lineRenderer.transform.TransformPoint(tipPos);

        tipSphere.position = tipPos;

        // (valfritt) rotera klotet mot laser-riktningen
        // tipSphere.rotation = Quaternion.LookRotation(tipSphere.position - rayStart.position);
    }
}
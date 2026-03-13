using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(LineRenderer))]
public class DrawCircleOnPlaneWorld : MonoBehaviour
{
    public Transform drawingPlane;
    public Transform viewer;         // dra in CenterEyeAnchor här (kamera)
    public float radius = 0.25f;
    public int segments = 96;
    public float offset = 0.05f;

    void OnEnable() => Draw();
    void OnValidate() => Draw();
    void Start() => Draw();

    private void Update()
    {
        Draw();
    }

    void Draw()
    {
        if (!drawingPlane) return;

        var lr = GetComponent<LineRenderer>();
        lr.useWorldSpace = true;
        lr.loop = true;
        lr.alignment = LineAlignment.View;
        lr.positionCount = Mathf.Max(16, segments);

        // Unity Plane: normal = up, yta = right + forward
        Vector3 normal = drawingPlane.up;
        Vector3 axisA = drawingPlane.right;
        Vector3 axisB = drawingPlane.forward;

        // Se till att normal pekar MOT kameran
        if (viewer)
        {
            Vector3 toViewer = (viewer.position - drawingPlane.position).normalized;
            if (Vector3.Dot(normal, toViewer) < 0f)
                normal = -normal;
        }

        Vector3 center = drawingPlane.position + normal * offset;

        for (int i = 0; i < lr.positionCount; i++)
        {
            float a = (float)i / lr.positionCount * Mathf.PI * 2f;
            lr.SetPosition(i, center + (Mathf.Cos(a) * axisA + Mathf.Sin(a) * axisB) * radius);
        }
    }
}
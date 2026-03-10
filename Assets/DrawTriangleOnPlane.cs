using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(LineRenderer))]
public class DrawTriangleOnPlane : MonoBehaviour
{
    public Transform drawingPlane;
    public Transform viewer;
    public float offset = 0.05f;

    [Header("Triangle size")]
    public float width = 0.28f;
    public float height = 0.28f;

    void OnEnable() => Draw();
    void OnValidate() => Draw();
    void Start() => Draw();

    void Update()
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
        lr.positionCount = 3;

        Vector3 normal = drawingPlane.up;
        Vector3 axisA = drawingPlane.right;
        Vector3 axisB = drawingPlane.forward;

        if (viewer)
        {
            Vector3 toViewer = (viewer.position - drawingPlane.position).normalized;
            if (Vector3.Dot(normal, toViewer) < 0f)
                normal = -normal;
        }

        Vector3 center = drawingPlane.position + normal * offset;

        Vector2[] shape = new Vector2[]
        {
            new Vector2( 0.00f,  0.50f),   // topp
            new Vector2(-0.45f, -0.35f),   // vänster hörn
            new Vector2( 0.45f, -0.35f),   // höger hörn
        };

        for (int i = 0; i < shape.Length; i++)
        {
            Vector3 pos =
                center +
                axisA * (shape[i].x * width) +
                axisB * (shape[i].y * height);

            lr.SetPosition(i, pos);
        }
    }
}
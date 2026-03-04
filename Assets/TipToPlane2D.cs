using UnityEngine;

public class TipToPlane2D : MonoBehaviour
{
    public Transform tipSphere;
    public Transform drawingPlane; // din Unity Plane (DrawingPlane)

    public Vector2 P { get; private set; }
    public float depth { get; private set; }

    void Update()
    {
        if (!tipSphere || !drawingPlane) return;

        Vector3 local = drawingPlane.InverseTransformPoint(tipSphere.position);

        // Unity Plane: y = normal (framför/bakom), x/z = på planet
        P = new Vector2(local.x, local.z);
        depth = local.y;
    }
}
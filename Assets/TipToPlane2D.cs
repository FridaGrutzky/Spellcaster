using UnityEngine;

public class TipToPlane2D : MonoBehaviour
{
    public Transform tip;           // din boll
    public Transform drawingPlane;  // objektet framför kameran

    public Vector2 PlanePoint { get; private set; }
    public float depth { get; private set; } // hur långt framför/bakom planet tippen är (valfritt)

    void Update()
    {
        if (!tip || !drawingPlane) return;

        Vector3 local = drawingPlane.InverseTransformPoint(tip.position);
        PlanePoint = new Vector2(local.x, local.y);
        depth = local.z; // 0 = exakt på planet
    }
}
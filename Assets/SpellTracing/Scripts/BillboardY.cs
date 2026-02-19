using UnityEngine;

/// <summary>
/// Rotates the attached GameObject on the Y-axis only to always face
/// the active camera / XR eye. Uses SpellInputProvider so it works
/// identically in mouse/editor mode and on Meta Quest.
///
/// Attach to: SpellTablet_[Name] root GameObject.
/// </summary>
public class BillboardY : MonoBehaviour
{
    void LateUpdate()
    {
        // Ask SpellInputProvider where the camera/eye is.
        // This works for both mouse mode and XR without any changes here.
        if (SpellInputProvider.Instance == null) return;

        Vector3 camPos = SpellInputProvider.Instance.GetCameraPosition();

        // Direction from this tablet to the camera, flattened to XZ plane
        Vector3 dir = camPos - transform.position;
        dir.y = 0;

        // Avoid jitter if camera is directly above/below (nearly zero dir)
        if (dir.sqrMagnitude < 0.001f) return;

        // Face toward the camera (negate so the front of the quad faces out)
        transform.rotation = Quaternion.LookRotation(-dir);
    }
}

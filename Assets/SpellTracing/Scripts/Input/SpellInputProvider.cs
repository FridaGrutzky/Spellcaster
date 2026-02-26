using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Scene singleton. Abstracts all pointer input so every other script
/// is agnostic to whether we're running in the editor (mouse) or on
/// Meta Quest (XR controller / gaze).
///
/// Usage from any other script:
///   Ray ray          = SpellInputProvider.Instance.GetPointerRay();
///   bool held        = SpellInputProvider.Instance.IsPointerHeld();
///   Vector3 camPos   = SpellInputProvider.Instance.GetCameraPosition();
/// </summary>
public class SpellInputProvider : MonoBehaviour
{
    // ── Singleton ────────────────────────────────────────────────────────
    public static SpellInputProvider Instance { get; private set; }

    // ── Input mode ───────────────────────────────────────────────────────
    public enum InputMode
    {
        MouseRaycast,       // Editor / desktop debug
        XRControllerRay,    // Meta Quest — controller forward ray
        XRGazeFallback      // Meta Quest — HMD center-eye forward ray
    }

    [Header("Mode")]
    [Tooltip("MouseRaycast for editor/debug. Switch to XR modes for Quest build.")]
    public InputMode inputMode = InputMode.MouseRaycast;

    // ── XR references (only needed in XR modes) ──────────────────────────
    [Header("XR References (leave empty in mouse mode)")]
    [Tooltip("The Transform of your XR controller ray origin (e.g. RightHandController).")]
    public Transform xrControllerTransform;

    [Tooltip("The center-eye camera transform on your XR Origin.")]
    public Transform xrCenterEyeTransform;

    [Tooltip("XR trigger InputActionReference — used for IsPointerHeld in XR modes.")]
    public InputActionReference xrTriggerAction;

    // ── Unity lifecycle ──────────────────────────────────────────────────
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void OnEnable()
    {
        if (xrTriggerAction != null)
            xrTriggerAction.action.Enable();
    }

    void OnDisable()
    {
        if (xrTriggerAction != null)
            xrTriggerAction.action.Disable();
    }

    // ── Public API ───────────────────────────────────────────────────────

    /// <summary>
    /// Returns a Ray from the active pointer origin into the world.
    /// Mouse mode:   ray from camera through cursor.
    /// XR Controller: ray from controller transform forward.
    /// XR Gaze:      ray from center-eye transform forward.
    /// </summary>
    public Ray GetPointerRay()
    {
        switch (inputMode)
        {
            case InputMode.XRControllerRay:
                if (xrControllerTransform != null)
                    return new Ray(xrControllerTransform.position,
                                   xrControllerTransform.forward);
                Debug.LogWarning("SpellInputProvider: xrControllerTransform not assigned.");
                return FallbackMouseRay();

            case InputMode.XRGazeFallback:
                if (xrCenterEyeTransform != null)
                    return new Ray(xrCenterEyeTransform.position,
                                   xrCenterEyeTransform.forward);
                Debug.LogWarning("SpellInputProvider: xrCenterEyeTransform not assigned.");
                return FallbackMouseRay();

            default: // MouseRaycast
                return FallbackMouseRay();
        }
    }

    /// <summary>
    /// Returns true while the pointer trigger/button is held.
    /// Mouse mode:   left mouse button.
    /// XR modes:     trigger action value above threshold.
    /// </summary>
    public bool IsPointerHeld()
    {
        switch (inputMode)
        {
            case InputMode.XRControllerRay:
            case InputMode.XRGazeFallback:
                if (xrTriggerAction != null)
                    return xrTriggerAction.action.ReadValue<float>() > 0.5f;
                Debug.LogWarning("SpellInputProvider: xrTriggerAction not assigned.");
                return false;

            default: // MouseRaycast
                return Input.GetMouseButton(0);
        }
    }

    /// <summary>
    /// Returns the position to use for billboard Y-axis facing.
    /// Mouse mode:   Camera.main position.
    /// XR modes:     center-eye transform position.
    /// </summary>
    public Vector3 GetCameraPosition()
    {
        switch (inputMode)
        {
            case InputMode.XRControllerRay:
            case InputMode.XRGazeFallback:
                if (xrCenterEyeTransform != null)
                    return xrCenterEyeTransform.position;
                Debug.LogWarning("SpellInputProvider: xrCenterEyeTransform not assigned.");
                return Camera.main.transform.position;

            default: // MouseRaycast
                return Camera.main.transform.position;
        }
    }

    // ── Private helpers ──────────────────────────────────────────────────
    private Ray FallbackMouseRay()
    {
        if (Camera.main != null)
            return Camera.main.ScreenPointToRay(Input.mousePosition);

        Debug.LogError("SpellInputProvider: No Camera.main found for mouse ray.");
        return new Ray(Vector3.zero, Vector3.forward);
    }
}

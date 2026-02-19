using System.Collections;
using UnityEngine;

/// <summary>
/// Attach to: ShapeQuad on each SpellTablet.
///
/// Listens to PathTracer events and drives the shader + visual feedback:
///   OnProgressUpdated → sets _FillAmount on the material
///   OnCompleted       → pulse coroutine (briefly brightens fill color)
///   OnMessUp          → red flash coroutine (fill color lerps to red then back)
/// </summary>
[RequireComponent(typeof(MeshRenderer))]
public class PathVisual : MonoBehaviour
{
    // ── Inspector ─────────────────────────────────────────────────────────
    [Header("References")]
    [Tooltip("The PathTracer on the sibling InputQuad.")]
    public PathTracer pathTracer;

    [Header("Pulse (OnCompleted)")]
    public Color  pulseColor     = Color.white;
    public float  pulseDuration  = 0.4f;

    [Header("Mess-Up Flash (OnMessUp)")]
    public Color  flashColor     = Color.red;
    public float  flashDuration  = 0.3f;

    // ── Private ───────────────────────────────────────────────────────────
    private MeshRenderer _renderer;
    private Material     _mat;          // instance material (not shared)
    private Color        _originalFill;
    private Coroutine    _activeRoutine;

    // Shader property IDs (cached for performance)
    private static readonly int FillAmountID = Shader.PropertyToID("_FillAmount");
    private static readonly int FillColorID  = Shader.PropertyToID("_FillColor");

    // ── Unity lifecycle ───────────────────────────────────────────────────
    void Awake()
    {
        _renderer = GetComponent<MeshRenderer>();

        // Create a per-instance material so tablets don't share state
        _mat = _renderer.material;
    }

    void Start()
    {
        // Cache the original fill color set on the material
        _originalFill = _mat.GetColor(FillColorID);

        // Subscribe to PathTracer events
        if (pathTracer != null)
        {
            pathTracer.OnProgressUpdated.AddListener(OnProgressUpdated);
            pathTracer.OnCompleted.AddListener(OnCompleted);
            pathTracer.OnMessUp.AddListener(OnMessUp);
        }
        else
        {
            Debug.LogWarning($"PathVisual on {gameObject.name}: pathTracer not assigned.");
        }
    }

    void OnDestroy()
    {
        // Unsubscribe to avoid memory leaks
        if (pathTracer != null)
        {
            pathTracer.OnProgressUpdated.RemoveListener(OnProgressUpdated);
            pathTracer.OnCompleted.RemoveListener(OnCompleted);
            pathTracer.OnMessUp.RemoveListener(OnMessUp);
        }
    }

    // ── Event handlers ────────────────────────────────────────────────────
    void OnProgressUpdated(float progress)
    {
        _mat.SetFloat(FillAmountID, progress);
    }

    void OnCompleted()
    {
        if (_activeRoutine != null) StopCoroutine(_activeRoutine);
        _activeRoutine = StartCoroutine(PulseRoutine());
    }

    void OnMessUp()
    {
        if (_activeRoutine != null) StopCoroutine(_activeRoutine);
        _activeRoutine = StartCoroutine(FlashRoutine());
    }

    // ── Coroutines ────────────────────────────────────────────────────────

    /// <summary>Briefly brightens the fill color on completion.</summary>
    IEnumerator PulseRoutine()
    {
        float half = pulseDuration * 0.5f;
        float t    = 0f;

        // Fade to pulse color
        while (t < half)
        {
            t += Time.deltaTime;
            _mat.SetColor(FillColorID, Color.Lerp(_originalFill, pulseColor, t / half));
            yield return null;
        }

        // Fade back
        t = 0f;
        while (t < half)
        {
            t += Time.deltaTime;
            _mat.SetColor(FillColorID, Color.Lerp(pulseColor, _originalFill, t / half));
            yield return null;
        }

        _mat.SetColor(FillColorID, _originalFill);
        _activeRoutine = null;
    }

    /// <summary>Flashes red briefly on mess-up.</summary>
    IEnumerator FlashRoutine()
    {
        float half = flashDuration * 0.5f;
        float t    = 0f;

        // Fade to red
        while (t < half)
        {
            t += Time.deltaTime;
            _mat.SetColor(FillColorID, Color.Lerp(_originalFill, flashColor, t / half));
            yield return null;
        }

        // Fade back
        t = 0f;
        while (t < half)
        {
            t += Time.deltaTime;
            _mat.SetColor(FillColorID, Color.Lerp(flashColor, _originalFill, t / half));
            yield return null;
        }

        _mat.SetColor(FillColorID, _originalFill);
        _activeRoutine = null;
    }
}

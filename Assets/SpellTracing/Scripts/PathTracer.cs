using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Attach to: InputQuad (the invisible collider quad on each SpellTablet).
///
/// Casts a ray from SpellInputProvider each frame while the pointer is held.
/// Converts the hit point to local UV space, projects it onto the PathData
/// waypoints, and advances a high-water-mark progress value.
///
/// Events fired:
///   OnProgressUpdated(float 0-1)  — every frame pointer is on path
///   OnCompleted                   — when highWaterMark reaches 1
///   OnMessUp                      — when pointer strays beyond messUpThreshold
/// </summary>
[RequireComponent(typeof(MeshCollider))]
public class PathTracer : MonoBehaviour
{
    // ── Inspector ─────────────────────────────────────────────────────────
    [Header("Data")]
    public PathData pathData;

    [Header("Events")]
    public UnityEvent<float> OnProgressUpdated;
    public UnityEvent        OnCompleted;
    public UnityEvent        OnMessUp;

    // ── State ─────────────────────────────────────────────────────────────
    private float _highWaterMark = 0f;
    private bool  _completed     = false;
    private MeshCollider _col;

    // ── Unity lifecycle ───────────────────────────────────────────────────
    void Awake()
    {
        _col = GetComponent<MeshCollider>();
    }

    void Update()
    {
        if (_completed) return;
        if (pathData == null || pathData.waypoints == null || pathData.waypoints.Count < 2) return;
        if (SpellInputProvider.Instance == null) return;

        // Only trace while pointer button / trigger is held
        if (!SpellInputProvider.Instance.IsPointerHeld()) return;

        // Cast ray from active input source
        Ray ray = SpellInputProvider.Instance.GetPointerRay();
        if (!Physics.Raycast(ray, out RaycastHit hit)) return;

        // Make sure we hit THIS quad, not another collider
        if (hit.collider != _col) return;

        // Convert world hit point → local UV (0,0 bottom-left, 1,1 top-right)
        Vector2 uv = WorldToUV(hit.point);

        // Project UV onto the waypoint path
        GetPathSample(uv, out float progress, out float dist);

        // ── Three zones ───────────────────────────────────────────────────
        if (dist <= pathData.traceTolerance)
        {
            // On path — advance high-water mark (never decreases)
            if (progress > _highWaterMark)
            {
                _highWaterMark = progress;
                OnProgressUpdated?.Invoke(_highWaterMark);
            }

            // Check completion
            if (_highWaterMark >= 1f)
            {
                _completed = true;
                OnProgressUpdated?.Invoke(1f);
                OnCompleted?.Invoke();
            }
        }
        else if (dist <= pathData.messUpThreshold)
        {
            // Near path but off — pause, no change
        }
        else
        {
            // Too far from path — mess up
            OnMessUp?.Invoke();
        }
    }

    // ── Public utility ────────────────────────────────────────────────────

    /// <summary>Resets progress so the spell can be traced again.</summary>
    public void ResetTrace()
    {
        _highWaterMark = 0f;
        _completed     = false;
        OnProgressUpdated?.Invoke(0f);
    }

    // ── Gizmos ────────────────────────────────────────────────────────────
    void OnDrawGizmos()
    {
        if (pathData == null || pathData.waypoints == null) return;

        List<Vector2> pts = pathData.waypoints;

        for (int i = 0; i < pts.Count; i++)
        {
            Vector3 world = UVToWorld(pts[i]);

            // Sphere at each waypoint
            Gizmos.color = (i == 0) ? Color.green : (i == pts.Count - 1 ? Color.red : Color.yellow);
            Gizmos.DrawSphere(world, 0.02f);

            // Line connecting waypoints
            if (i < pts.Count - 1)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawLine(world, UVToWorld(pts[i + 1]));
            }
        }
    }

    // ── Private helpers ───────────────────────────────────────────────────

    /// <summary>
    /// Converts a world-space point on this quad's surface to UV coords.
    /// Assumes a standard Unity Quad (1x1 units, centered at origin).
    /// </summary>
    Vector2 WorldToUV(Vector3 worldPoint)
    {
        Vector3 local = transform.InverseTransformPoint(worldPoint);
        // Unity Quad verts go from -0.5 to 0.5 in X and Y
        float u = local.x + 0.5f;
        float v = local.y + 0.5f;
        return new Vector2(Mathf.Clamp01(u), Mathf.Clamp01(v));
    }

    /// <summary>
    /// Converts a UV coordinate back to a world-space point (used by Gizmos).
    /// </summary>
    Vector3 UVToWorld(Vector2 uv)
    {
        Vector3 local = new Vector3(uv.x - 0.5f, uv.y - 0.5f, 0f);
        return transform.TransformPoint(local);
    }

    /// <summary>
    /// Projects a UV point onto the nearest segment of the waypoint path.
    /// Returns progress (0-1 along full path) and distance from path.
    /// </summary>
    void GetPathSample(Vector2 uv, out float progress, out float dist)
    {
        List<Vector2> pts = pathData.waypoints;

        // Pre-compute total path length
        float totalLength = 0f;
        for (int i = 0; i < pts.Count - 1; i++)
            totalLength += Vector2.Distance(pts[i], pts[i + 1]);

        float bestDist      = float.MaxValue;
        float bestProgress  = 0f;
        float accumulated   = 0f;

        for (int i = 0; i < pts.Count - 1; i++)
        {
            Vector2 a = pts[i];
            Vector2 b = pts[i + 1];
            float segLen = Vector2.Distance(a, b);

            // Closest point on segment AB to UV point
            float t = Vector2.Dot(uv - a, b - a) / (segLen * segLen);
            t = Mathf.Clamp01(t);
            Vector2 closest = a + t * (b - a);

            float d = Vector2.Distance(uv, closest);

            if (d < bestDist)
            {
                bestDist     = d;
                bestProgress = (accumulated + t * segLen) / totalLength;
            }

            accumulated += segLen;
        }

        progress = bestProgress;
        dist     = bestDist;
    }
}

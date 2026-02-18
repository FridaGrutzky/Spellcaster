using System.Collections.Generic;
using UnityEngine;
using Meta.XR.MRUtilityKit;

public class QRWand : MonoBehaviour
{
    void Start()
    {
        Debug.Log("QRWand START");
    }

    [Header("What to spawn on a QR code")]
    [SerializeField] private GameObject objectPrefab;

    [Header("Optional filtering (leave empty to allow ANY QR payload)")]
    [Tooltip("If empty: spawn for any QR payload. If not empty: only spawn for these payloads.")]
    [SerializeField] private List<string> allowedPayloads = new List<string>();

    [Header("Placement tweak")]
    [SerializeField] private Vector3 localPositionOffset = Vector3.zero;
    [SerializeField] private Vector3 localEulerOffset = Vector3.zero;
    [SerializeField] private Vector3 localScale = Vector3.one;

    // Keep track of spawned objects per QR payload
    private readonly Dictionary<string, GameObject> _spawnedByPayload = new Dictionary<string, GameObject>();

    // (Optional) keep trackable reference if you want extra control later
    private readonly Dictionary<string, MRUKTrackable> _trackableByPayload = new Dictionary<string, MRUKTrackable>();

    /// <summary>
    /// Hook this to MRUK's "On Trackable Added" event (same style as your QRBookManager).
    /// </summary>
    public void OnTrackableAdded(MRUKTrackable trackable)
    {
        Debug.Log("Wand detected");

        if (trackable == null) return;

        // Only handle QR codes
        if (trackable.TrackableType != OVRAnchor.TrackableType.QRCode) return;

        string payload = (trackable.MarkerPayloadString ?? "").Trim();
        if (string.IsNullOrEmpty(payload)) return;

        // Filter (optional)
        if (allowedPayloads != null && allowedPayloads.Count > 0)
        {
            bool allowed = false;
            for (int i = 0; i < allowedPayloads.Count; i++)
            {
                if (string.Equals(payload, allowedPayloads[i].Trim(), System.StringComparison.OrdinalIgnoreCase))
                {
                    allowed = true;
                    break;
                }
            }
            if (!allowed) return;
        }

        // Already spawned for this payload?
        if (_spawnedByPayload.ContainsKey(payload)) return;

        if (objectPrefab == null)
        {
            Debug.LogWarning("QRObjectOverlayManager: objectPrefab is not set.");
            return;
        }

        // Spawn and parent to the trackable so it follows automatically
        GameObject spawned = Instantiate(objectPrefab);

        spawned.transform.SetParent(trackable.transform, worldPositionStays: false);
        spawned.transform.localPosition = localPositionOffset;
        spawned.transform.localRotation = Quaternion.Euler(localEulerOffset);
        spawned.transform.localScale = localScale;

        _spawnedByPayload[payload] = spawned;
        _trackableByPayload[payload] = trackable;

        Debug.Log($">>> QR OVERLAY SPAWNED for payload: {payload}");
    }

    /// <summary>
    /// Hook this to MRUK's "On Trackable Removed" event.
    /// </summary>
    public void OnTrackableRemoved(MRUKTrackable trackable)
    {
        if (trackable == null) return;
        if (trackable.TrackableType != OVRAnchor.TrackableType.QRCode) return;

        string payload = (trackable.MarkerPayloadString ?? "").Trim();
        if (string.IsNullOrEmpty(payload)) return;

        if (_spawnedByPayload.TryGetValue(payload, out GameObject spawned) && spawned != null)
        {
            Destroy(spawned);
        }

        _spawnedByPayload.Remove(payload);
        _trackableByPayload.Remove(payload);

        Debug.Log($">>> QR OVERLAY REMOVED for payload: {payload}");
    }

    /// <summary>
    /// OPTIONAL: If your MRUK setup exposes an "On Trackable Updated" event,
    /// you can hook it here. If you parentar objektet så behövs oftast inte detta.
    /// </summary>
    public void OnTrackableUpdated(MRUKTrackable trackable)
    {
        if (trackable == null) return;
        if (trackable.TrackableType != OVRAnchor.TrackableType.QRCode) return;

        string payload = (trackable.MarkerPayloadString ?? "").Trim();
        if (string.IsNullOrEmpty(payload)) return;

        // If we didn't parent (or you vill göra world offsets), kan du synca manuellt här.
        // Men i denna lösning följer objektet via parenting.
    }
}

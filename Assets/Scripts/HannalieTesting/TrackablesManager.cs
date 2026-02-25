using Meta.XR.MRUtilityKit;
using UnityEngine;
using TMPro;

public class TrackablesManager : MonoBehaviour
{
    [SerializeField] private GameObject trackedObjectPrefab;
    [SerializeField] private GameObject trackedObjectInfoPrefab; // Your World Space Canvas Prefab
    [SerializeField] private GameObject trackedBoundsPrefab;

    [Header("Spell UI Prefabs")]
    [SerializeField] private GameObject fireSpellPrefab;
    [SerializeField] private GameObject lightSpellPrefab;
    [SerializeField] private GameObject windSpellPrefab;

    public void OnTrackableAdded(MRUKTrackable trackable)
    {
        Debug.Log($"Trackable of type {trackable.TrackableType} added");
        var trackedObjectInstance = Instantiate(trackedObjectPrefab, trackable.transform);
        var trackedBoundsInstance = Instantiate(trackedBoundsPrefab, trackedObjectInstance.transform);

        if (trackable.TrackableType == OVRAnchor.TrackableType.QRCode &&
            trackable.MarkerPayloadString != null)
        {
            // 1. Convert to Uppercase to match your IF statements exactly
            string payload = trackable.MarkerPayloadString.Trim().ToUpper();

            // 2. Instantiate the Canvas (the Info Prefab) as a child of the QR
            var canvasInstance = Instantiate(trackedObjectInfoPrefab, trackedObjectInstance.transform);

            // 3. Choose which spell to SPAWN inside that canvas
            GameObject spellToSpawn = null;
            if (payload == "FIRESPELL") spellToSpawn = fireSpellPrefab;
            else if (payload == "LIGHTSPELL") spellToSpawn = lightSpellPrefab;
            else if (payload == "WINDSPELL") spellToSpawn = windSpellPrefab;

            if (spellToSpawn != null)
            {
                // Instantiate the spell UI INSIDE the canvas instance
                Instantiate(spellToSpawn, canvasInstance.transform);
            }

            // --- Bounds Logic ---
            var boundsAreaRect = trackable.PlaneRect.Value;
            trackedBoundsInstance.transform.localScale = new Vector3(boundsAreaRect.width, boundsAreaRect.height, 0.01f);
            trackedBoundsInstance.transform.localPosition = new Vector3(boundsAreaRect.center.x, boundsAreaRect.center.y, 0.0f);
        }
    }

    public void OnTrackableRemoved(MRUKTrackable trackable)
    {
        // When the QR is lost, the whole trackable.gameObject is destroyed, 
        // which automatically kills the UI panels attached to it.
        Destroy(trackable.gameObject);
    }
}
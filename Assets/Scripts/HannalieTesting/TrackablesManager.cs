using Meta.XR.MRUtilityKit;
using UnityEngine;
using TMPro;
using System.Collections.Generic; // Added for Dictionary

public class TrackablesManager : MonoBehaviour
{
    [SerializeField] private GameObject trackedObjectPrefab;
    [SerializeField] private GameObject trackedObjectInfoPrefab;
    [SerializeField] private GameObject trackedBoundsPrefab;

    [Header("Spell UI Prefabs")]
    [SerializeField] private GameObject fireSpellPrefab;
    [SerializeField] private GameObject lightSpellPrefab;
    [SerializeField] private GameObject windSpellPrefab;

    // Dictionary to map trackables to their spawned UI objects for reliable cleanup
    private Dictionary<MRUKTrackable, GameObject> _spawnedObjects = new Dictionary<MRUKTrackable, GameObject>();

    public void OnTrackableAdded(MRUKTrackable trackable)
    {
        Debug.Log($"Trackable of type {trackable.TrackableType} added");
        var trackedObjectInstance = Instantiate(trackedObjectPrefab, trackable.transform);
        var trackedBoundsInstance = Instantiate(trackedBoundsPrefab, trackedObjectInstance.transform);

        // Store the instance so we can find it later in OnTrackableRemoved
        if (!_spawnedObjects.ContainsKey(trackable))
        {
            _spawnedObjects.Add(trackable, trackedObjectInstance);
        }

        if (trackable.TrackableType == OVRAnchor.TrackableType.QRCode &&
            trackable.MarkerPayloadString != null)
        {
            string payload = trackable.MarkerPayloadString.Trim().ToUpper();

            /* --- COMMENTED OUT OLD INDIVIDUAL SPAWNING LOGIC (KEEPING FOR SAFETY) ---
            var canvasInstance = Instantiate(trackedObjectInfoPrefab, trackedObjectInstance.transform);

            GameObject spellToSpawn = null;
            if (payload == "FIRESPELL") spellToSpawn = fireSpellPrefab;
            else if (payload == "LIGHTSPELL") spellToSpawn = lightSpellPrefab;
            else if (payload == "WINDSPELL") spellToSpawn = windSpellPrefab;

            if (spellToSpawn != null)
            {
                Instantiate(spellToSpawn, canvasInstance.transform);
            }
            ------------------------------------------------------- */

            // --- ALL-IN-ONE SEARCH LOGIC ---
            GameObject fireUI = null;
            GameObject lightUI = null;
            GameObject windUI = null;

            foreach (Transform child in trackedObjectInstance.GetComponentsInChildren<Transform>(true))
            {
                if (child.name == "FIRESPELL") fireUI = child.gameObject;
                if (child.name == "LIGHTSPELL") lightUI = child.gameObject;
                if (child.name == "WINDSPELL") windUI = child.gameObject;
            }

            if (fireUI) fireUI.SetActive(false);
            if (lightUI) lightUI.SetActive(false);
            if (windUI) windUI.SetActive(false);

            if (payload == "FIRESPELL" && fireUI) fireUI.SetActive(true);
            else if (payload == "LIGHTSPELL" && lightUI) lightUI.SetActive(true);
            else if (payload == "WINDSPELL" && windUI) windUI.SetActive(true);

            // --- Bounds Logic (Centered) ---
            var boundsAreaRect = trackable.PlaneRect.Value;
            Vector3 localCenter = new Vector3(boundsAreaRect.center.x, boundsAreaRect.center.y, 0.0f);

            trackedObjectInstance.transform.localPosition = localCenter;
            trackedBoundsInstance.transform.localScale = new Vector3(boundsAreaRect.width, boundsAreaRect.height, 0.01f);
            trackedBoundsInstance.transform.localPosition = localCenter;
        }
    }

    public void OnTrackableRemoved(MRUKTrackable trackable)
    {
        // Check our dictionary to see if we have a UI instance associated with this trackable
        if (_spawnedObjects.TryGetValue(trackable, out GameObject instance))
        {
            if (instance != null)
            {
                Destroy(instance);
            }
            _spawnedObjects.Remove(trackable);
        }

        /* --- COMMENTED OUT PREVIOUS LOGIC ---
        // When the QR is lost, the whole trackable.gameObject is destroyed, 
        // which automatically kills the UI panels attached to it.
        // Destroy(trackable.gameObject);
        --------------------------------------- */
    }
}
using Meta.XR.MRUtilityKit;
using UnityEngine;
using TMPro;

public class TrackablesManager : MonoBehaviour
{
    [SerializeField] private GameObject trackedObjectPrefab;
    [SerializeField] private GameObject trackedObjectInfoPrefab;
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
            // We search for the specific GameObjects by name inside the prefab we just spawned.
            GameObject fireUI = null;
            GameObject lightUI = null;
            GameObject windUI = null;

            // This looks through EVERY child and grandchild of the spawned prefab to find the names
            foreach (Transform child in trackedObjectInstance.GetComponentsInChildren<Transform>(true))
            {
                /* --- PREVIOUS FIND LOGIC (COMMENTED OUT) ---
                // Transform fireUI = trackedObjectInstance.transform.Find("FIRESPELL");
                // Transform lightUI = trackedObjectInstance.transform.Find("LIGHTSPELL");
                // Transform windUI = trackedObjectInstance.transform.Find("WINDSPELL");
                ---------------------------------------------- */

                if (child.name == "FIRESPELL") fireUI = child.gameObject;
                if (child.name == "LIGHTSPELL") lightUI = child.gameObject;
                if (child.name == "WINDSPELL") windUI = child.gameObject;
            }

            // 1. Turn EVERYTHING off first so they don't overlap
            if (fireUI) fireUI.SetActive(false);
            if (lightUI) lightUI.SetActive(false);
            if (windUI) windUI.SetActive(false);

            // 2. Turn on ONLY the one that matches the QR code payload
            if (payload == "FIRESPELL" && fireUI) fireUI.SetActive(true);
            else if (payload == "LIGHTSPELL" && lightUI) lightUI.SetActive(true);
            else if (payload == "WINDSPELL" && windUI) windUI.SetActive(true);
            // -----------------------------

            // --- Bounds Logic (Centered) ---
            var boundsAreaRect = trackable.PlaneRect.Value;
            Vector3 localCenter = new Vector3(boundsAreaRect.center.x, boundsAreaRect.center.y, 0.0f);

            // Apply center to both the object and the bounds
            trackedObjectInstance.transform.localPosition = localCenter;
            trackedBoundsInstance.transform.localScale = new Vector3(boundsAreaRect.width, boundsAreaRect.height, 0.01f);
            trackedBoundsInstance.transform.localPosition = localCenter;
        }
    }

    public void OnTrackableRemoved(MRUKTrackable trackable)
    {
        // When the QR is lost, the whole trackable.gameObject is destroyed, 
        // which automatically kills the UI panels attached to it.
        Destroy(trackable.gameObject);
    }
}
using Meta.XR.MRUtilityKit;
using UnityEngine;
using TMPro;

public class TrackablesManager : MonoBehaviour
{
    [SerializeField] private GameObject trackedObjectPrefab;
    [SerializeField] private GameObject trackedObjectInfoPrefab;
    [SerializeField] private GameObject trackedBoundsPrefab;

    // --- NEW UI PANEL REFERENCES ---
    [Header("Spell UI Panels")]
    [SerializeField] private GameObject fireSpellPanel;
    [SerializeField] private GameObject lightSpellPanel;
    [SerializeField] private GameObject windSpellPanel;

    public void OnTrackableAdded(MRUKTrackable trackable)
    {
        Debug.Log($"Trackable of type {trackable.TrackableType} added");
        var trackedObjectInstance = Instantiate(trackedObjectPrefab, trackable.transform);
        var trackedBoundsInstance = Instantiate(trackedBoundsPrefab, trackedObjectInstance.transform);

        if (trackable.TrackableType == OVRAnchor.TrackableType.QRCode &&
            trackable.MarkerPayloadString != null)
        {
            /* COMMENTED OUT ORIGINAL PAYLOAD TEXT LOGIC
            var trackedObjectInfo = Instantiate(trackedObjectInfoPrefab,
                trackedObjectInstance. transform);
            trackedObjectInfo.GetComponentInChildren<TextMeshProUGUI>(). text = 
                $"<color=red> QR Code Payload:</color>{trackable.MarkerPayloadString}";
            */

            // --- NEW LOGIC FOR SPELL ACTIVATION ---
            string payload = trackable.MarkerPayloadString.Trim(); // Trim removes accidental spaces

            if (payload == "FIRESPELL")
            {
                fireSpellPanel.SetActive(true);
            }
            else if (payload == "LIGHTSPELL")
            {
                lightSpellPanel.SetActive(true);
            }
            else if (payload == "WINDSPELL")
            {
                windSpellPanel.SetActive(true);
            }

            var boundsAreaRect = trackable.PlaneRect.Value;
            trackedBoundsInstance.transform.localScale = new
                Vector3(boundsAreaRect.width, boundsAreaRect.height, 0.01f);

            trackedBoundsInstance.transform.localPosition =
                new Vector3(boundsAreaRect.center.x, boundsAreaRect.center.y, 0.0f);
        }
    }

    public void OnTrackableRemoved(MRUKTrackable trackable)
    {
        Debug.Log($"Trackable of type {trackable.TrackableType} removed");

        // --- NEW LOGIC TO HIDE PANELS WHEN QR IS LOST ---
        // (Optional: Remove this if you want the UI to stay visible)
        string payload = trackable.MarkerPayloadString?.Trim();
        if (payload == "firespell") fireSpellPanel.SetActive(false);
        if (payload == "lightspell") lightSpellPanel.SetActive(false);
        if (payload == "windspell") windSpellPanel.SetActive(false);

        Destroy(trackable.gameObject);
    }
}
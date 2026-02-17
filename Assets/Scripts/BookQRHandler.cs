using UnityEngine;
using Meta.XR.MRUtilityKit;
using TMPro;

public class QRBookManager : MonoBehaviour
{
    [SerializeField] private GameObject _bookUI;
    [SerializeField] private TextMeshProUGUI _pageText;

    // We'll keep a reference to the current code we are reading
    private string _currentPayload = "";

    // 1. Hook to "On Trackable Added"
    public void OnTrackableAdded(MRUKTrackable trackable)
    {
        // Debug line: This will show up in your Unity Console (or headset logs)
        Debug.Log(">>> SCANNER DETECTED: " + trackable.MarkerPayloadString);

        if (trackable.TrackableType == OVRAnchor.TrackableType.QRCode)
        {
            _currentPayload = trackable.MarkerPayloadString;
            _bookUI.SetActive(true);
            UpdateSpellText(_currentPayload);
        }
    }

    // 2. Hook to "On Trackable Removed"
    public void OnTrackableRemoved(MRUKTrackable trackable)
    {
        // Only hide if the code being removed is the one we are currently looking at
        if (trackable.MarkerPayloadString == _currentPayload)
        {
            Debug.Log(">>> SCANNER LOST: " + _currentPayload);
            _bookUI.SetActive(false);
            _currentPayload = "";
        }
    }

    private void UpdateSpellText(string payload)
    {
        // Using ToUpper() ensures that "firespell" and "FIRESPELL" both work
        switch (payload.ToUpper().Trim())
        {
            case "FIRESPELL":
                _pageText.text = "<color=red>FIRE SPELL</color>\n\nSummoning a wall of flame...";
                break;
            case "LIGHTSPELL":
                _pageText.text = "<color=yellow>LIGHT SPELL</color>\n\nA holy radiance appears.";
                break;
            case "WINDSPELL":
                _pageText.text = "<color=cyan>WIND SPELL</color>\n\nThe storm obeys your command.";
                break;
            default:
                _pageText.text = "Reading scroll...\nDetected: " + payload;
                break;
        }
    }
}
using UnityEngine;
using Meta.XR.MRUtilityKit;
using TMPro;

public class OLDQRBookManager : MonoBehaviour
{
    [SerializeField] private GameObject _bookUI;
    [SerializeField] private TextMeshProUGUI _pageText;

    // We store the trackable object itself to monitor its status
    private MRUKTrackable _activeTrackable;

    public void OnTrackableAdded(MRUKTrackable trackable)
    {
        if (trackable.TrackableType == OVRAnchor.TrackableType.QRCode)
        {
            Debug.Log(">>> QR FOUND: " + trackable.MarkerPayloadString);
            _activeTrackable = trackable;
            _bookUI.SetActive(true);
            UpdateSpellText(trackable.MarkerPayloadString);
        }
    }

    private void Update()
    {
        // If we don't have a trackable, make sure UI is off and exit
        if (_activeTrackable == null)
        {
            if (_bookUI.activeSelf) _bookUI.SetActive(false);
            return;
        }

        // STRICT CHECK: If the GameObject is disabled by MRUK or 
        // the tracking state becomes invalid, hide immediately.
        if (!_activeTrackable.gameObject.activeInHierarchy)
        {
            Debug.Log(">>> QR VISIBILITY LOST");
            ClearActiveTrackable();
        }
    }

    // Still keep this as a backup in case the system actually removes the anchor
    public void OnTrackableRemoved(MRUKTrackable trackable)
    {
        if (trackable == _activeTrackable)
        {
            Debug.Log(">>> QR ANCHOR REMOVED FROM MEMORY");
            ClearActiveTrackable();
        }
    }

    private void ClearActiveTrackable()
    {
        _bookUI.SetActive(false);
        _activeTrackable = null;
    }

    private void UpdateSpellText(string payload)
    {
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
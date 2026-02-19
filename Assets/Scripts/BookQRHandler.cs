using UnityEngine;
using Meta.XR.MRUtilityKit;
using TMPro;

public class QRBookManager : MonoBehaviour
{
    [SerializeField] private GameObject _bookUI;
    [SerializeField] private TextMeshProUGUI _pageText;

    // Triggered by MRUK Event Utilities
    public void OnTrackableAdded(MRUKTrackable trackable)
    {
        if (trackable.TrackableType == OVRAnchor.TrackableType.QRCode)
        {
            _bookUI.SetActive(true);
            UpdateSpellText(trackable.MarkerPayloadString);
        }
    }

    // Triggered by your UI Button (On Click)
    public void CloseUI()
    {
        _bookUI.SetActive(false);
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
            default:
                _pageText.text = "Reading scroll...\nDetected: " + payload;
                break;
        }
    }
}
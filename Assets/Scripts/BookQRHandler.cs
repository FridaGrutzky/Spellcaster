using UnityEngine;
using Meta.XR.MRUtilityKit;
using TMPro; // Needed for TextMeshPro

public class BookQRHandler : MonoBehaviour
{
    [SerializeField] private GameObject _headsetUI;
    [SerializeField] private TextMeshProUGUI _pageText; // Drag your UI's text component here

    private int _visibleQRCount = 0;

    public void OnTrackableAdded(MRUKTrackable trackable)
    {
        if (trackable.TrackableType != OVRAnchor.TrackableType.QRCode) return;

        string qrValue = trackable.MarkerPayloadString;
        if (string.IsNullOrEmpty(qrValue)) return;

        // The Switch Statement: Check which page we just 'opened'
        switch (qrValue)
        {
            case "FIRESPELL":
                _pageText.text = "mlem";
                ShowUI();
                break;

            case "LIGHTSPELL":
                _pageText.text = "dog";
                ShowUI();
                break;

            case "WINDSPELL":
                _pageText.text = "yippie";
                ShowUI();
                break;

            default:
                Debug.Log($"Scanned unknown QR: {qrValue}");
                break;
        }
    }

    private void ShowUI()
    {
        _visibleQRCount++;
        if (_headsetUI != null) _headsetUI.SetActive(true);
    }

    public void OnTrackableRemoved(MRUKTrackable trackable)
    {
        if (trackable.TrackableType != OVRAnchor.TrackableType.QRCode) return;

        _visibleQRCount--;

        if (_visibleQRCount <= 0 && _headsetUI != null)
        {
            _headsetUI.SetActive(false);
        }
    }
}
using UnityEngine;
using Meta.XR.MRUtilityKit;

public class BookQRHandler : MonoBehaviour
{
    [SerializeField] private GameObject _bookPrefab;
    private GameObject _activeBookUI;
    private MRUKTrackable _currentTrackable;

    public void OnTrackableAdded(MRUKTrackable trackable)
    {
        // Safety check: Ensure the anchor data exists
        if (trackable.Anchor == null) return;

        // In Unity 6 / MRUK, Labels are now a list or bitmask inside the Anchor
        // We use HasLabel to check for the internal "MARKER" string
        if (!trackable.HasLabel("MARKER"))
        {
            return;
        }

        Debug.Log("Success! QR Trackable detected.");

        // Singleton UI logic
        if (_activeBookUI == null)
        {
            _activeBookUI = Instantiate(_bookPrefab);
        }

        // Attach UI to the QR
        _activeBookUI.transform.SetParent(trackable.transform, false);
        _activeBookUI.transform.localPosition = Vector3.zero;
        _activeBookUI.transform.localRotation = Quaternion.identity;

        _activeBookUI.SetActive(true);
        _currentTrackable = trackable;
    }

    public void OnTrackableRemoved(MRUKTrackable trackable)
    {
        if (_currentTrackable == trackable)
        {
            if (_activeBookUI != null) _activeBookUI.SetActive(false);
            _currentTrackable = null;
        }
    }
}
using UnityEngine;
using Meta.XR.MRUtilityKit;

public class QRDebugLogger : MonoBehaviour
{
    public void OnTrackableAdded(MRUKTrackable trackable)
    {
        if (trackable == null) return;

        // Check if it's a QR code
        if (trackable.TrackableType == OVRAnchor.TrackableType.QRCode)
        {
            Debug.Log("QRCODEDETECTED");
        }
    }
}

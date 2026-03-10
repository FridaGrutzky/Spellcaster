using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class QRMuzzleTracker : MonoBehaviour
{
    public string qrMarkerName = "WandTipQR"; // Name of the image in your Library
    public Transform virtualMuzzle;          // Drag your Muzzle object here
    private ARTrackedImageManager imageManager;

    void Awake() => imageManager = GetComponentInParent<ARTrackedImageManager>();

    void OnEnable() => imageManager.trackedImagesChanged += OnChanged;
    void OnDisable() => imageManager.trackedImagesChanged -= OnChanged;

    void OnChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var trackedImage in eventArgs.updated)
        {
            // If the camera sees the QR code on your wand
            if (trackedImage.referenceImage.name == qrMarkerName)
            {
                // Move the virtual muzzle to the physical QR code's position
                virtualMuzzle.position = trackedImage.transform.position;
                virtualMuzzle.rotation = trackedImage.transform.rotation;
            }
        }
    }
}
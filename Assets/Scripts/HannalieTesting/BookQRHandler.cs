using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class BookQRHandler : MonoBehaviour
{
    public string bookMarkerName = "BookQR";
    public GameObject qrAnchor;
    public TutorialChecklist checklist; // 1. ADD THIS LINE

    private ARTrackedImageManager imageManager;

    void Awake() => imageManager = GetComponentInParent<ARTrackedImageManager>();

    void OnEnable() => imageManager.trackedImagesChanged += OnChanged;
    void OnDisable() => imageManager.trackedImagesChanged -= OnChanged;

    void OnChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var trackedImage in eventArgs.added)
        {
            if (trackedImage.referenceImage.name == bookMarkerName)
            {
                qrAnchor.SetActive(true);

                // 2. ADD THIS LINE TO STRIKETHROUGH THE TEXT
                if (checklist != null) checklist.CompleteStep1();
            }
        }

        foreach (var trackedImage in eventArgs.updated)
        {
            if (trackedImage.referenceImage.name == bookMarkerName)
            {
                // Ensure it stays active and checklist stays updated
                qrAnchor.SetActive(true);
                if (checklist != null) checklist.CompleteStep1();
            }
        }
    }
}
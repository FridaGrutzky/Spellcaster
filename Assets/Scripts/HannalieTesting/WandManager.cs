using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.InputSystem;

public class WandManager : MonoBehaviour
{
    [Header("QR Tracking")]
    public string qrName = "WandQR"; // Name from your Reference Image Library
    public Transform muzzleTransform; // The invisible 'tip' of your wand

    [Header("Spells")]
    public GameObject spellPrefab;
    public float spellSpeed = 15f;
    public InputActionReference castAction;

    private ARTrackedImageManager imageManager;

    void Awake() => imageManager = GetComponent<ARTrackedImageManager>();

    void OnEnable()
    {
        imageManager.trackedImagesChanged += OnQRChanged;
        if (castAction != null) castAction.action.Enable();
    }

    void OnDisable()
    {
        imageManager.trackedImagesChanged -= OnQRChanged;
        if (castAction != null) castAction.action.Disable();
    }

    void Update()
    {
        // Check for manual trigger pull
        if (castAction != null && castAction.action.WasPressedThisFrame())
        {
            Cast();
        }
    }

    void OnQRChanged(ARTrackedImagesChangedEventArgs args)
    {
        foreach (var image in args.updated)
        {
            if (image.referenceImage.name == qrName)
            {
                // Sync virtual muzzle to physical QR code
                muzzleTransform.position = image.transform.position;
                muzzleTransform.rotation = image.transform.rotation;
            }
        }
    }

    public void Cast()
    {
        GameObject spell = Instantiate(spellPrefab, muzzleTransform.position, muzzleTransform.rotation);
        if (spell.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.linearVelocity = muzzleTransform.forward * spellSpeed;
        }
        Destroy(spell, 5f);
    }
}
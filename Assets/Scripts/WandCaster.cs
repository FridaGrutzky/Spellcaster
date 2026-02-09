using UnityEngine;
using UnityEngine.XR;

public class WandCaster : MonoBehaviour
{
    public Transform castPoint;

    InputDevice device;

    void Start()
    {
        device = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
    }

    bool wasPressed = false;

    void Update()
    {
        if (device.TryGetFeatureValue(CommonUsages.triggerButton, out bool pressed))
        {
            if (pressed && !wasPressed)
            {
                CastSpell();
            }
            wasPressed = pressed;
        }
    }

    void CastSpell()
    {
        if (SpellManager.Instance.currentSpell == null) return;

        Instantiate(
            SpellManager.Instance.currentSpell.spellPrefab,
            castPoint.position,
            castPoint.rotation
        );
    }
}

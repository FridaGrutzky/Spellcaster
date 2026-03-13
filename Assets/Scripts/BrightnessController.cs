using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BrightnessController : MonoBehaviour
{
    public float startLight = -5;
    public Volume volume;
    private ColorAdjustments colorAdjustments;

    void Start()
    {
        // Hämtar Color Adjustments från Volume
        volume.profile.TryGet(out colorAdjustments);

        // Startljus
        colorAdjustments.postExposure.value = startLight;
    }

    public void SetBrightness(float value)
    {
        // value kan vara t.ex. -5 (mörkt) till +5 (ljust)
        colorAdjustments.postExposure.value = value;
    }
}

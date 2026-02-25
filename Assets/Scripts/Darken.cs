using UnityEngine;

public class Darken : Spell
{
    public BrightnessController brightnessController;

    public void CastSpell()
    {
        brightnessController.SetBrightness(-5);
    }
}

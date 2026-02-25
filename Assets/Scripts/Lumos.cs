using UnityEngine;

public class Lumos : Spell
{
    public BrightnessController brightnessController;
    public void CastSpell()
    {
        brightnessController.SetBrightness(0);
    }
}

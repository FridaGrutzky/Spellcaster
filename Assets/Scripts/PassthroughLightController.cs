using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PassthroughLightController : MonoBehaviour
{
    public Image darkOverlay;
    public float fadeDuration = 2f;


    void Update() // Remove when the script is connected to the spell
    {
        // Press L to simulate Light Spell
        if (Input.GetKeyDown(KeyCode.L))
        {
            CastLightSpell();
        }
    }
    public void CastLightSpell()
    {
        StopAllCoroutines();
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        float startAlpha = darkOverlay.color.a;
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float t = time / fadeDuration;

            float newAlpha = Mathf.Lerp(startAlpha, 0f, t);
            darkOverlay.color = new Color(0, 0, 0, newAlpha);

            yield return null;
        }

        darkOverlay.color = new Color(0, 0, 0, 0f);
    }


    // Connect The script to the Spell System In SpellManager or WandCaster, when the light spell succeeds add:      passthroughLightController.CastLightSpell();

}

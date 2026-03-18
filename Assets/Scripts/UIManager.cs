using UnityEngine;

public class UIManager : MonoBehaviour
{
    // Dra dit UI-elementet du vill aktivera i Inspektorn
    public GameObject uiToActivate;
    public GameObject musicUI;
    public GameObject lightUI;
    public GameObject lightningUI;

    // Tid i sekunder innan UI:t aktiveras
    public float delay = 20f;

    void Start()
    {
        // Startar Coroutine som väntar och aktiverar UI:t
        StartCoroutine(ActivateUIAfterDelay());
    }

    private System.Collections.IEnumerator ActivateUIAfterDelay()
    {
        // Väntar 'delay' sekunder
        yield return new WaitForSeconds(delay);

        // Aktiverar UI:t
        uiToActivate.SetActive(true);
        musicUI.SetActive(true);
        lightUI.SetActive(true);
        lightningUI.SetActive(true);
    }
}

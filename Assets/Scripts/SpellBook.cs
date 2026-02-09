using UnityEngine;

public class SpellBook : MonoBehaviour
{
    public GameObject spellUI;

    public void OpenBook()
    {
        spellUI.SetActive(true);
    }

    public void CloseBook()
    {
        spellUI.SetActive(false);
    }
}

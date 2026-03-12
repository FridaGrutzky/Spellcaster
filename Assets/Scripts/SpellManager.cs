using UnityEngine;

public class SpellManager : MonoBehaviour
{
    public static SpellManager Instance;

    public Spell currentSpell;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void SelectSpell(Spell spell)
    {
        currentSpell = spell;
        Debug.Log("Vald spell: " + spell.spellName);
    }
}

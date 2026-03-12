using UnityEngine;

public class SpellManager : MonoBehaviour
{
    [SerializeField] private BaseSpellGesture[] spells;

    void Update()
    {
        if (spells == null || spells.Length == 0)
            return;

        BaseSpellGesture bestSpell = null;
        float bestScore = float.MinValue;

        // Låt alla spells uppdatera sin recognition
        foreach (var spell in spells)
        {
            if (spell == null) continue;

            spell.Process();

            if (spell.IsComplete && spell.CanCast)
            {
                if (spell.Score > bestScore)
                {
                    bestScore = spell.Score;
                    bestSpell = spell;
                }
            }
        }

        // Bara en spell får skjutas
        if (bestSpell != null)
        {
            bestSpell.Cast();

            // Reseta alla så att ingen annan skjuts samtidigt
            foreach (var spell in spells)
            {
                if (spell != null)
                    spell.ResetGesture();
            }
        }
    }
}
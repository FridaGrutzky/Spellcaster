using UnityEngine;
using ExtralityLab;

public class SpellManager : MonoBehaviour
{
    [SerializeField] private BaseSpellGesture[] spells;
    [SerializeField] private MqttClientExampleSendRGB mqttSender;

    void Update()
    {
        if (spells == null || spells.Length == 0)
            return;

        BaseSpellGesture bestSpell = null;
        float bestScore = float.MinValue;

        foreach (var spell in spells)
        {
            if (spell == null)
                continue;

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

        if (bestSpell != null)
        {
            bestSpell.Cast();

            if (bestSpell is LightningSpell)
            {
                if (mqttSender != null)
                {
                    mqttSender.TriggerLightningSpell();
                }
                else
                {
                    Debug.LogWarning("MQTT sender saknas i SpellManager.");
                }
            }

            foreach (var spell in spells)
            {
                if (spell != null)
                    spell.ResetGesture();
            }
        }
    }
}
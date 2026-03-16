using UnityEngine;
using ExtralityLab;

public class SpellManager : MonoBehaviour
{
    [SerializeField] private PassthroughLightController passthroughLightController;
    [SerializeField] private BaseSpellGesture[] spells;
    [SerializeField] private MqttSpellSender mqttSender;
    [SerializeField] private float globalSpellCooldown = 1f;
    [SerializeField] private Spawner spawner;
    private float nextCastTime = 0f;
    public GameObject introUI;

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

        if (bestSpell != null && Time.time >= nextCastTime)
        {
            bestSpell.Cast();
            nextCastTime = Time.time + globalSpellCooldown;

            if (mqttSender != null)
            {
                if (bestSpell is LightningSpell)
                {
                    mqttSender.TriggerLightningSpell();
                    passthroughLightController.CastLightSpell();

                    if (spawner.isActiveAndEnabled)
                    {
                        spawner.ActivateSpawner();
                    }
                }
                else if (bestSpell is CircleSpell)
                {
                    mqttSender.TriggerCircleSpell();
                }
                else if(bestSpell is WindSpell)
                {
                    Object.Destroy(introUI);
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
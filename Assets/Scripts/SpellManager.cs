using UnityEngine;
using ExtralityLab;

public class SpellManager : MonoBehaviour
{
    [SerializeField] private BaseSpellGesture[] spells;
    [SerializeField] private MqttSpellSender mqttSender;
    [SerializeField] private float globalSpellCooldown = 1f;
    private float nextCastTime = 0f;
    public PassthroughLightController passthroughLightController;
    public GameObject introUI;
    public GameObject musicUI;
    public GameObject lightUI;
    public GameObject lightningUI;

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
                    lightUI.SetActive(true);
                }
                else if (bestSpell is CircleSpell)
                {
                    mqttSender.TriggerCircleSpell();
                    musicUI.SetActive(true);
                }
                else if(bestSpell is WindSpell)
                {
                    Object.Destroy(introUI);
                    lightningUI.SetActive(true);
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
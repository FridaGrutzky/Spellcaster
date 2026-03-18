using UnityEngine;
using UnityEngine.UI; // Lägg till denna för att hantera UI-text
using TMPro;           // Använd denna om du kör TextMeshPro (rekommenderas)
using ExtralityLab;

public class SpellManager1 : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private BaseSpellGesture[] spells;
    [SerializeField] private MqttSpellSender mqttSender;
    [SerializeField] private float globalSpellCooldown = 1f;

    [Header("Score System")]
    [SerializeField] private TextMeshProUGUI scoreText; // Dra in ditt Text-objekt här
    private int totalScore = 0;

    [Header("References")]
    public PassthroughLightController passthroughLightController;
    public GameObject introUI;

    private float nextCastTime = 0f;

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
            // --- Logik för Score ---
            AddScore(100); // Du kan ändra detta till t.ex. (int)bestSpell.Score om du vill ha dynamisk poäng

            bestSpell.Cast();
            nextCastTime = Time.time + globalSpellCooldown;

            if (mqttSender != null)
            {
                if (bestSpell is LightningSpell)
                {
                    mqttSender.TriggerLightningSpell();
                    passthroughLightController.CastLightSpell();
                }
                else if (bestSpell is CircleSpell)
                {
                    mqttSender.TriggerCircleSpell();
                }
                else if (bestSpell is WindSpell)
                {
                    if (introUI != null) Object.Destroy(introUI);
                }
            }

            foreach (var spell in spells)
            {
                if (spell != null)
                    spell.ResetGesture();
            }
        }
    }

    // Metod för att hantera poängökning och uppdatera UI
    private void AddScore(int amount)
    {
        totalScore += amount;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + totalScore.ToString();
        }
    }
}
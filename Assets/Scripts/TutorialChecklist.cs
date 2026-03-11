using TMPro;
using UnityEngine;

public class TutorialChecklist : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text step1Text;
    public TMP_Text step2Text;
    public TMP_Text step3Text;

    [Header("Step text")]
    public string step1Label = "Shake the wand";
    public string step2Label = "Perform a spell from the book";
    public string step3Label = "Kill an enemy with a spell";

    private bool step1Done;
    private bool step2Done;
    private bool step3Done;

    void Start()
    {
        RefreshUI();
    }

    public void CompleteStep1()
    {
        step1Done = true;
        RefreshUI();
    }

    public void CompleteStep2()
    {
        step2Done = true;
        RefreshUI();
    }

    public void CompleteStep3()
    {
        step3Done = true;
        RefreshUI();
    }

    private void RefreshUI()
    {
        step1Text.text = step1Done
            ? $"<color=#888888><s>{step1Label}</s></color>"
            : step1Label;

        step2Text.text = step2Done
            ? $"<color=#888888><s>{step2Label}</s></color>"
            : step2Label;

        step3Text.text = step3Done
            ? $"<color=#888888><s>{step3Label}</s></color>"
            : step3Label;
    }
}
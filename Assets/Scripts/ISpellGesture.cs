using UnityEngine;

public interface ISpellGesture
{
    float Score { get; }          // Hur bra matchen är just nu (0..1 eller högre)
    bool IsComplete { get; }      // Är gesture färdig?
    bool CanCast { get; }         // Är cooldown klar?

    void Process();
    void Cast();
    void ResetGesture();
}
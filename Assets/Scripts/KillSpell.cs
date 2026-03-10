using UnityEngine;

public class KillSpell : Spell
{
    public void CastSpell()
    {
        var creatures = FindObjectsOfType<FlyingCreature>();
        
        foreach (var creature in creatures)
        {
            Destroy(creature.gameObject);
        }
    }
}

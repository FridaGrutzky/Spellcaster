using UnityEngine;

public class CastFireball : Spell
{
    public void CastSpell()
    {
        if (spellPrefab == null || muzzlePoint == null) return;

        GameObject spell = Instantiate(spellPrefab, muzzlePoint.position, muzzlePoint.rotation);
        Rigidbody rb = spell.GetComponent<Rigidbody>();

        if (rb != null)
        {
            // This shoots it in the direction the Muzzle is pointing
            rb.linearVelocity = muzzlePoint.forward * spellPower;
        }

        Destroy(spell, 5f);
    }
}

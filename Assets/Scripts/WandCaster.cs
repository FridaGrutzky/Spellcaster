using UnityEngine;
using UnityEngine.InputSystem;

public class WandCaster : MonoBehaviour
{
    [Header("Magic Settings")]
    public GameObject spellPrefab;
    public Transform muzzlePoint;
    public float spellPower = 15f;

    [Header("Debug Auto-Cast")]
    public bool useAutoCast = true; // Check this box to shoot automatically
    public float shootInterval = 2.0f; // Seconds between shots
    private float timer;

    [Header("Input (Optional for now)")]
    public InputActionReference castAction;

    void Update()
    {
        // Path A: Auto-shooting for testing
        if (useAutoCast)
        {
            timer += Time.deltaTime;
            if (timer >= shootInterval)
            {
                CastSpell();
                timer = 0;
            }
        }

        // Path B: Manual shooting (for when you have the goggles)
        // We check if castAction is assigned to avoid errors
        if (castAction != null && castAction.action != null)
        {
            if (castAction.action.WasPressedThisFrame())
            {
                CastSpell();
            }
        }
    }

    void CastSpell()
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
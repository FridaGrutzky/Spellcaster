using UnityEngine;

public abstract class BaseSpellGesture : MonoBehaviour, ISpellGesture
{
    [Header("Refs")]
    public TipToPlane2D tracker;
    public Transform head;
    public Transform tipSphere;

    [Header("Cast")]
    public GameObject spellPrefab;
    public float projectileSpeed = 15f;
    public float projectileLifetime = 3f;
    public float cooldown = 0.25f;

    protected float cooldownUntil;

    public bool CanCast => Time.time >= cooldownUntil;

    public abstract float Score { get; }
    public abstract bool IsComplete { get; }

    public abstract void Process();
    public abstract void ResetGesture();

    public virtual void Cast()
    {
        if (!tipSphere || !head || !spellPrefab)
            return;

        GameObject g = Instantiate(spellPrefab, tipSphere.position, tipSphere.rotation);

        Vector3 directionAwayFromYou = (tipSphere.position - head.position).normalized;
        g.transform.forward = directionAwayFromYou;

        Rigidbody rb = g.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = directionAwayFromYou * projectileSpeed;
        }

        Destroy(g, projectileLifetime);
        cooldownUntil = Time.time + cooldown;
    }
}
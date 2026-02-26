using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Scene singleton. Tracks completion of all three spell traces and fires
/// a win event when all are done.
///
/// Also listens to OnMessUp from all tracers for optional global responses
/// (cooldown, rollback, etc).
///
/// Attach to: A new empty GameObject called "GameManager" in the scene root.
/// </summary>
public class GameManager : MonoBehaviour
{
    // ── Singleton ─────────────────────────────────────────────────────────
    public static GameManager Instance { get; private set; }

    // ── Inspector ─────────────────────────────────────────────────────────
    [Header("Spell Tracers")]
    [Tooltip("Drag the InputQuad of SpellTablet_Lightning here.")]
    public PathTracer lightningTracer;

    [Tooltip("Drag the InputQuad of SpellTablet_Swirl here.")]
    public PathTracer swirlTracer;

    [Tooltip("Drag the InputQuad of SpellTablet_Flame here.")]
    public PathTracer flameTracer;

    [Header("Mess-Up Settings")]
    [Tooltip("If true, a mess-up rolls back progress slightly.")]
    public bool rollbackOnMessUp = false;

    [Range(0f, 0.2f)]
    [Tooltip("How much progress to subtract on mess-up (if rollback enabled).")]
    public float rollbackAmount = 0.05f;

    [Tooltip("Seconds to ignore input after a mess-up (optional cooldown).")]
    public float messUpCooldown = 0f;

    [Header("Events")]
    [Tooltip("Fired when all three spells are successfully traced.")]
    public UnityEvent OnAllSpellsComplete;

    // ── State ─────────────────────────────────────────────────────────────
    private bool[] _spellsDone = new bool[3];
    private float  _cooldownTimer = 0f;

    // ── Unity lifecycle ───────────────────────────────────────────────────
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        SubscribeTracer(lightningTracer, 0);
        SubscribeTracer(swirlTracer,     1);
        SubscribeTracer(flameTracer,     2);
    }

    void Update()
    {
        if (_cooldownTimer > 0f)
            _cooldownTimer -= Time.deltaTime;
    }

    void OnDestroy()
    {
        UnsubscribeTracer(lightningTracer);
        UnsubscribeTracer(swirlTracer);
        UnsubscribeTracer(flameTracer);
    }

    // ── Private helpers ───────────────────────────────────────────────────
    void SubscribeTracer(PathTracer tracer, int index)
    {
        if (tracer == null)
        {
            Debug.LogWarning($"GameManager: tracer at index {index} not assigned.");
            return;
        }

        // Use lambdas with captured index so we know which spell completed
        tracer.OnCompleted.AddListener(() => OnSpellCompleted(index));
        tracer.OnMessUp.AddListener(OnSpellMessUp);
    }

    void UnsubscribeTracer(PathTracer tracer)
    {
        if (tracer == null) return;
        tracer.OnCompleted.RemoveAllListeners();
        tracer.OnMessUp.RemoveAllListeners();
    }

    void OnSpellCompleted(int index)
    {
        _spellsDone[index] = true;
        Debug.Log($"GameManager: spell {index} complete.");

        // Check if all three are done
        if (_spellsDone[0] && _spellsDone[1] && _spellsDone[2])
        {
            Debug.Log("GameManager: all spells complete — firing win event.");
            OnAllSpellsComplete?.Invoke();
        }
    }

    void OnSpellMessUp()
    {
        if (_cooldownTimer > 0f) return;

        Debug.Log("GameManager: mess-up detected.");

        if (messUpCooldown > 0f)
            _cooldownTimer = messUpCooldown;

        // Rollback is handled per-tracer if enabled — extend PathTracer
        // to expose a RollBack(float) method and call it here if needed.
    }
}

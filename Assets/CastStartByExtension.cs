using UnityEngine;

public class CastStartByExtension : MonoBehaviour
{
    [Header("Refs")]
    public Transform head;            // CenterEyeAnchor
    public Transform hand;            // RightControllerAnchor eller hand transform
    public PatternMatcher2D matcher;  // din matcher
    public int selectedSpellIndex = 0; // 0=circle, 1=lightning, 2=third

    [Header("Tuning")]
    public float minDistance = 0.35f;      // meter från huvudet
    public float forwardFromHeadDot = 0.6f; // hand framför (0.6 ~ 53° kon)
    public float aimForwardDot = 0.6f;      // handen pekar ungefär samma riktning som blicken
    public float releaseDelay = 0.15f;      // liten hysteresis så det inte fladdrar

    bool _casting;
    float _lastGoodTime;

    void Update()
    {
        if (!head || !hand || !matcher) return;

        Vector3 toHand = hand.position - head.position;
        float dist = toHand.magnitude;

        float aheadDot = Vector3.Dot(head.forward, toHand.normalized);      // >0 = framför
        float aimDot = Vector3.Dot(hand.forward, head.forward);           // 1 = samma riktning

        bool extended =
            dist > minDistance &&
            aheadDot > forwardFromHeadDot &&
            aimDot > aimForwardDot;

        if (extended)
        {
            _lastGoodTime = Time.time;

            if (!_casting)
            {
                _casting = true;
                matcher.StartPattern(selectedSpellIndex);
            }
        }
        else
        {
            // släpp inte direkt (anti-fladder)
            if (_casting && (Time.time - _lastGoodTime) > releaseDelay)
            {
                _casting = false;
                matcher.StopPattern();
            }
        }
    }
}
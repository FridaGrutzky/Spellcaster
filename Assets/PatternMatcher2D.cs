using UnityEngine;

public class PatternMatcher2D : MonoBehaviour
{
    [CreateAssetMenu(menuName = "Spells/Pattern2D")]
    public class Pattern2D : ScriptableObject
    {
        public string spellName;
        public Vector2[] points;     // checkpoint-punkter i planet (lokal x,y)
        public float tolerance = 0.10f;
        public float maxTime = 3.0f;
    }
    
    public TipToPlane2D tracker;     // scriptet som ger PlanePoint
    public Pattern2D[] patterns;

    int _active = -1;
    int _i = 0;
    float _t0;

    public void StartPattern(int index)
    {
        _active = index;
        _i = 0;
        _t0 = Time.time;
    }

    public void StopPattern() => _active = -1;

    void Update()
    {
        if (_active < 0 || tracker == null) return;

        var ptn = patterns[_active];

        if (Time.time - _t0 > ptn.maxTime)
        {
            ResetProgress();
            return;
        }

        Vector2 p = tracker.PlanePoint;
        Vector2 target = ptn.points[_i];

        if (Vector2.Distance(p, target) <= ptn.tolerance)
        {
            _i++;
            if (_i >= ptn.points.Length)
            {
                CastSpell(ptn.spellName);
                ResetProgress();
            }
        }
    }

    void ResetProgress()
    {
        _i = 0;
        _t0 = Time.time;
    }

    void CastSpell(string spellName)
    {
        Debug.Log("CAST: " + spellName);
        // TODO: din spell
    }
}
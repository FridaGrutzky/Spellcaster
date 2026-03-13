using UnityEngine;

public class CircleMusicSpell : BaseSpellGesture
{
    [Header("Circle template")]
    public float radius = 0.25f;
    public float radialTolerance = 0.35f;
    public int sectors = 16;
    public int requiredSectors = 5;
    public float maxTime = 10f;

    private bool[] visited;
    private int visitedCount;
    private float t0;
    private bool complete;
    private float score;

    public override float Score => score;
    public override bool IsComplete => complete;

    void Awake()
    {
        EnsureVisitedArray();
        ResetGesture();
    }

    void OnValidate()
    {
        EnsureVisitedArray();
    }

    void EnsureVisitedArray()
    {
        int n = Mathf.Max(8, sectors);
        if (visited == null || visited.Length != n)
            visited = new bool[n];
    }

    public override void Process()
    {
        if (tracker == null || complete)
            return;

        if (Time.time - t0 > maxTime)
        {
            ResetGesture();
            return;
        }

        Vector2 p = tracker.P;
        float r = p.magnitude;

        // Ju närmare rätt radie, desto bättre score
        float radiusError = Mathf.Abs(r - radius);

        if (radiusError > radialTolerance)
            return;

        float ang = Mathf.Atan2(p.y, p.x) * Mathf.Rad2Deg;
        if (ang < 0f) ang += 360f;

        int s = Mathf.FloorToInt(ang / 360f * visited.Length);
        s = Mathf.Clamp(s, 0, visited.Length - 1);

        if (!visited[s])
        {
            visited[s] = true;
            visitedCount++;
        }

        float sectorProgress = (float)visitedCount / requiredSectors;
        float radiusQuality = 1f - Mathf.Clamp01(radiusError / radialTolerance);

        // Score = hur långt du kommit + hur ren cirkeln är
        score = sectorProgress + radiusQuality * 0.25f;

        if (visitedCount >= requiredSectors)
        {
            complete = true;
        }
    }

    public override void ResetGesture()
    {
        EnsureVisitedArray();

        for (int i = 0; i < visited.Length; i++)
            visited[i] = false;

        visitedCount = 0;
        t0 = Time.time;
        complete = false;
        score = 0f;
    }
}
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PathData", menuName = "SpellTracing/PathData")]
public class PathData : ScriptableObject
{
    [Header("Spell Info")]
    public string spellName = "Unnamed Spell";

    [Header("Textures")]
    public Texture2D shapeTexture;
    public Texture2D fillMapTexture;

    [Header("Tracing Settings")]
    public List<Vector2> waypoints = new List<Vector2>();

    [Range(0.01f, 0.2f)]
    public float traceTolerance = 0.08f;

    [Range(0.05f, 0.5f)]
    public float messUpThreshold = 0.2f;
}

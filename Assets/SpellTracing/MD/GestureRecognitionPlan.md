# 3D Spell Gesture Recognition — Implementation Plan
**Project:** Hand-tracked spell casting via 3D finger movement pattern matching
**Status:** Backend first — no live hand tracking yet, mouse/keyboard debug input only

---

## Environment
- Unity 6000.0.34f1
- Universal Render Pipeline (URP)
- Meta Quest (target platform) — Meta XR SDK / OVRPlugin
- Windows 11 development machine
- Input System: Unity New Input System + Meta hand tracking (OVRSkeleton) for Quest build
- Existing project already has: SpellManager singleton, OVRCameraRig, hand tracking infrastructure from other team members

---

## Concept Overview

The player performs a spell by moving their index finger (or debug cursor) through the air in a recognizable shape. The system:

1. Detects when casting has begun (pinch gesture on Quest / mouse button in debug)
2. Records 3D finger positions over time
3. Projects them onto a **reference plane** derived from the player's head position/forward at cast start
4. Compares the resulting 2D trace to a reference PathData shape using **DTW (Dynamic Time Warping)**
5. If similarity is above threshold → spell fires
6. If gesture times out or similarity is too low → fail state

The player is assumed to be **mostly stationary**. The reference plane updates automatically if the player moves beyond a set distance threshold, but only when no cast is in progress.

---

## Reference Plane System

### How it works
At cast start, the system snapshots:
- Player head **world position**
- Player head **forward direction** (flattened to XZ, i.e. ignoring vertical look angle)
- World **up** vector

These define a local 2D coordinate system — essentially the vertical plane directly in front of the player at that moment. All finger positions during the gesture are projected onto this plane and converted to local 2D coordinates for pattern matching.

### Re-snapshot conditions
The reference plane updates (outside of active casts only) when:
- Player head moves more than `positionThreshold` world units from the last snapshot (suggested default: 0.75f)

Rotation is **not** tracked for re-snapshot purposes — the player's facing direction at cast-start is sufficient for each individual gesture.

### Upright lock
At cast start, the dot product of the player's head up vector and world up vector is checked. If the player is tilted beyond ~40 degrees from upright, the cast is rejected silently. This prevents spells being cast while crouching, lying down, or in awkward positions, and ensures the reference plane is always roughly vertical.

---

## PathData (Existing — Unchanged)
```
List<Vector2> waypoints     — reference shape in normalized 2D space (0,0)–(1,1)
float traceTolerance        — not used in gesture system (kept for tablet legacy)
float messUpThreshold       — not used in gesture system (kept for tablet legacy)
string spellName
Texture2D shapeTexture      — used in spellbook UI guide
Texture2D fillMapTexture    — used in spellbook UI guide
```
PathData assets already exist: PathData_Lightning, PathData_Swirl, PathData_Flame.
Waypoints encode the intended 2D shape. The gesture system uses these as the DTW reference sequence.

---

## New Scripts Required

### GestureInputProvider.cs
**Replaces / extends SpellInputProvider for gesture context.**

Responsibilities:
- Maintains the current reference plane snapshot (position + forward + up)
- Exposes `Vector3 GetFingerTipWorld()` — returns world-space finger position
  - Debug mode: projects mouse screen position onto a plane at fixed depth from camera
  - Quest mode: reads `OVRSkeleton` index finger tip bone world position
- Exposes `bool IsCastingGesture()` — true while cast input is held
  - Debug mode: left mouse button held
  - Quest mode: pinch gesture detected (OVRHand.GetFingerIsPinching)
- Exposes `Plane GetReferencePlane()` — current casting plane
- Exposes `Vector2 ProjectToPlane(Vector3 worldPos)` — projects world pos to normalized 2D on reference plane
- Handles re-snapshot logic (distance threshold check each frame, only when not casting)
- Handles upright check at cast start

```csharp
public enum GestureInputMode
{
    MouseDebug,       // editor / flat screen
    XRHandTracking    // Meta Quest OVRSkeleton
}
```

### GestureRecorder.cs
Records finger positions during an active cast gesture.

Responsibilities:
- Subscribes to cast start/end from GestureInputProvider
- Each frame while casting: calls `GestureInputProvider.ProjectToPlane()` on current finger world pos
- Accumulates a `List<Vector2>` of projected 2D points (sampled at fixed interval, e.g. every 0.05s, to avoid density artifacts)
- On cast end: passes recorded point list to GestureMatcher
- Enforces a maximum gesture duration (e.g. 3 seconds) — auto-cancels if exceeded
- Enforces a minimum point count before bothering to match (e.g. 8 points)
- Fires events:
  - `OnGestureRecorded(List<Vector2> points)`
  - `OnGestureCancelled()`

### GestureMatcher.cs
Compares a recorded gesture to all known PathData shapes.

Responsibilities:
- Holds references to all PathData assets (Lightning, Swirl, Flame)
- On `OnGestureRecorded`: normalizes the incoming point list to 0–1 space (same as PathData waypoints)
- Runs DTW against each PathData waypoint list
- If best match score is below `matchThreshold` → fires `OnSpellMatched(PathData spell)`
- If no match is good enough → fires `OnNoMatch()`
- DTW is implemented internally (no external library needed — simple O(n*m) implementation)

```
Normalization: translate so min point = (0,0), scale so max extent = 1 on longest axis.
This makes the comparison scale and position invariant — only shape matters.
```

### GestureDebugVisualizer.cs *(editor only, can be stripped for build)*
Draws the recorded gesture in the scene for tuning purposes.
- Draws the raw 3D finger trail as a Gizmo line
- Draws the projected 2D points on the reference plane
- Draws the best-matching PathData waypoints overlaid for comparison
- Shows the DTW score in a debug GUI label

---

## DTW Algorithm (inside GestureMatcher)

Dynamic Time Warping finds the minimum-cost alignment between two sequences of 2D points. It handles:
- Gestures performed at different speeds
- Slight positional offsets within the normalized space
- Sequences of different lengths

```
Input:  List<Vector2> recorded  (normalized, from GestureRecorder)
        List<Vector2> reference (PathData.waypoints)
Output: float score (lower = better match)

Algorithm:
  Build cost matrix: cost[i,j] = Vector2.Distance(recorded[i], reference[j])
  Fill DTW matrix with cumulative minimum path
  Return dtw[n-1, m-1] / (n + m)   ← normalized by sequence length
```

A `matchThreshold` of around `0.15–0.25` is a reasonable starting point (tune per spell).

---

## Debug Mode (Mouse)

Since Quest hand tracking is not available during backend development:

- `GestureInputMode.MouseDebug` in GestureInputProvider
- Finger world position = ray from camera through mouse position, intersected with reference plane at a fixed depth
- Left mouse button held = casting gesture active
- The reference plane in debug mode = a vertical plane at Z = 0 facing the camera (or camera forward at startup)
- This gives a usable 2D trace on a flat monitor that exercises the full pipeline

---

## Integration with Existing SpellManager

When `GestureMatcher.OnSpellMatched(PathData spell)` fires:
- Look up the corresponding `Spell` object by `spell.spellName`
- Call `SpellManager.Instance.SelectSpell(spell)` then immediately cast

`GestureManager.cs` (a thin coordinator, replaces the old GameManager for this system) handles this wiring.

---

## Dependency Order

```
GestureInputProvider   — implement first, everything depends on it
GestureRecorder        — depends on GestureInputProvider
GestureMatcher         — depends on GestureRecorder + PathData assets
GestureManager         — depends on GestureMatcher + SpellManager
GestureDebugVisualizer — implement last, depends on all of the above
```

---

## Folder Structure
```
Assets/SpellTracing/
    Scripts/
        Input/
            GestureInputProvider.cs   ← was SpellInputProvider
        GestureRecorder.cs
        GestureMatcher.cs
        GestureManager.cs
        GestureDebugVisualizer.cs
    ScriptableObjects/
        PathData_Lightning.asset
        PathData_Swirl.asset
        PathData_Flame.asset
```

---

## Resuming in a New Chat — Entry Points

Always paste this full document when resuming. Then use:

```
"I am on GestureInputProvider — here is the existing PathData.cs: [paste]"
"I am on GestureRecorder — here is GestureInputProvider.cs: [paste]"
"I am on GestureMatcher — here are GestureRecorder.cs and PathData.cs: [paste]"
"I am on GestureManager — here are GestureMatcher.cs and SpellManager.cs: [paste]"
```

---

## Open Questions (resolve before Quest integration)
- Confirm OVRSkeleton bone ID for index finger tip (likely `OVRSkeleton.BoneId.Hand_IndexTip`)
- Confirm pinch detection API (`OVRHand.GetFingerIsPinching(OVRHand.HandFinger.Index)`)
- Decide: one-handed only, or support both hands casting different spells?
- Decide: should the gesture have a visible particle trail so the player gets feedback while drawing?

---
GestureRecognitionPlan.md — v1.0

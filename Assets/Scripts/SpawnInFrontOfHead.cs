// using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class SpawnInFrontOfHead : MonoBehaviour
{
    public float distance = 1.2f; // meters in front of the player
    public float heightOffset = -0.1f; // slightly below eye level
    public bool repositionOnEnable = true;

    Transform head;

    private void Awake()
    {
        head = Camera.main ? Camera.main.transform : null;
    }
    private void OnEnable()
    {
        if (repositionOnEnable) Place();
    }
    [ContextMenu ("Place Now")]
    public void Place()
    {
        if (!head) head = Camera.main ? Camera.main.transform : null;
        if (!head) return;

        // forward flattened so UI doesn't tilt up/down with head pitch
        Vector3 forward = head.forward;
        forward.y = 0f;
        forward = forward.sqrMagnitude < 0.001f ? head.forward : forward.normalized;

        Vector3 pos = head.position + forward * distance;
        pos.y = head.position.y + heightOffset;

        transform.position = pos;
        transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
    }
}

using UnityEngine;

public class LaserPointer : MonoBehaviour
{
    public Transform rayStart;
    public float length = 5f;

    private LineRenderer lr;

    void Start()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = 2;
    }

    void Update()
    {
        Vector3 start = rayStart.position;
        Vector3 end = start + rayStart.forward * length;

        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
    }
}
using UnityEngine;

public class HandTest : MonoBehaviour
{
    private OVRHand _hand;
    private OVRSkeleton _skeleton;

    void Start()
    {
        _hand = GetComponent<OVRHand>();
        _skeleton = GetComponent<OVRSkeleton>();
    }

    void Update()
    {
        if (_hand == null || !_hand.IsTracked) return;
        if (_skeleton == null || !_skeleton.IsInitialized) return;

        foreach (var bone in _skeleton.Bones)
        {
            if (bone.Id == OVRSkeleton.BoneId.Hand_IndexTip)
            {
                Debug.Log($"IndexTip world position: {bone.Transform.position}");
            }
        }
    }
}
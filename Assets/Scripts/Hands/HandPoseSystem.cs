using UnityEngine;

public class HandPoseSystem : MonoBehaviour
{
    public enum Handedness
    {
        Left = 0,
        Right = 1
    }
    
    [SerializeField]
    HandPoseController leftHandPoseController;
    [SerializeField]
    HandPoseController rightHandPoseController;
    [SerializeField]
    public float poseUpdateSpeed;

    public void init()
    {
        leftHandPoseController.init(Handedness.Left);
        rightHandPoseController.init(Handedness.Right);
    }
}

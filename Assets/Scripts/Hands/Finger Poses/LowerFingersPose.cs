using VrTheatre.Hands;
using UnityEngine;

[CreateAssetMenu(fileName = "Lower Fingers Pose", menuName = "Hand Poses")]
public class LowerFingersPose : ScriptableObject
{
    public FingerPose middlePose;
    
    public FingerPose ringPose;
    
    public FingerPose pinkyPose;
}

using VrTheatre.Hands;
using UnityEngine;

[CreateAssetMenu(fileName = "Index Pose", menuName = "Hand Poses")]
public class IndexPose : ScriptableObject
{
    FingerPose indexPose;

    public void Lerp(IndexPose other, float lerp)
    {
        indexPose.Lerp(other.indexPose, lerp);
    }
    
    public static IndexPose Lerp(IndexPose a, IndexPose b, float t)
    {
        var new_pose = new IndexPose();
        new_pose.indexPose.bend = Mathf.Lerp(a.indexPose.bend, b.indexPose.bend, t);
        new_pose.indexPose.rootRotation = Vector3.Slerp(a.indexPose.rootRotation, b.indexPose.rootRotation, t);
        return new_pose;
    }
}

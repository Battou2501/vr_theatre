using UnityEngine;
using VrTheatre.Hands;

public class FingerController : MonoBehaviour
{
    [SerializeField] 
    Transform rootBone;
    [SerializeField] 
    Transform secondPhalanxBone;
    [SerializeField] 
    Transform thirdPhalanxBone;
    [SerializeField]
    Vector3[] minBendRotations;
    [SerializeField]
    Vector3[] maxBendRotations;

    public void set_pose(FingerPose pose)
    {
        rootBone.rotation = Quaternion.Euler(pose.rootRotation);
        secondPhalanxBone.rotation = Quaternion.Slerp(Quaternion.Euler(minBendRotations[0]), Quaternion.Euler(maxBendRotations[0]), pose.bend);
    }


}

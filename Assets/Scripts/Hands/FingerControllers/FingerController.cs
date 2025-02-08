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
    Quaternion[] minBendRotations;
    [SerializeField]
    Quaternion[] maxBendRotations;

    public void set_pose(FingerPose pose)
    {
        if(
              rootBone == null 
           || secondPhalanxBone == null 
           || thirdPhalanxBone == null 
           || minBendRotations is not {Length: 2} 
           || maxBendRotations is not {Length: 2}
           ) 
            return;
        
        rootBone.localRotation = pose.rootRotation;
        secondPhalanxBone.localRotation = Quaternion.Slerp(minBendRotations[0], maxBendRotations[0], pose.bend);
        thirdPhalanxBone.localRotation = Quaternion.Slerp(minBendRotations[1], maxBendRotations[1], pose.bend);
    }


}

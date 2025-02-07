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
        if(
              rootBone == null 
           || secondPhalanxBone == null 
           || thirdPhalanxBone == null 
           || minBendRotations is not {Length: 2} 
           || maxBendRotations is not {Length: 2}
           ) 
            return;
        
        rootBone.rotation = Quaternion.Euler(pose.rootRotation);
        secondPhalanxBone.rotation = Quaternion.Slerp(Quaternion.Euler(minBendRotations[0]), Quaternion.Euler(maxBendRotations[0]), pose.bend);
        thirdPhalanxBone.rotation = Quaternion.Slerp(Quaternion.Euler(minBendRotations[1]), Quaternion.Euler(maxBendRotations[1]), pose.bend);
    }


}

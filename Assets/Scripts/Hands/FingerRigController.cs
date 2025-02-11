#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using VrTheatre.Hands;

public class FingerRigController : MonoBehaviour
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

    public void set_pose(FingerPoseData poseData)
    {
        if(
              rootBone == null 
           || secondPhalanxBone == null 
           || thirdPhalanxBone == null 
           || minBendRotations is not {Length: 2} 
           || maxBendRotations is not {Length: 2}
           ) 
            return;
        
        rootBone.localRotation = poseData.rootRotation;
        secondPhalanxBone.localRotation = Quaternion.Slerp(minBendRotations[0], maxBendRotations[0], poseData.bend);
        thirdPhalanxBone.localRotation = Quaternion.Slerp(minBendRotations[1], maxBendRotations[1], poseData.bend);
    }
    
#if UNITY_EDITOR
    [CustomEditor(typeof(FingerRigController))]
    public class FingerRigControllerEditor : Editor
    {
        
        float bend;
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            
        }
    }
#endif
}

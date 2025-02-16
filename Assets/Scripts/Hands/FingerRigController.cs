#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using VrTheatre.Hands;

public class FingerRigController : MonoBehaviour
{
    [SerializeField] 
    public Transform rootBone;
    [SerializeField] 
    public Transform secondPhalanxBone;
    [SerializeField] 
    public Transform thirdPhalanxBone;
    //[SerializeField]
    //Quaternion[] minBendRotations;
    //[SerializeField]
    //Quaternion[] maxBendRotations;

    public void set_pose(FingerPoseData poseData)
    {
        if(
              rootBone == null 
           || secondPhalanxBone == null 
           || thirdPhalanxBone == null 
           //|| minBendRotations is not {Length: 2} 
           //|| maxBendRotations is not {Length: 2}
           ) 
            return;
        
        rootBone.localRotation = poseData.rootRotation;
        secondPhalanxBone.localRotation = poseData.secondPhalanxRotation;
        thirdPhalanxBone.localRotation = poseData.thirdPhalanxRotation;
        
        //secondPhalanxBone.localRotation = Quaternion.Slerp(minBendRotations[0], maxBendRotations[0], poseData.bend);
        //thirdPhalanxBone.localRotation = Quaternion.Slerp(minBendRotations[1], maxBendRotations[1], poseData.bend);
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

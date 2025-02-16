using UnityEditor;
using UnityEngine;
using VrTheatre.Hands;

public class HandPoseBakeTool : MonoBehaviour
{
    public FingerRigController thumbRigController;
    public FingerRigController indexRigController;
    public FingerRigController middleRigController;
    public FingerRigController ringRigController;
    public FingerRigController pinkyRigController;
    
    
#if UNITY_EDITOR
    [CustomEditor(typeof(HandPoseBakeTool))]
    public class HandPoseBakeToolEditor : Editor
    {
        private HandPoseBakeTool tool;
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            tool = (HandPoseBakeTool)target;
            
            if(tool == null) return;

            if (GUILayout.Button("Bake Hand Pose"))
            {
                bake_pose();
            }
            
        }

        void bake_pose()
        {
            if(tool.thumbRigController == null) return;
            if(tool.indexRigController == null) return;
            if(tool.middleRigController == null) return;
            if(tool.ringRigController == null) return;
            if(tool.pinkyRigController == null) return;

            var poseFile = ScriptableObject.CreateInstance<FingersPoseSO>();
            poseFile.pose_data = new FingerPoseData[5];
            poseFile.pose_data[0].rootRotation = tool.thumbRigController.rootBone.localRotation;
            poseFile.pose_data[0].secondPhalanxRotation = tool.thumbRigController.secondPhalanxBone.localRotation;
            poseFile.pose_data[0].thirdPhalanxRotation = tool.thumbRigController.thirdPhalanxBone.localRotation;
            
            poseFile.pose_data[1].rootRotation = tool.indexRigController.rootBone.localRotation;
            poseFile.pose_data[1].secondPhalanxRotation = tool.indexRigController.secondPhalanxBone.localRotation;
            poseFile.pose_data[1].thirdPhalanxRotation = tool.indexRigController.thirdPhalanxBone.localRotation;
            
            poseFile.pose_data[2].rootRotation = tool.middleRigController.rootBone.localRotation;
            poseFile.pose_data[2].secondPhalanxRotation = tool.middleRigController.secondPhalanxBone.localRotation;
            poseFile.pose_data[2].thirdPhalanxRotation = tool.middleRigController.thirdPhalanxBone.localRotation;
            
            poseFile.pose_data[3].rootRotation = tool.ringRigController.rootBone.localRotation;
            poseFile.pose_data[3].secondPhalanxRotation = tool.ringRigController.secondPhalanxBone.localRotation;
            poseFile.pose_data[3].thirdPhalanxRotation = tool.ringRigController.thirdPhalanxBone.localRotation;
            
            poseFile.pose_data[4].rootRotation = tool.pinkyRigController.rootBone.localRotation;
            poseFile.pose_data[4].secondPhalanxRotation = tool.pinkyRigController.secondPhalanxBone.localRotation;
            poseFile.pose_data[4].thirdPhalanxRotation = tool.pinkyRigController.thirdPhalanxBone.localRotation;
            
            AssetDatabase.CreateAsset(poseFile, "Assets/Hand poses/New Pose.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = poseFile;
        }
    }
#endif
}

using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using VrTheatre.Hands;
using Zenject;
using static HandControlSystem;
using static UnityEngine.ScriptableObject;

public class HandPoseController : MonoBehaviour
{
    [SerializeField]
    protected FingerRigController[] fingerControllers;
    [SerializeField]
    protected FingersPoseSO poseIdle;
    [SerializeField]
    protected FingersPoseSO poseSpecial;
    [SerializeField]
    protected FingersPoseSO poseFist;
    
    private bool current_trigger_touched;
    private float current_trigger_value;
    private bool current_thumb_touched;
    private float current_grip_value;

    //private InputAction trigger_touched_action;
    //private InputAction trigger_value_action;
    //private InputAction thumb_touched_action;
    //private InputAction grip_value_action;
    
    private bool is_initialized; 
    private bool is_left;
    private int fingers_count;
    //private InputActionMap action_map;
    private FingersPoseSO target_pose;
    private FingersPoseSO current_pose;
    private FingersPoseSO grab_override_pose;
    private HandControlSystem hand_control_system;
    private VrInputSystem _vrInputSystem;
    
    private float pose_update_speed => hand_control_system == null ? 1 : hand_control_system.poseUpdateSpeed;
    public bool has_grab_override_pose => grab_override_pose != null;

    [Inject]
    public void Construct(HandControlSystem handControlSystem, VrInputSystem vrInputSystem)
    {
        hand_control_system = handControlSystem;
        _vrInputSystem = vrInputSystem;
    }

    public void init(Handedness handedness)
    {
        is_left = handedness == Handedness.Left;
        
        if(fingerControllers == null) return;
        if(poseIdle == null || poseIdle.pose_data == null) return;
        if(poseSpecial == null || poseSpecial.pose_data == null) return;
        if(poseFist == null || poseFist.pose_data == null) return;

        var lengths = new[] {poseIdle.pose_data.Length, poseSpecial.pose_data.Length, poseFist.pose_data.Length, fingerControllers.Length};
        
        if(lengths.Distinct().Count() > 1) return;

        fingers_count = fingerControllers.Length;
        
        current_pose = CreateInstance<FingersPoseSO>();
        
        target_pose = CreateInstance<FingersPoseSO>();
        
        current_pose.set_data(poseIdle);
        
        target_pose.set_data(poseIdle);

        set_fingers_pose();

        if (is_left)
        {
            _vrInputSystem.leftThumbTouchedChanged += update_thumb_touched;
            _vrInputSystem.leftTriggerTouchedChanged += update_trigger_touched;
            _vrInputSystem.leftTriggerValueChanged += update_trigger_value;
            _vrInputSystem.leftGripValueChanged += update_grip_value;
        }
        else
        {
            _vrInputSystem.rightThumbTouchedChanged += update_thumb_touched;
            _vrInputSystem.rightTriggerTouchedChanged += update_trigger_touched;
            _vrInputSystem.rightTriggerValueChanged += update_trigger_value;
            _vrInputSystem.rightGripValueChanged += update_grip_value;
        }

        is_initialized = true;
    }

    private void Update()
    {
        update_current_pose();
    }

    private void update_thumb_touched(bool state)
    {
        current_thumb_touched = state;
        update_target_pose();
    }
    
    private void update_trigger_touched(bool state)
    {
        current_trigger_touched = state;
        update_target_pose();
    }
    
    private void update_trigger_value(float value)
    {
        current_trigger_value = value;
        update_target_pose();
    }
    
    private void update_grip_value(float value)
    {
        current_grip_value = value;
        update_target_pose();
    }
    
    public void set_grab_override_pose(FingersPoseSO pose)
    {
        if(pose == null || pose.pose_data == null ||  pose.pose_data.Length != fingers_count) return;
        
        grab_override_pose = pose;
        
        target_pose.set_data(grab_override_pose);
    }

    public void remove_grab_override_pose()
    {
        grab_override_pose = null;
        update_target_pose();
        //update_current_pose();
    }

    public void update_current_pose()
    {
        if (!is_initialized || current_pose.Equals(target_pose)) return;
        
        //current_pose.Lerp(target_pose, Time.deltaTime * poseUpdateSpeed);
        current_pose.move_towards_linear_ease_out(target_pose, Time.deltaTime * pose_update_speed);
        
        set_fingers_pose();
    }

    private void set_fingers_pose()
    {
        for (var i = 0; i < fingers_count; i++)
        {
            fingerControllers[i].set_pose(current_pose.pose_data[i]);
        }
    }
    
    private void update_target_pose()
    {
        if (has_grab_override_pose) return;

        update_thumb_target_pose();
        
        update_index_finger_target_pose();

        update_grip_fingers_target_pose();
    }
    
    private void update_thumb_target_pose()
    {
        if (!current_thumb_touched)
        {
            if(!target_pose.pose_data[0].Equals(poseSpecial.pose_data[0]))
                target_pose.pose_data[0] = poseSpecial.pose_data[0];
            
            return;
        }
        
        FingerPoseData.Lerp(poseIdle.pose_data[0], poseFist.pose_data[0], Mathf.Max(current_grip_value, current_trigger_value), ref target_pose.pose_data[0]);
    }
    
    private void update_index_finger_target_pose()
    {
        if (!current_trigger_touched)
        {
            if(!target_pose.pose_data[1].Equals(poseSpecial.pose_data[1]))
                target_pose.pose_data[1] = poseSpecial.pose_data[1];
            
            return;
        }

        FingerPoseData.Lerp(poseIdle.pose_data[1], poseFist.pose_data[1], current_trigger_value, ref target_pose.pose_data[1]);
    }

    private void update_grip_fingers_target_pose()
    {
        for (var i = 2; i < 5; i++)
        {
            FingerPoseData.Lerp(poseIdle.pose_data[i], poseFist.pose_data[i], current_grip_value, ref target_pose.pose_data[i]);
        }
    }
    
#if UNITY_EDITOR
    [CustomEditor(typeof(HandPoseController))]
    public class HandPoseControllerEditor : Editor
    {
        
        HandPoseController controller;
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            controller = (HandPoseController)target;
            
            if(controller == null) return;

            if (GUILayout.Button("Set Idle Pose"))
            {
                if(controller.poseIdle == null) return;

                Undo.RecordObject(controller.fingerControllers[0].rootBone.transform, "Set Idle Pose");
                Undo.RecordObject(controller.fingerControllers[0].secondPhalanxBone.transform, "Set Idle Pose");
                Undo.RecordObject(controller.fingerControllers[0].thirdPhalanxBone.transform, "Set Idle Pose");
                
                Undo.RecordObject(controller.fingerControllers[1].rootBone.transform, "Set Idle Pose");
                Undo.RecordObject(controller.fingerControllers[1].secondPhalanxBone.transform, "Set Idle Pose");
                Undo.RecordObject(controller.fingerControllers[1].thirdPhalanxBone.transform, "Set Idle Pose");
                
                Undo.RecordObject(controller.fingerControllers[2].rootBone.transform, "Set Idle Pose");
                Undo.RecordObject(controller.fingerControllers[2].secondPhalanxBone.transform, "Set Idle Pose");
                Undo.RecordObject(controller.fingerControllers[2].thirdPhalanxBone.transform, "Set Idle Pose");
                
                Undo.RecordObject(controller.fingerControllers[3].rootBone.transform, "Set Idle Pose");
                Undo.RecordObject(controller.fingerControllers[3].secondPhalanxBone.transform, "Set Idle Pose");
                Undo.RecordObject(controller.fingerControllers[3].thirdPhalanxBone.transform, "Set Idle Pose");
                
                Undo.RecordObject(controller.fingerControllers[4].rootBone.transform, "Set Idle Pose");
                Undo.RecordObject(controller.fingerControllers[4].secondPhalanxBone.transform, "Set Idle Pose");
                Undo.RecordObject(controller.fingerControllers[4].thirdPhalanxBone.transform, "Set Idle Pose");
                
                controller.current_pose = controller.poseIdle;
                controller.fingers_count = controller.fingerControllers.Length;
                controller.set_fingers_pose();
            }
        }
    }
#endif
}

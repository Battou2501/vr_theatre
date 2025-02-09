using System.Linq;
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

    private InputAction trigger_touched_action;
    private InputAction trigger_value_action;
    private InputAction thumb_touched_action;
    private InputAction grip_value_action;
    
    private bool is_initialized; 
    private bool is_left;
    private int fingers_count;
    private InputActionMap action_map;
    private FingersPoseSO target_pose;
    private FingersPoseSO current_pose;
    private FingersPoseSO grab_override_pose;
    private HandControlSystem hand_control_system;
    
    private float pose_update_speed => hand_control_system == null ? 1 : hand_control_system.poseUpdateSpeed;
    public bool has_grab_override_pose => grab_override_pose != null;

    [Inject]
    public void Construct(HandControlSystem handControlSystem)
    {
        hand_control_system = handControlSystem;
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

        action_map = InputSystem.actions.FindActionMap(is_left ? "Left Controller" : "Right Controller");
        
        if( action_map == null ) return;
        
        thumb_touched_action = action_map.FindAction("Thumb Touched");
        
        if (thumb_touched_action != null)
        {
            thumb_touched_action.performed += update_thumb_touched;
            thumb_touched_action.canceled += update_thumb_touched;
        }
        
        trigger_touched_action = action_map.FindAction("Trigger Touched");

        if (trigger_touched_action != null)
        {
            trigger_touched_action.performed += update_trigger_touched;
            trigger_touched_action.canceled += update_trigger_touched;
        }

        trigger_value_action = action_map.FindAction("Trigger Value");

        if (trigger_value_action != null)
        {
            trigger_value_action.performed += update_trigger_value;
            trigger_value_action.canceled += update_trigger_value;
        }
        
        grip_value_action = action_map.FindAction("Grip Value");

        if (grip_value_action != null)
        {
            grip_value_action.performed += update_grip_value;
            grip_value_action.canceled += update_grip_value;
        }
        
        is_initialized = true;
    }
    
    private void update_thumb_touched(InputAction.CallbackContext context)
    {
        current_thumb_touched = context.ReadValueAsButton();
        update_target_pose();
    }
    
    private void update_trigger_touched(InputAction.CallbackContext context)
    {
        current_trigger_touched = context.ReadValueAsButton();
        update_target_pose();
    }
    
    private void update_trigger_value(InputAction.CallbackContext context)
    {
        current_trigger_value = context.ReadValue<float>();
        update_target_pose();
    }
    
    private void update_grip_value(InputAction.CallbackContext context)
    {
        current_grip_value = context.ReadValue<float>();
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
}

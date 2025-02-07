using System;
using NUnit.Compatibility;
using UnityEngine;
using UnityEngine.InputSystem;
using VrTheatre.Hands;

public class IndexPoseController : FingerPoseController
{
    [SerializeField]
    FingerController indexFingerController;
    [SerializeField]
    IndexPose indexPoseIdle;
    [SerializeField]
    IndexPose indexPosePoint;
    [SerializeField]
    IndexPose indexPoseOK;
    [SerializeField]
    IndexPose indexPoseFist;

    bool current_thumb_touched;
    bool current_grip_pressed;
    bool current_trigger_touched;
    float current_trigger_value;

    InputAction thumb_touched_action;
    InputAction grip_pressed_action;
    InputAction trigger_touched_action;
    InputAction trigger_value_action;

    IndexPose current_pose;
    
    IndexPose target_pose;

    IndexPose grab_override_pose;
    
    public override void init(bool l)
    {
        action_map = InputSystem.actions.FindActionMap( is_left ? "Left Controller" : "Right Controller" );
        
        if( action_map == null ) return;
        
        thumb_touched_action = action_map.FindAction("Thumb Touched");

        if (thumb_touched_action != null)
        {
            thumb_touched_action.performed += update_thumb_touched;
            thumb_touched_action.canceled += update_thumb_touched;
        }

        grip_pressed_action = action_map.FindAction("Grip Pressed");

        if (grip_pressed_action != null)
        {
            grip_pressed_action.performed += update_grip_pressed;
            grip_pressed_action.canceled += update_grip_pressed;
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

        is_left = l;
        is_initialized = true;
    }

    void OnDestroy()
    {
        if(!is_initialized) return;

        if (thumb_touched_action != null)
        {
            thumb_touched_action.performed -= update_thumb_touched;
            thumb_touched_action.canceled -= update_thumb_touched;
        }

        if (grip_pressed_action != null)
        {
            grip_pressed_action.performed -= update_grip_pressed;
            grip_pressed_action.canceled -= update_grip_pressed;
        }

        if (trigger_touched_action != null)
        {
            trigger_touched_action.performed -= update_trigger_touched;
            trigger_touched_action.canceled -= update_trigger_touched;
        }

        if (trigger_value_action != null)
        {
            trigger_value_action.performed -= update_trigger_value;
            trigger_value_action.canceled -= update_trigger_value;
        }
    }

    void update_thumb_touched(InputAction.CallbackContext context)
    {
        current_thumb_touched = context.ReadValueAsButton();
        update_target_pose();
    }
    
    void update_grip_pressed(InputAction.CallbackContext context)
    {
        current_grip_pressed = context.ReadValueAsButton();
        update_target_pose();
    }
    
    void update_trigger_touched(InputAction.CallbackContext context)
    {
        current_trigger_touched = context.ReadValueAsButton();
        update_target_pose();
    }
    
    void update_trigger_value(InputAction.CallbackContext context)
    {
        current_trigger_value = context.ReadValue<float>();
        update_target_pose();
    }

    public void set_grip_override_pose(IndexPose pose)
    {
        grab_override_pose = pose;
    }

    public void remove_grip_override_pose()
    {
        grab_override_pose = null;
    }
    
    public override void update_current_pose()
    {
        if(current_pose.Equals(target_pose)) return;
        
        //current_pose.Lerp(target_pose, Time.deltaTime * poseUpdateSpeed);
        current_pose.move_towards_linear_ease_out(target_pose, Time.deltaTime * poseUpdateSpeed);
        
        indexFingerController.set_pose(current_pose.indexPose);
    }

    protected override void update_target_pose()
    {
        if (grab_override_pose != null && !target_pose.Equals(grab_override_pose))
        {
            target_pose = grab_override_pose;
            return;
        }
        
        target_pose = check_trigger_touched();
    }

    IndexPose check_trigger_touched()
    {
        return !current_trigger_touched ? indexPosePoint : check_grip_pressed(current_trigger_value);
    }

    IndexPose check_grip_pressed(float bend_value)
    {
        if (current_grip_pressed && current_thumb_touched) return indexPoseOK;
        
        return IndexPose.Lerp(indexPoseIdle, indexPoseFist, bend_value);
    }
    
}

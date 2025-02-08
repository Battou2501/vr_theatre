using UnityEngine;
using UnityEngine.InputSystem;

public class ThumbPoseController : FingerPoseController
{
    private bool current_thumb_touched;
    private float current_grip_value;
    private float current_trigger_value;

    private InputAction thumb_touched_action;
    private InputAction grip_value_action;
    private InputAction trigger_value_action;

    public override bool init(bool l)
    {
        base.init(l);

        thumb_touched_action = action_map.FindAction("Thumb Touched");
        
        if (thumb_touched_action != null)
        {
            thumb_touched_action.performed += update_thumb_touched;
            thumb_touched_action.canceled += update_thumb_touched;
        }
        
        grip_value_action = action_map.FindAction("Grip Value");

        if (grip_value_action != null)
        {
            grip_value_action.performed += update_grip_value;
            grip_value_action.canceled += update_grip_value;
        }
        
        trigger_value_action = action_map.FindAction("Trigger Value");

        if (trigger_value_action != null)
        {
            trigger_value_action.performed += update_trigger_value;
            trigger_value_action.canceled += update_trigger_value;
        }
        
        return true;
    }

    protected override void update_target_pose()
    {
        if (!current_thumb_touched)
        {
            target_pose.set_data(poseSpecial);
            return;
        }
        
        FingersPoseSO.Lerp(poseIdle, poseFist, Mathf.Max(current_grip_value, current_trigger_value), target_pose);
    }
    
    private void OnDestroy()
    {
        if(!is_initialized) return;
        
        if (thumb_touched_action != null)
        {
            thumb_touched_action.performed -= update_thumb_touched;
            thumb_touched_action.canceled -= update_thumb_touched;
        }

        if (grip_value_action != null)
        {
            grip_value_action.performed -= update_grip_value;
            grip_value_action.canceled -= update_grip_value;
        }

        if (trigger_value_action != null)
        {
            trigger_value_action.performed -= update_trigger_value;
            trigger_value_action.canceled -= update_trigger_value;
        }
    }

    private void update_thumb_touched(InputAction.CallbackContext context)
    {
        current_thumb_touched = context.ReadValueAsButton();
        update_target_pose();
    }
    
    private void update_grip_value(InputAction.CallbackContext context)
    {
        current_grip_value = context.ReadValue<float>();
        update_target_pose();
    }
    
    private void update_trigger_value(InputAction.CallbackContext context)
    {
        current_trigger_value = context.ReadValue<float>();
        update_target_pose();
    }
}

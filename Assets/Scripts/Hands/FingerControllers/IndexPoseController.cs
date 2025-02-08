using UnityEngine.InputSystem;

public class IndexPoseController : FingerPoseController
{
    //bool current_thumb_touched;
    //bool current_grip_pressed;
    private bool current_trigger_touched;
    private float current_trigger_value;

    //InputAction thumb_touched_action;
    //InputAction grip_pressed_action;
    private InputAction trigger_touched_action;
    private InputAction trigger_value_action;
    
    public override bool init(bool l)
    {
        if(!base.init(l)) return false;
        
        //thumb_touched_action = action_map.FindAction("Thumb Touched");
        //
        //if (thumb_touched_action != null)
        //{
        //    thumb_touched_action.performed += update_thumb_touched;
        //    thumb_touched_action.canceled += update_thumb_touched;
        //}
        //
        //grip_pressed_action = action_map.FindAction("Grip Pressed");
        //
        //if (grip_pressed_action != null)
        //{
        //    grip_pressed_action.performed += update_grip_pressed;
        //    grip_pressed_action.canceled += update_grip_pressed;
        //}

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
        
        return true;
    }

    protected override void update_target_pose()
    {
        if (has_grab_override_pose()) return;
        
        check_trigger_touched();
    }
    
    private void OnDestroy()
    {
        if(!is_initialized) return;

        //if (thumb_touched_action != null)
        //{
        //    thumb_touched_action.performed -= update_thumb_touched;
        //    thumb_touched_action.canceled -= update_thumb_touched;
        //}
        //
        //if (grip_pressed_action != null)
        //{
        //    grip_pressed_action.performed -= update_grip_pressed;
        //    grip_pressed_action.canceled -= update_grip_pressed;
        //}

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

    //void update_thumb_touched(InputAction.CallbackContext context)
    //{
    //    current_thumb_touched = context.ReadValueAsButton();
    //    update_target_pose();
    //}
    //
    //void update_grip_pressed(InputAction.CallbackContext context)
    //{
    //    current_grip_pressed = context.ReadValueAsButton();
    //    update_target_pose();
    //}
    
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

    private void check_trigger_touched()
    {
        if (!current_trigger_touched)
            target_pose.set_data(poseSpecial);
        else
            //check_grip_pressed(current_trigger_value);
            FingersPoseSO.Lerp(poseIdle, poseFist, current_trigger_value, target_pose);
    }

    //void check_grip_pressed(float bend_value)
    //{
    //    //if we are here then Trigger is touched
    //    
    //    if (current_grip_pressed && current_thumb_touched)
    //        FingersPoseSO.Lerp(poseOK, poseFist, bend_value, target_pose);
    //    else
    //        FingersPoseSO.Lerp(poseIdle, poseFist, bend_value, target_pose);
    //}
    
}

using UnityEngine;
using UnityEngine.InputSystem;

public class LowerFingersPoseController : FingerPoseController
{
    [SerializeField]
    FingersPoseSO lowerFingersPoseIdle;
    [SerializeField]
    FingersPoseSO lowerFingersPoseFist;
    
    float current_grip_value;

    InputAction grip_value_action;

    public override bool init(bool l)
    {
        if(!base.init(l)) return false;

        grip_value_action = action_map.FindAction("Grip Value");

        if (grip_value_action != null)
        {
            grip_value_action.performed += update_grip_value;
            grip_value_action.canceled += update_grip_value;
        }
        
        return true;
    }
    
    protected override void update_target_pose()
    {
        FingersPoseSO.Lerp(lowerFingersPoseIdle, lowerFingersPoseFist, current_grip_value, target_pose);
    }

    private void OnDestroy()
    {
        if (!is_initialized) return;
        
        if (grip_value_action != null)
        {
            grip_value_action.performed -= update_grip_value;
            grip_value_action.canceled -= update_grip_value;
        }
    }

    private void update_grip_value(InputAction.CallbackContext context)
    {
        current_grip_value = context.ReadValue<float>();
        update_target_pose();
    }
}

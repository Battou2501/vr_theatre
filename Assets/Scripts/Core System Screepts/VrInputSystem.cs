using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.OpenXR;

public class VrInputSystem : MonoBehaviour
{
    public event Action<bool>  leftThumbTouchedChanged;
    public event Action<bool>  leftTriggerTouchedChanged;
    public event Action<float> leftTriggerValueChanged;
    public event Action<bool>  leftTriggerPressedChanged;
    public event Action<float> leftGripValueChanged;
    public event Action<bool>  leftGripPressedChanged;
    
    public event Action<bool>  rightThumbTouchedChanged;
    public event Action<bool>  rightTriggerTouchedChanged;
    public event Action<float> rightTriggerValueChanged;
    public event Action<bool>  rightTriggerPressedChanged;
    public event Action<float> rightGripValueChanged;
    public event Action<bool>  rightGripPressedChanged;
    
    public InputActionAsset actionAsset;
    
    public InputActionReference leftThumbTouchAction;
    private bool leftThumbTouch;
    public InputActionReference leftTriggerTouchAction;
    private bool leftTriggerTouch;
    public InputActionReference leftTriggerValueAction;
    private float leftTriggerValue;
    public InputActionReference leftTriggerPressedAction;
    private bool leftTriggerPressed;
    public InputActionReference leftGripValueAction;
    private float leftGripValue;
    public InputActionReference leftGripPressedAction;
    private bool leftGripPressed;
    public InputActionReference rightThumbTouchAction;
    private bool rightThumbTouch;
    public InputActionReference rightTriggerTouchAction;
    private bool rightTriggerTouch;
    public InputActionReference rightTriggerValueAction;
    private float rightTriggerValue;
    public InputActionReference rightTriggerPressedAction;
    private bool rightTriggerPressed;
    public InputActionReference rightGripValueAction;
    private float rightGripValue;
    public InputActionReference rightGripPressedAction;
    private bool rightGripPressed;
    
    void Start()
    {
        actionAsset?.Enable();
        
        if (leftThumbTouchAction != null)
        {
            leftThumbTouchAction.action.performed += update_left_thumb_touched;
            leftThumbTouchAction.action.canceled += update_left_thumb_touched;
        }
        
        if (leftTriggerTouchAction != null)
        {
            leftTriggerTouchAction.action.performed += update_left_trigger_touched;
            leftTriggerTouchAction.action.canceled += update_left_trigger_touched;
        }

        if (leftTriggerValueAction != null)
        {
            leftTriggerValueAction.action.performed += update_left_trigger_value;
            leftTriggerValueAction.action.canceled += update_left_trigger_value;
        }
        
        if (leftTriggerPressedAction != null)
        {
            leftTriggerPressedAction.action.performed += update_left_trigger_pressed;
            leftTriggerPressedAction.action.canceled += update_left_trigger_pressed;
        }
        
        if (leftGripValueAction != null)
        {
            leftGripValueAction.action.performed += update_left_grip_value;
            leftGripValueAction.action.canceled += update_left_grip_value;
        }
        
        if (leftGripPressedAction != null)
        {
            leftGripPressedAction.action.performed += update_left_grip_pressed;
            leftGripPressedAction.action.canceled += update_left_grip_pressed;
        }
        
        
        
        if (rightThumbTouchAction != null)
        {
            rightThumbTouchAction.action.performed += update_right_thumb_touched;
            rightThumbTouchAction.action.canceled += update_right_thumb_touched;
        }
        
        if (rightTriggerTouchAction != null)
        {
            rightTriggerTouchAction.action.performed += update_right_trigger_touched;
            rightTriggerTouchAction.action.canceled += update_right_trigger_touched;
        }

        if (rightTriggerValueAction != null)
        {
            rightTriggerValueAction.action.performed += update_right_trigger_value;
            rightTriggerValueAction.action.canceled += update_right_trigger_value;
        }
        
        if (rightTriggerPressedAction != null)
        {
            rightTriggerPressedAction.action.performed += update_right_trigger_pressed;
            rightTriggerPressedAction.action.canceled += update_right_trigger_pressed;
        }
        
        if (rightGripValueAction != null)
        {
            rightGripValueAction.action.performed += update_right_grip_value;
            rightGripValueAction.action.canceled += update_right_grip_value;
        }
        
        if (rightGripPressedAction != null)
        {
            rightGripPressedAction.action.performed += update_right_grip_pressed;
            rightGripPressedAction.action.canceled += update_right_grip_pressed;
        }
    }

    private void update_left_grip_pressed(InputAction.CallbackContext context)
    {
        leftGripPressed = context.ReadValueAsButton();
        leftGripPressedChanged?.Invoke(leftGripPressed);
    }

    private void update_left_grip_value(InputAction.CallbackContext context)
    {
        leftGripValue = context.ReadValue<float>();
        leftGripValueChanged?.Invoke(leftGripValue);
    }

    private void update_left_trigger_pressed(InputAction.CallbackContext context)
    {
        leftTriggerPressed = context.ReadValueAsButton();
        leftTriggerPressedChanged?.Invoke(leftTriggerPressed);
    }
    
    private void update_left_trigger_value(InputAction.CallbackContext context)
    {
        leftTriggerValue = context.ReadValue<float>();
        leftTriggerValueChanged?.Invoke(leftTriggerValue);
    }

    private void update_left_trigger_touched(InputAction.CallbackContext context)
    {
        leftTriggerTouch = context.ReadValueAsButton();
        leftTriggerTouchedChanged?.Invoke(leftTriggerTouch);
    }

    private void update_left_thumb_touched(InputAction.CallbackContext context)
    {
        leftThumbTouch = context.ReadValueAsButton();
        leftThumbTouchedChanged?.Invoke(leftThumbTouch);
    }
    
    
    
    private void update_right_grip_pressed(InputAction.CallbackContext context)
    {
        rightGripPressed = context.ReadValueAsButton();
        rightGripPressedChanged?.Invoke(rightGripPressed);
    }

    private void update_right_grip_value(InputAction.CallbackContext context)
    {
        rightGripValue = context.ReadValue<float>();
        rightGripValueChanged?.Invoke(rightGripValue);
    }

    private void update_right_trigger_pressed(InputAction.CallbackContext context)
    {
        rightTriggerPressed = context.ReadValueAsButton();
        rightTriggerPressedChanged?.Invoke(rightTriggerPressed);
    }
    
    private void update_right_trigger_value(InputAction.CallbackContext context)
    {
        rightTriggerValue = context.ReadValue<float>();
        rightTriggerValueChanged?.Invoke(rightTriggerValue);
    }

    private void update_right_trigger_touched(InputAction.CallbackContext context)
    {
        rightTriggerTouch = context.ReadValueAsButton();
        rightTriggerTouchedChanged?.Invoke(rightTriggerTouch);
    }

    private void update_right_thumb_touched(InputAction.CallbackContext context)
    {
        rightThumbTouch = context.ReadValueAsButton();
        rightThumbTouchedChanged?.Invoke(rightThumbTouch);
    }
    
    
}

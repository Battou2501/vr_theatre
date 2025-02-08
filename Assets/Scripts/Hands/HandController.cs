using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandController : MonoBehaviour
{
    public Action<HandController> triggerPressed;
    public Action<HandController> triggerReleased;
    public Action<HandController> gripPressed;
    public Action<HandController> gripReleased;
    
    [SerializeField] 
    private HandControlSystem.Handedness hand;
    [SerializeField] 
    private HandPoseController poseController;
    [SerializeField] 
    public Transform grabPoint;
    [SerializeField] 
    public Collider grabCollider;

    InputAction trigger_pressed_action;
    InputAction grip_pressed_action;

    public void init()
    {
        if(poseController == null) return;
        
        poseController.init(hand);
        
        var is_left = hand == HandControlSystem.Handedness.Left;
        
        var action_map = InputSystem.actions.FindActionMap(is_left ? "Left Controller" : "Right Controller");
        
        if( action_map == null ) return;
        
        trigger_pressed_action = action_map.FindAction("Trigger Pressed");

        if (trigger_pressed_action != null)
        {
            trigger_pressed_action.performed += OnTriggerPressed;
            trigger_pressed_action.canceled += OnTriggerReleased;
        }
        
        grip_pressed_action = action_map.FindAction("Grip Pressed");

        if (grip_pressed_action != null)
        {
            grip_pressed_action.performed += OnGripPressed;
            grip_pressed_action.canceled += OnGripReleased;
        }
    }

    public void set_grab_pose(FingersPoseSO pose)
    {
        poseController.set_grab_override_pose(pose);
        grabCollider.enabled = false;
    }

    public void remove_grab_pose()
    {
        poseController.remove_grab_override_pose();
        grabCollider.enabled = true;
    }
    
    void OnTriggerPressed(InputAction.CallbackContext obj)
    {
        triggerPressed?.Invoke(this);
    }
    
    void OnTriggerReleased(InputAction.CallbackContext obj)
    {
        triggerReleased?.Invoke(this);
    }
    
    void OnGripPressed(InputAction.CallbackContext obj)
    {
        gripPressed?.Invoke(this);
    }
    
    void OnGripReleased(InputAction.CallbackContext obj)
    {
        gripReleased?.Invoke(this);
    }
    
    
}

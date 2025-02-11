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

    public bool is_grabbing => poseController.has_grab_override_pose;
    public Vector3 get_move_vector { get; private set; }


    InputAction trigger_pressed_action;
    InputAction grip_pressed_action;

    private Vector3 position_previous_frame;
    
    private Transform this_transform;
    
    private float fixed_delta_time_multiplier;
    
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

        position_previous_frame = this_transform.position;
        
        fixed_delta_time_multiplier = 1f / Time.fixedDeltaTime;
    }

    private void FixedUpdate()
    {
        get_move_vector = (this_transform.position - position_previous_frame) * fixed_delta_time_multiplier;
        position_previous_frame = this_transform.position;
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

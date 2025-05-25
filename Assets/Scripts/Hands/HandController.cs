using System;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class HandController : MonoBehaviour
{
    public Func<HandController, bool> triggerPressed;
    public Func<HandController, bool> triggerReleased;
    public Func<HandController, bool> gripPressed;
    public Func<HandController, bool> gripReleased;
    
    [SerializeField] 
    private HandControlSystem.Handedness hand;
    [SerializeField] 
    private HandPoseController poseController;
    [SerializeField] 
    public Transform grabPoint;
    [SerializeField] 
    public Rigidbody grabBody;

    [SerializeField] private BaseHandPointer pointer;
    public HandControlSystem.Handedness Handedness => hand;
    public bool is_grabbing => poseController.has_grab_override_pose;
    
    private Transform this_transform;

    private VrInputSystem _vrInputSystem;
    
    private bool _is_initialized;

    private bool _is_left;
    
    private HashSet<GrabbableObject> _triggerGrabbableObjectsHovered = new HashSet<GrabbableObject>();
    private MainControls _mainControls;
    
    [Inject]
    private void Construct(VrInputSystem vrInputSystem, MainControls mainControls)
    {
        _vrInputSystem = vrInputSystem;
        _mainControls = mainControls;
    }
    
    public void init()
    {
        if(poseController == null) return;
        
        poseController.init(hand);
        
        this_transform = transform;
        
        _is_left = hand == HandControlSystem.Handedness.Left;

        if (_is_left)
        {
            _vrInputSystem.leftTriggerPressedChanged += OnTriggerPressedChanged;
            _vrInputSystem.leftGripPressedChanged += OnGripPressedChanged;
        }
        else
        {
            _vrInputSystem.rightTriggerPressedChanged += OnTriggerPressedChanged;
            _vrInputSystem.rightGripPressedChanged += OnGripPressedChanged;
        }
    }

    private void OnDestroy()
    {
        if (!_is_initialized) return;
        
        if (_is_left)
        {
            _vrInputSystem.leftTriggerPressedChanged -= OnTriggerPressedChanged;
            _vrInputSystem.leftGripPressedChanged -= OnGripPressedChanged;
        }
        else
        {
            _vrInputSystem.rightTriggerPressedChanged -= OnTriggerPressedChanged;
            _vrInputSystem.rightGripPressedChanged -= OnGripPressedChanged;
        }
    }

    public void set_grab_pose(FingersPoseSO pose)
    {
        pointer?.SetActive(false);
        poseController.set_grab_override_pose(pose);
        grabBody.detectCollisions = false;
    }

    public void remove_grab_pose()
    {
        pointer?.SetActive(_mainControls.IsUiOpened);
        poseController.remove_grab_override_pose();
        grabBody.detectCollisions = true;
    }
    
    void OnTriggerPressedChanged(bool state)
    {
        if(state)
            triggerPressed?.Invoke(this);
        else
            triggerReleased?.Invoke(this);
    }
    
    void OnGripPressedChanged(bool state)
    {
        if(state)
            gripPressed?.Invoke(this);
        else
            gripReleased?.Invoke(this);
    }
    
    
}

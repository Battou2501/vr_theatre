using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

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
    public Rigidbody grabBody;

    public bool is_grabbing => poseController.has_grab_override_pose;
    public Vector3 get_move_vector { get; private set; }

    private Vector3 position_previous_frame;
    
    private Transform this_transform;
    
    private float fixed_delta_time_multiplier;

    private VrInputSystem _vrInputSystem;
    
    private bool _is_initialized;

    private bool _is_left;
    
    [Inject]
    private void Construct(VrInputSystem vrInputSystem)
    {
        _vrInputSystem = vrInputSystem;
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

        position_previous_frame = this_transform.position;
        
        fixed_delta_time_multiplier = 1f / Time.fixedDeltaTime;
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

    private void FixedUpdate()
    {
        if(this_transform == null) return;
        
        get_move_vector = (this_transform.position - position_previous_frame) * fixed_delta_time_multiplier;
        position_previous_frame = this_transform.position;
    }

    public void set_grab_pose(FingersPoseSO pose)
    {
        poseController.set_grab_override_pose(pose);
        grabBody.detectCollisions = false;
    }

    public void remove_grab_pose()
    {
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

using System;
using System.Collections.Generic;
using DefaultNamespace;
using Hands;
using UnityEngine;
using Zenject;

public abstract class BaseControl : MonoBehaviour
{
    //protected MainControls main_controls;
    //protected ManageVideoPlayerAudio video_manager;
    //protected FileNavigationManager file_navigation_manager;
    //protected UIManager ui_manager;

    protected ControlsAnimationFeedback animation_feedback;
    
    protected bool is_initiated;

    public bool isHovered { get; private set; } // => hovered_by.Count > 0;
    protected HashSet<GameObject> hovered_by => hovered_this_frame;

    protected Transform this_transform;
    
    protected GameObject _leftHandTriggerCollider;
    protected GameObject _rightHandTriggerCollider;
    
    private HashSet<GameObject> hovered_this_frame = new HashSet<GameObject>();
    private HashSet<GameObject> hovered_last_frame = new HashSet<GameObject>();
    
    //protected DiContainer container;
    
    [Inject]
    public void Construct(LeftHandPointer leftHandTriggerCollider, RightHandPointer rightHandTriggerCollider)
    {
        _leftHandTriggerCollider = leftHandTriggerCollider.gameObject;
        _rightHandTriggerCollider = rightHandTriggerCollider.gameObject;
    }
    
    public virtual void init()
    {
        animation_feedback = GetComponent<ControlsAnimationFeedback>();
        //hovered_by = new HashSet<GameObject>();

        //var parent_panel = gameObject.GetComponentInParent<BaseControlsPanel>(true);
        //if (parent_panel != null)
        //    parent_panel.Closed += OnParentPanelClosed;
        this_transform = transform;
        is_initiated = true;
    }

    //void OnParentPanelClosed()
    //{
    //    hovered_by.Clear();
    //}

    void OnDisable()
    {
        hovered_by?.Clear();
        animation_feedback?.reset();
        hovered_this_frame = new HashSet<GameObject>();
        hovered_last_frame = new HashSet<GameObject>();
        isHovered = false;
    }

    void TriggerEnter()
    {
        if(isHovered) return;
        
        isHovered = true;
        
        animation_feedback.real_null()?.OnHoverStart();
    }

    void TriggerExit()
    {
        if(!isHovered) return;
        
        if(hovered_this_frame.Count > 0) return;
        
        isHovered = false;
        
        animation_feedback.real_null()?.OnHoverEnd();
    }
    
    private void FixedUpdate()
    {
        CheckEnter();
        CheckExit();
        
        (hovered_this_frame, hovered_last_frame) = (hovered_last_frame, hovered_this_frame);
        
        hovered_this_frame.Clear();
    }
    
    private void OnTriggerStay(Collider other)
    {
        hovered_this_frame.Add(other.gameObject);
    }

    void CheckEnter()
    {
        foreach (var collider1 in hovered_this_frame)
        {
            if (!hovered_last_frame.Contains(collider1))
            {
                if(collider1 != _leftHandTriggerCollider && collider1 != _rightHandTriggerCollider ) return;
                
                TriggerEnter();
            }
        }
    }
    
    void CheckExit()
    {
        foreach (var collider1 in hovered_last_frame)
        {
            if (!hovered_this_frame.Contains(collider1))
            {
                TriggerExit();
            }
        }
    }
}

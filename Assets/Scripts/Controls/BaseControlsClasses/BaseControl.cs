using System;
using System.Collections.Generic;
using DefaultNamespace;
using Hands;
using UnityEngine;
using Zenject;

public abstract class BaseControl : MonoBehaviour
{
    protected MainControls main_controls;
    //protected ManageVideoPlayerAudio video_manager;
    //protected FileNavigationManager file_navigation_manager;
    //protected UIManager ui_manager;

    protected ControlsAnimationFeedback animation_feedback;
    
    protected bool is_initiated;

    private bool is_hovered => hovered_by.Count > 0;
    protected HashSet<GameObject> hovered_by;

    protected Transform this_transform;
    
    protected GameObject _leftHandTriggerCollider;
    protected GameObject _rightHandTriggerCollider;
    
    //protected DiContainer container;
    
    [Inject]
    public void Construct(MainControls m, LeftHandPointer leftHandTriggerCollider, RightHandPointer rightHandTriggerCollider)
    {
        main_controls = m;
        _leftHandTriggerCollider = leftHandTriggerCollider.gameObject;
        _rightHandTriggerCollider = rightHandTriggerCollider.gameObject;
    }
    
    public virtual void init()
    {
        animation_feedback = GetComponent<ControlsAnimationFeedback>();
        hovered_by = new HashSet<GameObject>();

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
    }

    void OnTriggerEnter(Collider other)
    {
        if(is_hovered) return;
        
        if(other.gameObject != _leftHandTriggerCollider && other.gameObject != _rightHandTriggerCollider ) return;
        
        hovered_by.Add(other.gameObject);
        
        if(hovered_by.Count > 1) return;
        
        animation_feedback.real_null()?.OnHoverStart();
    }

    void OnTriggerExit(Collider other)
    {
        if(!is_hovered || !hovered_by.Contains(other.gameObject)) return;
        
        hovered_by.Remove(other.gameObject);
        
        if(hovered_by.Count > 0) return;
        
        animation_feedback.real_null()?.OnHoverEnd();
    }
}

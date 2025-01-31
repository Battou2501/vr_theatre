using System;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public abstract class BaseControl : MonoBehaviour
{
    protected MainControls main_controls;
    protected ManageVideoPlayerAudio video_manager;
    protected FileNavigationManager file_navigation_manager;

    protected ControlsAnimationFeedback animation_feedback;
    
    protected bool is_initiated;

    protected bool is_hovered => hovered_by.Count > 0;
    protected HashSet<GameObject> hovered_by;
    
    public virtual void init(MainControls m)
    {
        main_controls = m;
        video_manager = main_controls.videoManager;
        file_navigation_manager = main_controls.fileNavigationManager;
        animation_feedback = GetComponent<ControlsAnimationFeedback>();
        hovered_by = new HashSet<GameObject>();
        is_initiated = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if(is_hovered) return;
        
        if(other.gameObject != main_controls.leftHandTriggerCollider && other.gameObject != main_controls.rightHandTriggerCollider ) return;
        
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

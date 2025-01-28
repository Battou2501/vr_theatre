using System;
using DefaultNamespace;
using UnityEngine;

public abstract class BaseControl : MonoBehaviour
{
    protected MainControls main_controls;
    protected ManageVideoPlayerAudio video_manager;

    protected ControlsAnimationFeedback animation_feedback;
    
    protected bool is_initiated;

    protected bool is_hovered => hovered_by != null;
    protected GameObject hovered_by;
    
    public virtual void init(MainControls m)
    {
        main_controls = m;
        video_manager = main_controls.videoManager;
        animation_feedback = GetComponent<ControlsAnimationFeedback>();
        is_initiated = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if(is_hovered) return;
        
        if(!other.CompareTag("Controller Ray")) return;
        
        hovered_by = other.gameObject;
        
        animation_feedback.real_null()?.OnHoverStart();
    }

    void OnTriggerExit(Collider other)
    {
        if(!is_hovered || other.gameObject != hovered_by) return;
        
        hovered_by = null;
        
        animation_feedback.real_null()?.OnHoverEnd();
    }
}

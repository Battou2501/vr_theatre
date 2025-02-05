using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(BaseControl))]
public class ControlsAnimationFeedback : MonoBehaviour
{
    [SerializeField]
    Ease easeType = Ease.OutBack;
    [SerializeField]
    float duration = 0.5f;
    [SerializeField] 
    float overshoot = 0.5f;
    [SerializeField] 
    bool bounceOnClick;
    Vector3 initial_scale;
    Vector3 increased_scale;
    bool is_hovered;
    bool is_initial_scale_set;
    
    //void OnEnable()
    //{
    //    set_initial_scale();
    //    reset();
    //}

    void set_initial_scale()
    {
        if(is_initial_scale_set) return;
        
        initial_scale = transform.localScale;
        increased_scale = initial_scale * 1.1f;
        
        is_initial_scale_set = true;
    }
    
    public void reset()
    {
        if(!is_initial_scale_set) return;
        
        transform.localScale = initial_scale;
    }
    
    public void OnTriggerPressed()
    {
        if(!is_hovered || !bounceOnClick) return;

        transform.DOPunchScale(initial_scale, 0.3f);
    }
    
    public void OnHoverStart()
    {
        set_initial_scale();
        
        transform.DOScale(increased_scale, duration).SetEase(easeType, overshoot);
    }

    public void OnHoverEnd()
    {
        transform.DOScale(initial_scale, duration).SetEase(easeType, overshoot);
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GrabbableObject : HoverableObjectBase
{
    
    [SerializeField]
    private FingersPoseSO grabPose;
    [SerializeField]
    private Transform grab_point;
    
    private HandController grabbed_by_hand_controller;


    protected override void OnTriggerEnter(Collider other)
    {
        if (grabbed_by_hand_controller != null) return;
        
        base.OnTriggerEnter(other);
    }


    protected override void OnTriggerExit(Collider other)
    {
        if(grabbed_by_hand_controller != null) return;

        base.OnTriggerExit(other);
    }

    protected override void OnGrabbed(HandController hand_controller)
    {
        grabbed_by_hand_controller = hand_controller;
        
        grabbed_by_hand_controller.set_grab_pose(grabPose);
        
        switch (grabbedWith)
        {
            case GrabbedWith.Trigger:
                hand_controller.triggerPressed -= OnGrabbed;
                hand_controller.triggerPressed += OnReleased;
                break;
            case GrabbedWith.Grip:
                hand_controller.gripPressed -= OnGrabbed;
                hand_controller.gripPressed += OnReleased;
                break;
        }
        
        hovered_by_hand_collider_dict.Clear();
        
        
        transform.SetParent(grabbed_by_hand_controller.grabPoint);
        transform.localRotation = Quaternion.Inverse(grab_point.localRotation);
        transform.localPosition = Vector3.zero;
        transform.position += transform.position - grab_point.position;

    }

    protected override void OnReleased(HandController hand_controller)
    {
        transform.SetParent(null);
        grabbed_by_hand_controller = null;
        hand_controller.remove_grab_pose();
    }

    public void release()
    {
        if(grabbed_by_hand_controller == null) return;
        
        OnReleased(grabbed_by_hand_controller);
    }
}

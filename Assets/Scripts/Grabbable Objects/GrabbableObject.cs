using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GrabbableObject : HoverableObjectBase
{
    protected enum GrabbedWith
    {
        Trigger = 0,
        Grip
    }

    [SerializeField]
    protected GrabbedWith grabbedWith;
    [SerializeField]
    private FingersPoseSO grabPose;
    [SerializeField]
    private Transform grabPointLeft;
    [SerializeField]
    private Transform grabPointRight;
    
    private HandController grabbed_by_hand_controller;

    private Dictionary<GameObject, HandController> hovered_by_hand_object_dict;

    public bool IsGrabbed => grabbed_by_hand_controller != null;
    
    public override void init()
    {
        base.init();
        hovered_by_hand_object_dict = new Dictionary<GameObject, HandController>();
    }

    protected override void OnDisable()
    {
        UnsubscribeAll();
        hovered_by_hand_object_dict?.Clear();
    }

    private void OnDestroy()
    {
        UnsubscribeAll();
    }

    private void UnsubscribeAll()
    {
        if (IsHovered && !IsGrabbed && hovered_by_hand_object_dict != null)
        {
            foreach (var (obj, controller) in hovered_by_hand_object_dict)
            {
                switch (grabbedWith)
                {
                    case GrabbedWith.Trigger:
                        controller.triggerPressed -= OnGrabbed;
                        break;
                    case GrabbedWith.Grip:
                        controller.gripPressed -= OnGrabbed;
                        break;
                }
            }
        }

        if (!IsGrabbed || grabbed_by_hand_controller == null) return;
        
        OnReleased(grabbed_by_hand_controller);
    }

    protected override void TriggerEnter(Collider other)
    {
        //if (IsGrabbed) return;

        if(other.attachedRigidbody == null) return;
        
        if(!other.attachedRigidbody.gameObject.CompareTag(hand_tag_handle)) return;
        
        base.TriggerEnter(other);
        
        var body_object = other.attachedRigidbody.gameObject;
        
        if(hovered_by_hand_object_dict.ContainsKey(body_object)) return;

        var hand_controller = other.GetComponentInParent<HandController>();
        
        if(hand_controller == null) return;
        
        hovered_by_hand_object_dict.Add(body_object, hand_controller);
        
        switch (grabbedWith)
        {
            case GrabbedWith.Trigger:
                hand_controller.triggerPressed += OnGrabbed;
                break;
            case GrabbedWith.Grip:
                hand_controller.gripPressed += OnGrabbed;
                break;
        }
        
        //Debug.Log("Entered");
    }


    protected override void TriggerExit(Collider other)
    {
        //if(IsGrabbed) return;
        
        if(other.attachedRigidbody == null) return;
        if(!other.attachedRigidbody.gameObject.CompareTag(hand_tag_handle)) return;
        
        base.TriggerExit(other);
        
        var body_object = other.attachedRigidbody.gameObject;
        
        if(!hovered_by_hand_object_dict.ContainsKey(body_object)) return;
        
        //if(hovered_by_hand_collider_dict.ContainsKey(body_object)) return;
        
        switch (grabbedWith)
        {
            case GrabbedWith.Trigger:
                hovered_by_hand_object_dict[body_object].triggerPressed -= OnGrabbed;
                break;
            case GrabbedWith.Grip:
                hovered_by_hand_object_dict[body_object].gripPressed -= OnGrabbed;
                break;
        }
        
        hovered_by_hand_object_dict.Remove(body_object);
    }

    public virtual bool OnGrabbed(HandController hand_controller)
    {
        if(IsGrabbed || hand_controller.is_grabbing) return false;
        
        //Debug.Log("Grabbed");
        
        //foreach (var (obj, controller) in hovered_by_hand_object_dict)
        //{
        //    switch (grabbedWith)
        //    {
        //        case GrabbedWith.Trigger:
        //            controller.triggerPressed -= OnGrabbed;
        //            break;
        //        case GrabbedWith.Grip:
        //            controller.gripPressed -= OnGrabbed;
        //            break;
        //    }
        //}

        //hovered_by_hand_object_dict.Clear();
        
        //hovered_by_hand_collider_dict.Clear();
        
        grabbed_by_hand_controller = hand_controller;
        
        grabbed_by_hand_controller.set_grab_pose(grabPose);
        
        switch (grabbedWith)
        {
            case GrabbedWith.Trigger:
                hand_controller.triggerReleased += OnReleased;
                break;
            case GrabbedWith.Grip:
                hand_controller.gripReleased += OnReleased;
                break;
        }

        transform.SetParent(hand_controller.grabPoint);
        
        align();
        
        gameObject.SetActive(true);
        
        return true;
    }

    private void align()
    {
        var grabPoint = grabbed_by_hand_controller.Handedness == HandControlSystem.Handedness.Left ? grabPointLeft : grabPointRight;
        
        transform.localRotation = Quaternion.Inverse(grabPoint.localRotation);
        if (transform.localScale.x < 0)
        {
            var r = transform.localRotation.eulerAngles;
            transform.localRotation = Quaternion.Euler(r.x, -r.y, -r.z);
        }
        transform.localPosition = Vector3.zero;
        transform.position += transform.position - grabPoint.position;
    }
    
    protected virtual bool OnReleased(HandController hand_controller)
    {
        //Debug.Log("Released 1");
        
        if(!IsGrabbed || grabbed_by_hand_controller != hand_controller) return false;
        
        switch (grabbedWith)
        {
            case GrabbedWith.Trigger:
                hand_controller.triggerReleased -= OnReleased;
                break;
            case GrabbedWith.Grip:
                hand_controller.gripReleased -= OnReleased;
                break;
        }
        
        transform.SetParent(null);
        grabbed_by_hand_controller = null;
        hand_controller.remove_grab_pose();
        //Debug.Log("Released 2");
        return true;
    }

    public void release()
    {
        if(grabbed_by_hand_controller == null) return;
        
        OnReleased(grabbed_by_hand_controller);
    }
}

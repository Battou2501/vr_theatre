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
    
    protected Dictionary<GameObject, HandController> hovered_by_hand_object_dict;

    public bool IsGRabbed => grabbed_by_hand_controller != null;
    
    public override void init()
    {
        base.init();
        hovered_by_hand_object_dict = new Dictionary<GameObject, HandController>();
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (grabbed_by_hand_controller != null) return;
        
        base.OnTriggerEnter(other);

        if(other.attachedRigidbody == null) return;
        
        if(!other.attachedRigidbody.gameObject.CompareTag(hand_tag_handle)) return;
        
        var body = other.attachedRigidbody.gameObject;
        
        if(hovered_by_hand_object_dict.ContainsKey(body)) return;

        var hand_controller = other.GetComponentInParent<HandController>();
        
        if(hand_controller == null) return;
        
        hovered_by_hand_object_dict.Add(body, hand_controller);
        
        switch (grabbedWith)
        {
            case GrabbedWith.Trigger:
                hand_controller.triggerPressed += OnGrabbed;
                break;
            case GrabbedWith.Grip:
                hand_controller.gripPressed += OnGrabbed;
                break;
        }
        
        Debug.Log("Entered");
    }


    protected override void OnTriggerExit(Collider other)
    {
        if(grabbed_by_hand_controller != null) return;

        base.OnTriggerExit(other);
        
        if(other.attachedRigidbody == null) return;
        
        //if(!other.attachedRigidbody.gameObject.CompareTag(hand_tag_handle)) return;
        
        var body = other.attachedRigidbody.gameObject;
        
        //if(hovered_by_hand_object_dict.ContainsKey(body)) return;
        
        if(!hovered_by_hand_object_dict.ContainsKey(body)) return;
        
        switch (grabbedWith)
        {
            case GrabbedWith.Trigger:
                hovered_by_hand_object_dict[body].triggerPressed -= OnGrabbed;
                break;
            case GrabbedWith.Grip:
                hovered_by_hand_object_dict[body].gripPressed -= OnGrabbed;
                break;
        }
        
        hovered_by_hand_object_dict.Remove(body);
        
        Debug.Log("Exited");
    }

    public virtual void OnGrabbed(HandController hand_controller)
    {
        if(hand_controller.is_grabbing) return;
        
        gameObject.SetActive(true);
        
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

        hovered_by_hand_object_dict.Clear();
        
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
        
        hovered_by_hand_collider_dict.Clear();

        transform.SetParent(hand_controller.grabPoint);
        
        align();
    }

    private void align()
    {
        transform.localRotation = Quaternion.Inverse(grab_point.localRotation);
        if (transform.localScale.x < 0)
        {
            var r = transform.localRotation.eulerAngles;
            transform.localRotation = Quaternion.Euler(r.x, -r.y, -r.z);
        }
        transform.localPosition = Vector3.zero;
        transform.position += transform.position - grab_point.position;
    }
    
    protected virtual void OnReleased(HandController hand_controller)
    {
        Debug.Log("Released");
        
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
    }

    public void release()
    {
        if(grabbed_by_hand_controller == null) return;
        
        OnReleased(grabbed_by_hand_controller);
    }
}

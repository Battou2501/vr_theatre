using System;
using System.Collections.Generic;
using UnityEngine;

public class GrabbableObject : MonoBehaviour
{
    public enum GrabbedWith
    {
        Trigger = 0,
        Grip
    }
    
    [SerializeField]
    private GrabbedWith grabbedWith;
    [SerializeField]
    private FingersPoseSO grabPose;
    [SerializeField]
    private Transform grab_point;
    
    private HandController grabbed_by_hand_controller;

    private TagHandle hand_tag_handle;
    
    private Dictionary<HandController, List<Collider>> hovered_by_hand_collider_dict;

    public void init()
    {
        hovered_by_hand_collider_dict = new Dictionary<HandController, List<Collider>>();

        hand_tag_handle = TagHandle.GetExistingTag("Hand");
    }

    void OnTriggerEnter(Collider other)
    {
        if(grabbed_by_hand_controller != null) return;
        
        if(!other.CompareTag(hand_tag_handle)) return;
        
        var hand_controller = other.GetComponentInParent<HandController>();
        
        if(hand_controller == null) return;
        
        if(!hovered_by_hand_collider_dict.ContainsKey(hand_controller))
            hovered_by_hand_collider_dict[hand_controller] = new List<Collider>();
        
        if(!hovered_by_hand_collider_dict[hand_controller].Contains(other))
            hovered_by_hand_collider_dict[hand_controller].Add(other);

        if(hovered_by_hand_collider_dict[hand_controller].Count != 1) return;
        
        switch (grabbedWith)
        {
            case GrabbedWith.Trigger:
                hand_controller.triggerPressed += OnGrabbed;
                break;
            case GrabbedWith.Grip:
                hand_controller.gripPressed += OnGrabbed;
                break;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(grabbed_by_hand_controller != null) return;

        if (!other.CompareTag(hand_tag_handle)) return;
            
        var hand_controller = other.GetComponentInParent<HandController>();
        
        if(hand_controller == null) return;
        
        if(!hovered_by_hand_collider_dict.ContainsKey(hand_controller)) return;
        
        hovered_by_hand_collider_dict[hand_controller].Remove(other);
        
        if(hovered_by_hand_collider_dict[hand_controller].Count > 0) return;
        
        hovered_by_hand_collider_dict.Remove(hand_controller);
        
        switch (grabbedWith)
        {
            case GrabbedWith.Trigger:
                hand_controller.triggerPressed -= OnGrabbed;
                break;
            case GrabbedWith.Grip:
                hand_controller.gripPressed -= OnGrabbed;
                break;
        }
    }

    void OnGrabbed(HandController hand_controller)
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

    void OnReleased(HandController hand_controller)
    {
        transform.SetParent(null);
        grabbed_by_hand_controller = null;
        hand_controller.remove_grab_pose();
    }
}

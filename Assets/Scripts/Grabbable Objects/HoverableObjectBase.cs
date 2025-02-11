using System.Collections.Generic;
using UnityEngine;


public class HoverableObjectBase : MonoBehaviour, IInitable
{
    protected enum GrabbedWith
    {
        Trigger = 0,
        Grip
    }

    [SerializeField]
    protected GrabbedWith grabbedWith;
    
    private TagHandle hand_tag_handle;

    protected Dictionary<HandController, List<Collider>> hovered_by_hand_collider_dict;
    
    public virtual void init()
    {
        hovered_by_hand_collider_dict = new Dictionary<HandController, List<Collider>>();

        hand_tag_handle = TagHandle.GetExistingTag("Hand");
    }
    
    protected virtual void OnTriggerEnter(Collider other)
    {
        if(!other.CompareTag(hand_tag_handle)) return;
    
        var hand_controller = other.GetComponentInParent<HandController>();
    
        if(hand_controller == null) return;
    
        if(hand_controller.is_grabbing) return;
    
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
    
    protected virtual void OnTriggerExit(Collider other)
    {
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
    
    protected virtual void OnGrabbed(HandController hand_controller)
    {
    }

    protected virtual void OnReleased(HandController hand_controller)
    {
    }
    
}
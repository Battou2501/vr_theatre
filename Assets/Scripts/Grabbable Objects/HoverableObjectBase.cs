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
    
    protected TagHandle hand_tag_handle;

    protected Dictionary<GameObject, List<Collider>> hovered_by_hand_collider_dict;
    
    public bool IsHovered => hovered_by_hand_collider_dict is {Count: > 0};
    
    public virtual void init()
    {
        hovered_by_hand_collider_dict = new Dictionary<GameObject, List<Collider>>();

        hand_tag_handle = TagHandle.GetExistingTag("Hand Grab Sphere");
    }
    
    protected virtual void OnTriggerEnter(Collider other)
    {
        if(other.attachedRigidbody == null) return;
        
        if(!other.attachedRigidbody.gameObject.CompareTag(hand_tag_handle)) return;
    
        //var hand_controller = other.GetComponentInParent<HandController>();
    
        //if(hand_controller == null) return;
    
        //if(hand_controller.is_grabbing) return;

        var body_object = other.attachedRigidbody.gameObject;
        
        if(!hovered_by_hand_collider_dict.ContainsKey(body_object))
            hovered_by_hand_collider_dict[body_object] = new List<Collider>();
    
        if(!hovered_by_hand_collider_dict[body_object].Contains(other))
            hovered_by_hand_collider_dict[body_object].Add(other);
    }
    
    protected virtual void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(hand_tag_handle)) return;
        
        //var hand_controller = other.GetComponentInParent<HandController>();
    
        //if(hand_controller == null) return;
    
        var body_object = other.attachedRigidbody.gameObject;
        
        if(!hovered_by_hand_collider_dict.ContainsKey(body_object)) return;
    
        hovered_by_hand_collider_dict[body_object].Remove(other);
    
        //if(hovered_by_hand_collider_dict[hand_controller].Count > 0) return;

        //if (hovered_by_hand_object_dict.ContainsKey(other.attachedRigidbody.gameObject))
        //    hovered_by_hand_object_dict.Remove(other.attachedRigidbody.gameObject);

        //hovered_by_hand_collider_dict.Remove(hand_controller);
        //
        //switch (grabbedWith)
        //{
        //    case GrabbedWith.Trigger:
        //        hand_controller.triggerPressed -= OnGrabbed;
        //        break;
        //    case GrabbedWith.Grip:
        //        hand_controller.gripPressed -= OnGrabbed;
        //        break;
        //}
    }
    
}
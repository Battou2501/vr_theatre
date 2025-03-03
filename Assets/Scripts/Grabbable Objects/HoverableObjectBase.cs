using System;
using System.Collections.Generic;
using UnityEngine;


public class HoverableObjectBase : MonoBehaviour, IInitable
{
    protected TagHandle hand_tag_handle;

    protected Dictionary<GameObject, List<Collider>> hovered_by_hand_collider_dict;
    
    public bool IsHovered => hovered_by_hand_collider_dict is {Count: > 0};

    protected virtual void OnDisable()
    {
        hovered_by_hand_collider_dict?.Clear();
    }

    public virtual void init()
    {
        hovered_by_hand_collider_dict = new Dictionary<GameObject, List<Collider>>();

        hand_tag_handle = TagHandle.GetExistingTag("Hand Grab Sphere");
    }
    
    protected virtual void OnTriggerEnter(Collider other)
    {
        if(other.attachedRigidbody == null) return;
        
        if(!other.attachedRigidbody.gameObject.CompareTag(hand_tag_handle)) return;
        
        var body_object = other.attachedRigidbody.gameObject;
        
        if(!hovered_by_hand_collider_dict.ContainsKey(body_object))
            hovered_by_hand_collider_dict[body_object] = new List<Collider>();
    
        if(!hovered_by_hand_collider_dict[body_object].Contains(other))
            hovered_by_hand_collider_dict[body_object].Add(other);
    }
    
    protected virtual void OnTriggerExit(Collider other)
    {
        if(other.attachedRigidbody == null) return;
        
        var body_object = other.attachedRigidbody.gameObject;
        
        if(!hovered_by_hand_collider_dict.ContainsKey(body_object)) return;
    
        hovered_by_hand_collider_dict[body_object].Remove(other);
    
        if(hovered_by_hand_collider_dict[body_object].Count > 0) return;

        hovered_by_hand_collider_dict.Remove(body_object);
    }
    
}
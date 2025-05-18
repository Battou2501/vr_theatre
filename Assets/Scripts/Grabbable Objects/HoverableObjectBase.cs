using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class HoverableObjectBase : MonoBehaviour, IInitable
{
    protected TagHandle hand_tag_handle;

    private Dictionary<GameObject, List<Collider>> hovered_by_hand_collider_dict;
    
    protected bool IsHovered => hovered_by_hand_collider_dict is {Count: > 0};

    protected virtual void OnDisable()
    {
        hovered_by_hand_collider_dict?.Clear();
    }

    public virtual void init()
    {
        hovered_by_hand_collider_dict = new Dictionary<GameObject, List<Collider>>();

        hand_tag_handle = TagHandle.GetExistingTag("Hand Grab Sphere");
    }
    
    protected virtual void TriggerEnter(Collider other)
    {
        if(other.attachedRigidbody == null) return;
        
        if(!other.attachedRigidbody.gameObject.CompareTag(hand_tag_handle)) return;
        
        var body_object = other.attachedRigidbody.gameObject;
        
        if(!hovered_by_hand_collider_dict.ContainsKey(body_object))
            hovered_by_hand_collider_dict[body_object] = new List<Collider>();
    
        if(!hovered_by_hand_collider_dict[body_object].Contains(other))
            hovered_by_hand_collider_dict[body_object].Add(other);
    }
    
    protected virtual void TriggerExit(Collider other)
    {
        if(other.attachedRigidbody == null) return;
        
        var body_object = other.attachedRigidbody.gameObject;
        
        if(!hovered_by_hand_collider_dict.ContainsKey(body_object)) return;
    
        hovered_by_hand_collider_dict[body_object].Remove(other);
    
        if(hovered_by_hand_collider_dict[body_object].Count > 0) return;

        hovered_by_hand_collider_dict.Remove(body_object);
    }

    private HashSet<Collider> hovered_this_frame = new HashSet<Collider>();
    private HashSet<Collider> hovered_last_frame = new HashSet<Collider>();
    
    private void FixedUpdate()
    {
        CheckEnter();
        CheckExit();
        
        hovered_last_frame.Clear();
        hovered_last_frame.AddRange(hovered_this_frame);
        hovered_this_frame.Clear();
    }

    private void OnTriggerStay(Collider other)
    {
        hovered_this_frame.Add(other);
    }

    void CheckEnter()
    {
        foreach (var collider1 in hovered_this_frame)
        {
            if (!hovered_last_frame.Contains(collider1))
            {
                TriggerEnter(collider1);
            }
        }
    }
    
    void CheckExit()
    {
        foreach (var collider1 in hovered_last_frame)
        {
            if (!hovered_this_frame.Contains(collider1))
            {
                TriggerExit(collider1);
            }
        }
    }
}
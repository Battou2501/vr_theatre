using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GrabbableSpawner : HoverableObjectBase
{
    //[SerializeField]
    //private GameObject spawnedObjectPrefab;
    [SerializeField]
    private GrabbableObjectsPool grabbableObjectsPool;

    //DiContainer container;
    //
    //[Inject]
    //public void Construct(DiContainer c)
    //{
    //    container = c;
    //}
    
    //protected virtual void OnGrabbed(HandController hand_controller)
    //{
    //    spawn_grabbable(hand_controller);
    //    
    //    hovered_by_hand_collider_dict.Remove(hand_controller);
    //    
    //    switch (grabbedWith)
    //    {
    //        case GrabbedWith.Trigger:
    //            hand_controller.triggerPressed -= OnGrabbed;
    //            break;
    //        case GrabbedWith.Grip:
    //            hand_controller.gripPressed -= OnGrabbed;
    //            break;
    //    }
    //}


    private void spawn_grabbable(HandController hand_controller)
    {
        if(hand_controller.is_grabbing) return;
        
        var spawned = grabbableObjectsPool.GetObject();
        
        //var spawned_grabbable = spawned.GetComponent<GrabbableObject>();
        
        spawned.OnGrabbed(hand_controller);
    }
}

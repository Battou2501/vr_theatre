using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GrabbableSpawner : GrabbableObject
{
    [SerializeField]
    private GrabbableObjectsPool grabbableObjectsPool;

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        
        //Debug.Log("Spawner trigger entered");
    }

    protected override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);
        
        //Debug.Log("Spawner trigger exited");
    }

    public override void OnGrabbed(HandController hand_controller)
    {
        if(hand_controller == null || hand_controller.is_grabbing) return;
        
        spawn_grabbable(hand_controller);
    }
    
    private void spawn_grabbable(HandController hand_controller)
    {
        if(hand_controller.is_grabbing) return;
        
        var spawned = grabbableObjectsPool.GetObject();
        
        spawned.OnGrabbed(hand_controller);
    }
}

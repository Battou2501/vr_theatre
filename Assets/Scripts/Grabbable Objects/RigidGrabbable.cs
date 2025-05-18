using System;
using Grabbable_Objects;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RigidGrabbable : GrabbableObject
{
    private Rigidbody rigid_body;
    private Vector3 moveVector;
    private Vector3 position_previous_frame;
    private Vector3 previous_move_vector;
    private PooledObject _pooledObject;
    public override void init()
    {
        base.init();
        
        rigid_body = GetComponent<Rigidbody>();
        _pooledObject = GetComponent<PooledObject>();
        position_previous_frame = transform.position;
    }

    private void OnEnable()
    {
        if(IsGrabbed || rigid_body == null) return;
        
        //rigid_body.detectCollisions = true;
        rigid_body.isKinematic = false;
        rigid_body.useGravity = true;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        
        if(rigid_body == null) return;
        
        //rigid_body.detectCollisions = false;
        rigid_body.isKinematic = true;
        rigid_body.useGravity = false;
        rigid_body.linearVelocity = Vector3.zero;
    }

    private void Update()
    {
        if(transform.position.sqrMagnitude > 200*200)
            ResetObject();
        
        if(!IsGrabbed) return;
        
        var newMoveVector = previous_move_vector * 0.5f + (transform.position - position_previous_frame) * 0.5f / Time.deltaTime;
        
        previous_move_vector = moveVector;
        
        moveVector = newMoveVector;
        
        position_previous_frame = transform.position;
    }
    
    public override bool OnGrabbed(HandController hand_controller)
    {
        //if(IsGrabbed) return false;
        
        //rigid_body.detectCollisions = false;
        if(!base.OnGrabbed(hand_controller)) return false;
        
        rigid_body.isKinematic = true;
        rigid_body.useGravity = false;
        rigid_body.linearVelocity = Vector3.zero;
        //base.OnGrabbed(hand_controller);
        
        return true;
    }

    protected override bool OnReleased(HandController hand_controller)
    {
        if(!base.OnReleased(hand_controller)) return false;
        //rigid_body.detectCollisions = true;
        rigid_body.isKinematic = false;
        rigid_body.useGravity = true;
        rigid_body.linearVelocity = moveVector;
        return true;
    }

    private void ResetObject()
    {
        gameObject.SetActive(false);
        
        if(_pooledObject == null) return;
        
        transform.position = _pooledObject.pool.transform.position;
        transform.SetParent(_pooledObject.pool.transform);
    }
}

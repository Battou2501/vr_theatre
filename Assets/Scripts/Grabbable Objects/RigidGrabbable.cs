using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RigidGrabbable : GrabbableObject
{
    private Rigidbody rigid_body;
    
    public override void init()
    {
        base.init();
        
        rigid_body = GetComponent<Rigidbody>();
    }

    public override void OnGrabbed(HandController hand_controller)
    {
        rigid_body.detectCollisions = false;
        rigid_body.isKinematic = true;
        rigid_body.useGravity = false;
        rigid_body.linearVelocity = Vector3.zero;
        base.OnGrabbed(hand_controller);
    }

    protected override void OnReleased(HandController hand_controller)
    {
        base.OnReleased(hand_controller);
        rigid_body.detectCollisions = true;
        rigid_body.isKinematic = false;
        rigid_body.linearVelocity = hand_controller.get_move_vector;
    }
}

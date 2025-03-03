using UnityEngine;

public class AnchoredGrabbable : GrabbableObject
{
    [SerializeField]
    private Transform anchorPoint;

    protected override void OnDisable()
    {
        base.OnDisable();
        
        transform.position = anchorPoint.position;
        transform.rotation = anchorPoint.rotation;
        anchorPoint.SetParent(transform);
    }

    public override void OnGrabbed(HandController hand_controller)
    {
        anchorPoint.SetParent(null);
        base.OnGrabbed(hand_controller);
    }

    protected override void OnReleased(HandController hand_controller)
    {
        base.OnReleased(hand_controller);
        
        transform.position = anchorPoint.position;
        transform.rotation = anchorPoint.rotation;
        anchorPoint.SetParent(transform);
    }
}

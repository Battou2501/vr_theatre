using UnityEngine;

public class LowerFingersPoseController : FingerPoseController
{
    [SerializeField]
    LowerFingersPose lowerFingersPoseIdle;
    [SerializeField]
    LowerFingersPose lowerFingersPoseFist;

    public override void init(bool l)
    {
        throw new System.NotImplementedException();
    }

    public override void update_current_pose()
    {
        throw new System.NotImplementedException();
    }

    protected override void update_target_pose()
    {
        throw new System.NotImplementedException();
    }
}

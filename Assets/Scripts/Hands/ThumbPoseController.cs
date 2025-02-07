using UnityEngine;

public class ThumbPoseController : FingerPoseController
{
    [SerializeField]
    ThumbPose thumbPoseIdle;
    [SerializeField]
    ThumbPose thumbPoseUp;
    [SerializeField]
    ThumbPose thumbPoseOK;
    [SerializeField]
    ThumbPose thumbPoseFist;

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

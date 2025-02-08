using UnityEngine;
using UnityEngine.InputSystem;
using static HandPoseSystem;

public class HandPoseController : MonoBehaviour
{
    [SerializeField]
    ThumbPoseController thumbPoseController;
    [SerializeField]
    IndexPoseController indexPoseController;
    [SerializeField]
    LowerFingersPoseController lowerFingersPoseController;

    bool is_initialized;
    
    public void init(Handedness hand)
    {
        thumbPoseController.init(hand == Handedness.Left);
        indexPoseController.init(hand == Handedness.Left);
        lowerFingersPoseController.init(hand == Handedness.Left);
        
        is_initialized = true;
    }

    void Update()
    {
        if(!is_initialized) return;
        
        thumbPoseController.update_current_pose();
        indexPoseController.update_current_pose();
        lowerFingersPoseController.update_current_pose();
    }
}

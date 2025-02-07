using UnityEngine;
using UnityEngine.InputSystem;

public class HandPoseController : MonoBehaviour
{
    public enum Handedness
    {
        Left = 0,
        Right = 1
    }
    [SerializeField]
    ThumbPoseController thumbPoseController;
    [SerializeField]
    IndexPoseController indexPoseController;
    [SerializeField]
    LowerFingersPoseController lowerFingersPoseController;
    [SerializeField]
    Handedness hand;

    bool is_initialized;
    
    public void init()
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

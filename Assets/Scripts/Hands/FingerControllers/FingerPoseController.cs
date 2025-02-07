using UnityEngine;
using UnityEngine.InputSystem;

public abstract class FingerPoseController : MonoBehaviour
{
    [SerializeField]
    protected FingerController fingerController;
    [SerializeField]
    protected float poseUpdateSpeed;
    protected bool is_left;
    protected bool is_initialized; 
    protected InputActionMap action_map;
    public abstract void init(bool l);
    public abstract void update_current_pose();
    protected abstract void update_target_pose();
}

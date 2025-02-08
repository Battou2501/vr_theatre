using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;
using static UnityEngine.ScriptableObject;

public abstract class FingerPoseController : MonoBehaviour
{
    [SerializeField]
    protected FingerRigController[] fingerControllers;
    [SerializeField]
    protected FingersPoseSO poseIdle;
    [SerializeField]
    protected FingersPoseSO poseSpecial;
    //[SerializeField]
    //protected FingersPoseSO poseOK;
    [SerializeField]
    protected FingersPoseSO poseFist;
    
    protected bool is_initialized; 
    protected InputActionMap action_map;
    protected FingersPoseSO target_pose;
    
    private bool is_left;
    private FingersPoseSO grab_override_pose;
    private int fingers_count;
    private FingersPoseSO current_pose;
    private HandPoseSystem hand_pose_system;
    private float pose_update_speed => hand_pose_system == null ? 1 : hand_pose_system.poseUpdateSpeed;

    [Inject]
    public void Construct(HandPoseSystem handPoseSystem)
    {
        hand_pose_system = handPoseSystem;
    }

    public virtual bool init(bool l)
    {
        is_left = l;
        
        if(fingerControllers == null) return false;
        if(poseIdle == null || poseIdle.pose_data == null) return false;
        if(poseSpecial == null || poseSpecial.pose_data == null) return false;
        //if(poseOK == null || poseOK.pose_data == null) return false;
        if(poseFist == null || poseFist.pose_data == null) return false;

        //var lengths_arr = new[] {poseIdle.pose_data.Length, poseSpecial.pose_data.Length, poseOK.pose_data.Length, poseFist.pose_data.Length, fingerControllers.Length};
        var lengths_arr = new[] {poseIdle.pose_data.Length, poseSpecial.pose_data.Length, poseFist.pose_data.Length, fingerControllers.Length};
        
        if(lengths_arr.Distinct().Count() > 1) return false;

        fingers_count = fingerControllers.Length;
        
        current_pose = CreateInstance<FingersPoseSO>();
        
        target_pose = CreateInstance<FingersPoseSO>();
        
        current_pose.set_data(poseIdle);
        
        target_pose.set_data(poseIdle);

        set_fingers_pose();

        action_map = InputSystem.actions.FindActionMap(is_left ? "Left Controller" : "Right Controller");
        
        if( action_map == null ) return false;
        
        
        is_initialized = true;
        
        return true;
    }
    
    public void set_grip_override_pose(FingersPoseSO pose)
    {
        grab_override_pose = pose;
    }

    public void remove_grip_override_pose()
    {
        grab_override_pose = null;
    }

    public void update_current_pose()
    {
        if (!is_initialized || current_pose.Equals(target_pose)) return;
        
        //current_pose.Lerp(target_pose, Time.deltaTime * poseUpdateSpeed);
        current_pose.move_towards_linear_ease_out(target_pose, Time.deltaTime * pose_update_speed);
        
        set_fingers_pose();
    }
    
    protected abstract void update_target_pose();
    
    protected bool has_grab_override_pose()
    {
        var result = grab_override_pose != null;
        
        if(result && target_pose != grab_override_pose)
            target_pose.set_data(grab_override_pose);
        
        return result;
    }
    
    private void set_fingers_pose()
    {
        for (var i = 0; i < fingers_count; i++)
        {
            fingerControllers[i].set_pose(current_pose.pose_data[i]);
        }
    }
}

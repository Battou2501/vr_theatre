using System;
using Unity.XR.CoreUtils;
using UnityEngine;
using Zenject;

public class FacePlayer : MonoBehaviour
{
    public enum FaceType
    {
        DoNotFace = 0,
        FaceHorizontally,
        FaceAllDirections
    }

    enum Modes
    {
        Stationary = 0,
        Follow
    }
    
    public Transform target;

    public FaceType faceTargetType;
    public bool followTarget;
    
    public float distance;
    public float angle;
    //public float verticalOffset;
    public float followSpeed;
    public float maxTurnAngle;
    public float turnAngleDeadZone;
    public float moveDeadZone;
    public float stationaryModeThreshold;
    public float minHeight;
    public bool fixedHeight;

    Vector3 rightLimit;
    Vector3 leftLimit;
    
    Transform xr_origin;
    Transform this_transform;

    private Vector3 follow_forward;
    private Vector3 follow_position;
    private Vector3 follow_right => Vector3.Cross(Vector3.up, follow_forward);

    private Modes currentMode;
    
    [Inject]
    private void Construct(XROrigin o)
    {
        xr_origin = o.transform;
    }

    private void Awake()
    {
        this_transform = transform;
        rightLimit = Quaternion.AngleAxis(maxTurnAngle, Vector3.up) * Vector3.forward;
        leftLimit = Quaternion.AngleAxis(-maxTurnAngle, Vector3.up) * Vector3.forward;
        follow_forward = Vector3.Cross(target.right, Vector3.up);
        currentMode = Modes.Follow;
    }

    private void OnEnable()
    {
        InitializeFollow();

        face();
    }

    private void Update()
    {
        if(target == null)return;
        
        follow();
        
        face();
    }

    private void LimitTargetForwardAngle()
    {
        if (Vector3.Angle(Vector3.forward, follow_forward) <= maxTurnAngle) return;
        
        var view_dot = Vector3.Dot(follow_forward, Vector3.right);
        follow_forward = view_dot > 0 ? rightLimit : leftLimit;
    }

    private void calculate_target_follow_forward()
    {
        var target_forward = Vector3.Cross(target.right, Vector3.up);
        //var follow_dot = Vector3.Dot(target_forward, follow_right);
        //var follow_angle = Vector3.Angle(follow_forward, target_forward);
        var follow_angle = Vector3.Angle(Vector3.Scale(this_transform.position-target.position, new Vector3(1,0,1)), target_forward);
        follow_forward = target_forward;
        LimitTargetForwardAngle();
        var target_position = target.position + follow_forward * distance;
        var min_height_adjusted = xr_origin.position.y + minHeight;
        if(target_position.y < min_height_adjusted || fixedHeight)
            target_position.y = min_height_adjusted;
        
        if (follow_angle <= turnAngleDeadZone && Vector3.Distance(target_position, follow_position) <= moveDeadZone && currentMode == Modes.Stationary) return;
        
        //follow_forward = Quaternion.AngleAxis(follow_dot > 0 ? -turnAngleDeadZone : turnAngleDeadZone, Vector3.up) * target_forward;
        //follow_forward = target_forward;
        //LimitTargetForwardAngle();
        currentMode = Modes.Follow;
        follow_position = target_position;
        //return target_position;
    }

    private void InitializeFollow()
    {
        if (!followTarget) return;
        calculate_target_follow_forward();
        this_transform.position = follow_position;
    }
    
    private void follow()
    {
        if(!followTarget) return;
        
        calculate_target_follow_forward();
        
        //if(currentMode == Modes.Follow)
        this_transform.position = Vector3.Slerp(this_transform.position, follow_position, followSpeed * Time.deltaTime);
        
        if(Vector3.Distance(this_transform.position, follow_position) < stationaryModeThreshold && currentMode == Modes.Follow)
            currentMode = Modes.Stationary;
    }

    private void face()
    {
        if(faceTargetType == FaceType.DoNotFace) return;

        switch (faceTargetType)
        {
            case FaceType.FaceHorizontally:
                this_transform.rotation = Quaternion.LookRotation(target.position - this_transform.position, Vector3.up);
                //this_transform.eulerAngles = new Vector3(0, this_transform.eulerAngles.y, 0);
                break;
            case FaceType.FaceAllDirections:
                this_transform.LookAt(target.position);
                break;
            default:
                return;
        }
        
        //this_transform.Rotate(this_transform.right, angle);
        this_transform.localRotation *= Quaternion.AngleAxis(angle, Vector3.left);
    }
}

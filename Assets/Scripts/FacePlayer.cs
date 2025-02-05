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
    
    public Transform target;

    public FaceType faceTargetType;
    public bool followTarget;
    
    public float distance;
    public float angle;
    //public float verticalOffset;
    public float followSpeed;
    public float maxTurnAngle;
    public float minHeight;

    Vector3 rightLimit;
    Vector3 leftLimit;
    
    Transform xr_origin;
    Transform this_transform;

    [Inject]
    void Construct(XROrigin o)
    {
        xr_origin = o.transform;
    }
    
    void Awake()
    {
        this_transform = transform;
        rightLimit = Quaternion.AngleAxis(maxTurnAngle, Vector3.up) * Vector3.forward;
        leftLimit = Quaternion.AngleAxis(-maxTurnAngle, Vector3.up) * Vector3.forward;
    }

    void OnEnable()
    {
        if(followTarget)
            this_transform.position = get_target_position();

        face();
    }

    void Update()
    {
        if(target == null)return;

        follow();
        
        face();
    }

    Vector3 get_target_position()
    {
        var new_right = Vector3.Cross(Vector3.up, target.forward).normalized;
        var new_forward = Vector3.Cross(new_right, Vector3.up).normalized;

        if (Vector3.Angle(Vector3.forward, new_forward) > maxTurnAngle)
        {
            var view_dot = Vector3.Dot(new_forward, Vector3.right);
            new_forward = view_dot> 0 ? rightLimit : leftLimit;
            new_right = Vector3.Cross(Vector3.up, new_forward).normalized;
        }

        var target_vector = Quaternion.AngleAxis(angle, new_right) * new_forward;
        var target_position = target.position + target_vector * distance;// + Vector3.up * verticalOffset;
        
        var min_height_adjusted = xr_origin.position.y + minHeight;
        
        if(target_position.y < min_height_adjusted)
            target_position.y = min_height_adjusted;
        
        return target_position;
    }

    void follow()
    {
        if(!followTarget) return;
        
        var target_position = get_target_position();
        
        this_transform.position = Vector3.Slerp(this_transform.position, target_position, followSpeed * Time.deltaTime);
    }

    void face()
    {
        if(faceTargetType == FaceType.DoNotFace) return;

        switch (faceTargetType)
        {
            case FaceType.FaceHorizontally:
                this_transform.rotation = Quaternion.LookRotation(target.position - this_transform.position, Vector3.up);
                this_transform.eulerAngles = new Vector3(0, this_transform.eulerAngles.y, 0);
                break;
            case FaceType.FaceAllDirections:
                this_transform.LookAt(target.position);
                break;
            default:
                return;
        }
        
    }
}

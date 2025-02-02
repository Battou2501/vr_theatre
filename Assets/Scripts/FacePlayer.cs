using System;
using Unity.XR.CoreUtils;
using UnityEngine;

public class FacePlayer : MonoBehaviour
{
    public Transform target;
    public float distance;
    public float angle;
    public float verticalOffset;
    public float followSpeed;
    public float maxTurnAngle;
    public float scaleMultiplier;
    public float minHeight;

    Vector3 rightLimit;
    Vector3 leftLimit;
    
    Transform xr_origin;
    
    void Awake()
    {
        rightLimit = Quaternion.AngleAxis(maxTurnAngle, Vector3.up) * Vector3.forward;
        leftLimit = Quaternion.AngleAxis(-maxTurnAngle, Vector3.up) * Vector3.forward;
        transform.localScale *= scaleMultiplier;
        xr_origin = FindFirstObjectByType<XROrigin>(FindObjectsInactive.Include).transform;
    }

    void Update()
    {
        if(target == null)return;
        
        var new_right = Vector3.Cross(Vector3.up, target.forward).normalized;
        var new_forward = Vector3.Cross(new_right, Vector3.up).normalized;

        if (Vector3.Angle(Vector3.forward, new_forward) > maxTurnAngle)
        {
            var view_dot = Vector3.Dot(new_forward, Vector3.right);
            new_forward = view_dot> 0 ? rightLimit : leftLimit;
            new_right = Vector3.Cross(Vector3.up, new_forward).normalized;
        }

        var target_vector = Quaternion.AngleAxis(angle, new_right) * new_forward;
        var target_position = target.position + target_vector * distance + Vector3.up * verticalOffset;
        //var target_position = target.position + new_forward * distance;

        var min_height_adjusted = xr_origin.position.y + minHeight;
        
        if(target_position.y < min_height_adjusted)
            target_position.y = min_height_adjusted;
        
        transform.position = Vector3.Slerp(transform.position, target_position, followSpeed * Time.deltaTime);
        
        
        transform.LookAt(target.position + Vector3.up * verticalOffset);
    }
}

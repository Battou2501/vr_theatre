using System;
using UnityEngine;

public class FacePlayer : MonoBehaviour
{
    public Transform target;
    public float distance;
    public float angle;
    public float followSpeed;
    public float maxTurnAngle;
    public float scaleMultiplier;

    Vector3 rightLimit;
    Vector3 leftLimit;
    
    void Awake()
    {
        rightLimit = Quaternion.AngleAxis(maxTurnAngle, Vector3.up) * Vector3.forward;
        leftLimit = Quaternion.AngleAxis(-maxTurnAngle, Vector3.up) * Vector3.forward;
        transform.localScale *= scaleMultiplier;
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
        var target_position = target.position + target_vector * distance;
        //var target_position = target.position + new_forward * distance;
        
        transform.position = Vector3.Slerp(transform.position, target_position, followSpeed * Time.deltaTime);
        transform.LookAt(target.position);
    }
}

using UnityEngine;

public class FacePlayer : MonoBehaviour
{
    public Transform target;
    public float distance;
    public float angle;
    public float followSpeed;

    void Update()
    {
        if(target == null)return;
        
        var new_right = Vector3.Cross(Vector3.up, target.forward).normalized;
        var new_forward = Vector3.Cross(new_right, Vector3.up).normalized;
        var target_vector = Quaternion.AngleAxis(angle, new_right) * new_forward;
        var target_position = target.position + target_vector * distance;
        
        transform.position = Vector3.Slerp(transform.position, target_position, followSpeed * Time.deltaTime);
        transform.LookAt(target.position);
    }
}

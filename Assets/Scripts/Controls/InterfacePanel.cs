using System;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace DefaultNamespace
{
    public class InterfacePanel : MonoBehaviour
    {
        protected Transform camera_transform;
        public float followSpeed;

        Vector3 cam_forward;
        Vector3 pos;
        Vector3 look_vec;
        Vector3 cam_pos;
        
        void Update()
        {
            cam_pos = camera_transform.position;
            
            cam_forward = Vector3.ProjectOnPlane(camera_transform.forward, Vector3.up).normalized * 0.5f;

            pos = Vector3.Slerp(transform.position, cam_pos + cam_forward, Time.deltaTime * followSpeed);
            
            transform.position = pos;

            look_vec = -cam_forward;
            
            transform.rotation = Quaternion.LookRotation(look_vec, Vector3.up);
        }
    }
}
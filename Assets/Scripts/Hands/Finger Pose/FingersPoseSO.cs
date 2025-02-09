using System;
using System.Linq;
using VrTheatre.Hands;
using UnityEngine;

[CreateAssetMenu(fileName = "Fingers Pose Preset", menuName = "Hand Poses")]
public class FingersPoseSO : ScriptableObject
{
    [SerializeField]
    public FingerPoseData[] pose_data;

    public void Lerp(FingersPoseSO other, float lerp)
    {
        if(other.pose_data == null || pose_data == null || pose_data.Length != other.pose_data.Length) return;

        for (var i = 0; i < pose_data.Length; i++)
        {
            pose_data[i].Lerp(other.pose_data[i], lerp);
        }
    }

    public void move_towards_linear_ease_out(FingersPoseSO other, float dist)
    {
        if(other.pose_data == null || pose_data == null || pose_data.Length != other.pose_data.Length) return;

        for (var i = 0; i < pose_data.Length; i++)
        {
            pose_data[i].move_towards_linear_ease_out(other.pose_data[i], dist);
        }
    }
    
    public static void Lerp(FingersPoseSO a, FingersPoseSO b, float t, FingersPoseSO result)
    {
        if(a.pose_data == null || b.pose_data == null || a.pose_data.Length != b.pose_data.Length) return;
        
        if(result == null)
            result = CreateInstance<FingersPoseSO>();
        
        if(result.pose_data == null || result.pose_data.Length != a.pose_data.Length)
            result.pose_data = new FingerPoseData[a.pose_data.Length];

        for (var i = 0; i < a.pose_data.Length; i++)
        {
            result.pose_data[i].bend = Mathf.Lerp(a.pose_data[i].bend, b.pose_data[i].bend, t);
            //result.index_pose_data.rootRotation = Vector3.Slerp(a.index_pose_data.rootRotation, b.index_pose_data.rootRotation, t);
            result.pose_data[i].rootRotation = Quaternion.Slerp(a.pose_data[i].rootRotation, b.pose_data[i].rootRotation, t);
        }
    }

    public void set_data(FingersPoseSO data)
    {
        if(data == null || data.pose_data == null) return;
        
        if(pose_data == null || pose_data.Length != data.pose_data.Length)
            pose_data = new FingerPoseData[data.pose_data.Length];
        
        Buffer.BlockCopy(data.pose_data, 0, pose_data, 0, pose_data.Length);
    }
}
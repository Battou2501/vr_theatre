using System;
using UnityEngine;

namespace VrTheatre.Hands
{
    [Serializable]
    public struct FingerPoseData
    {
        //Multiplier to convert 180 degree to 2.0f
        const float HalfRotationTo2 = 0.0111111111f;
        
        [SerializeField]
        public Quaternion rootRotation;
        [SerializeField]
        public Quaternion secondPhalanxRotation;
        [SerializeField]
        public Quaternion thirdPhalanxRotation;
        //[SerializeField]
        //public float bend;

        public void Lerp(FingerPoseData other, float t)
        {
            //bend = Mathf.Lerp(bend, other.bend, t);
            rootRotation = Quaternion.Slerp(rootRotation, other.rootRotation, t);
            secondPhalanxRotation = Quaternion.Slerp(secondPhalanxRotation, other.secondPhalanxRotation, t);
            thirdPhalanxRotation = Quaternion.Slerp(thirdPhalanxRotation, other.thirdPhalanxRotation, t);
        }

        public static void Lerp(FingerPoseData a, FingerPoseData b, float t, ref FingerPoseData target)
        {
            target.rootRotation = Quaternion.Slerp(a.rootRotation, b.rootRotation, t);
            target.secondPhalanxRotation = Quaternion.Slerp(a.secondPhalanxRotation, b.secondPhalanxRotation, t);
            target.thirdPhalanxRotation = Quaternion.Slerp(a.thirdPhalanxRotation, b.thirdPhalanxRotation, t);
            //target.bend = Mathf.Lerp(b.bend, a.bend, t);
        }
        
        public void move_towards_linear_ease_out(FingerPoseData other, float t)
        {
            //var bend_distance = Mathf.Abs(other.bend - bend);
            //var lerp_pos_bend = 1f - (bend_distance > 0 ? t / bend_distance : 1);
            //lerp_pos_bend = 1f - lerp_pos_bend * lerp_pos_bend * lerp_pos_bend;
            
            //bend = Mathf.Lerp(bend, other.bend, lerp_pos_bend);

            //var rot_this = rootRotation;
            //var rot_other = other.rootRotation;
            //
            ////Converting 0-180 angle to 0-2 range because 0-1 bend of a Phalanx is 0-90 angle of rotation of one Phalanx
            //var rot_distance = Quaternion.Angle(rot_this, rot_other) * HalfRotationTo2;
            //var lerp_pos_rot = 1f - (rot_distance > 0 ? t / rot_distance : 1);
            //lerp_pos_rot = 1f - lerp_pos_rot * lerp_pos_rot * lerp_pos_rot;
            
            //rootRotation = Quaternion.Slerp(rot_this, rot_other, lerp_pos_rot);
            rootRotation = slerp_or_linear_motion(rootRotation, other.rootRotation, t);
            secondPhalanxRotation = slerp_or_linear_motion(secondPhalanxRotation, other.secondPhalanxRotation, t);
            thirdPhalanxRotation = slerp_or_linear_motion(thirdPhalanxRotation, other.thirdPhalanxRotation, t);
        }

        public Quaternion slerp_or_linear_motion(Quaternion rot_a, Quaternion rot_b, float t)
        {
            //Converting 0-180 angle to 0-2 range because 0-1 bend of a Phalanx is 0-90 angle of rotation of one Phalanx
            var rot_distance = Quaternion.Angle(rot_a, rot_b) * HalfRotationTo2;
            var lerp_pos_rot = 1f - (rot_distance > 0 ? t / rot_distance : 1);
            lerp_pos_rot = 1f - lerp_pos_rot * lerp_pos_rot * lerp_pos_rot;
            
            return Quaternion.Slerp(rot_a, rot_b, lerp_pos_rot);
        }

        public override bool Equals(object obj)
        {
            return obj is FingerPoseData other && rootRotation.Equals(other.rootRotation) //&& bend.Equals(other.bend);
            && secondPhalanxRotation.Equals(other.secondPhalanxRotation)
            && thirdPhalanxRotation.Equals(other.thirdPhalanxRotation);
        }

        public override int GetHashCode()
        {
            return rootRotation.GetHashCode() ^ secondPhalanxRotation.GetHashCode() ^ thirdPhalanxRotation.GetHashCode();// ^ bend.GetHashCode();
        }
    }
}
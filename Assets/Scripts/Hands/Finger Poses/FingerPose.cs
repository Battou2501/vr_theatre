using UnityEngine;

namespace VrTheatre.Hands
{
    public struct FingerPose
    {
        //Multiplier to convert 180 degree to 2.0f
        const float HalfRotationTo2 = 0.0111111111f;
        
        public Vector3 rootRotation;
        public float bend;

        public void Lerp(FingerPose other, float t)
        {
            bend = Mathf.Lerp(bend, other.bend, t);
            rootRotation = Quaternion.Slerp(Quaternion.Euler(rootRotation), Quaternion.Euler(other.rootRotation), t).eulerAngles;
        }
        
        public void move_towards_linear_ease_out(FingerPose other, float t)
        {
            var bend_distance = Mathf.Abs(other.bend - bend);
            var lerp_pos_bend = 1f - (bend_distance > 0 ? t / bend_distance : 1);
            lerp_pos_bend = 1f - lerp_pos_bend * lerp_pos_bend * lerp_pos_bend;
            
            bend = Mathf.Lerp(bend, other.bend, lerp_pos_bend);

            var rot_this = Quaternion.Euler(rootRotation);
            var rot_other = Quaternion.Euler(other.rootRotation);
            
            //Converting 0-180 angle to 0-2 range because 0-1 bend of a Phalanx is 0-90 angle of rotation of one Phalanx
            var rot_distance = Quaternion.Angle(rot_this, rot_other) * HalfRotationTo2;
            var lerp_pos_rot = 1f - (rot_distance > 0 ? t / rot_distance : 1);
            lerp_pos_rot = 1f - lerp_pos_rot * lerp_pos_rot * lerp_pos_rot;
            
            rootRotation = Quaternion.Slerp(rot_this, rot_other, lerp_pos_rot).eulerAngles;
        }

        public override bool Equals(object obj)
        {
            return obj is FingerPose other && rootRotation.Equals(other.rootRotation) && bend.Equals(other.bend);
        }

        public override int GetHashCode()
        {
            return rootRotation.GetHashCode() ^ bend.GetHashCode();
        }

        //public static FingerPose Lerp(FingerPose a, FingerPose b, float t)
        //{
        //    var new_pose = new FingerPose();
        //    new_pose.bend = Mathf.Lerp(a.bend, b.bend, t);
        //    new_pose.rootRotation = Vector3.Slerp(a.rootRotation, b.rootRotation, t);
        //    return new_pose;
        //}
    }
}
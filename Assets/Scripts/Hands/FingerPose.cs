using Unity.VisualScripting;
using UnityEngine;

namespace VrTheatre.Hands
{
    public struct FingerPose
    {
        public Vector3 rootRotation;
        public float bend;

        public void Lerp(FingerPose other, float t)
        {
            bend = Mathf.Lerp(bend, other.bend, t);
            rootRotation = Vector3.Slerp(rootRotation, other.rootRotation, t);
        }

        public override bool Equals(object obj)
        {
            return obj is FingerPose other && rootRotation.Equals(other.rootRotation) && bend.Equals(other.bend);
        }

        public override int GetHashCode()
        {
            return rootRotation.GetHashCode() ^ bend.GetHashCode();
        }

        public static FingerPose Lerp(FingerPose a, FingerPose b, float t)
        {
            var new_pose = new FingerPose();
            new_pose.bend = Mathf.Lerp(a.bend, b.bend, t);
            new_pose.rootRotation = Vector3.Slerp(a.rootRotation, b.rootRotation, t);
            return new_pose;
        }
    }
}
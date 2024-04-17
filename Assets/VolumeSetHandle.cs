using UnityEngine;

namespace DefaultNamespace
{
    public class VolumeSetHandle : MonoBehaviour
    {
        public Transform minPoint;
        public Transform maxPoint;
        
        PlayerPanel panel;

        float volume_position => Vector3.Distance(minPoint.position, transform.position) / Vector3.Distance(minPoint.position, maxPoint.position);
        
        public void init(PlayerPanel p)
        {
            panel = p;
        }

        public void Release()
        {
            panel.set_volume(volume_position);
        }
    }
}
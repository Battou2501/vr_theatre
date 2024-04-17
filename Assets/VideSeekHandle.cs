using UnityEngine;

namespace DefaultNamespace
{
    public class VideSeekHandle : MonoBehaviour
    {
        public Transform minPoint;
        public Transform maxPoint;
        
        PlayerPanel panel;

        float video_position => Vector3.Distance(minPoint.position, transform.position) / Vector3.Distance(minPoint.position, maxPoint.position);
        
        public void init(PlayerPanel p)
        {
            panel = p;
        }

        public void Release()
        {
            panel.set_time(panel.Video_length * video_position);
        }

    }
}
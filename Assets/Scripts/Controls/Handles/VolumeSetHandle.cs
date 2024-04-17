using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class VolumeSetHandle : DraggableHandle
    {
        public Transform minPoint;
        public Transform maxPoint;
        
        PlayerPanel panel;

        float volume_position => Vector3.Distance(minPoint.position, transform.position) / Vector3.Distance(minPoint.position, maxPoint.position);
        
        public void init(PlayerPanel p)
        {
            panel = p;
        }

        public override void StopDrag()
        {
            panel.set_volume(volume_position);
        }

        void Update()
        {
            if (!isDragged)
            {
                transform.position = Vector3.Lerp(minPoint.position, maxPoint.position, panel.Audio_volume);
                
                return;
            }
            
            if(!Extensions.ClosestPointsOnTwoLines(out var pos, 
                   minPoint.position, 
                   maxPoint.position, 
                   panel.active_hand_transform.position, 
                   panel.active_hand_transform.forward*20)) return;

            transform.position = pos;
        }
    }
}
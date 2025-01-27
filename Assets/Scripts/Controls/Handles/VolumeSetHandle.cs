using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class VolumeSetHandle : DraggableHandle
    {
        public Transform minPoint;
        public Transform maxPoint;
        
        BaseControlsPanel parent_panel;

        float volume_position => Vector3.Distance(minPoint.position, transform.position) / Vector3.Distance(minPoint.position, maxPoint.position);

        public override void init(MainControls m)
        {
            base.init(m);

            parent_panel = GetComponentInParent<BaseControlsPanel>(true);
        }

        protected override void StopDrag_Action()
        {
            video_manager.set_volume(volume_position);
        }

        void Update()
        {
            if (!isDragged)
            {
                transform.position = Vector3.Lerp(minPoint.position, maxPoint.position, video_manager.Audio_volume);
                
                return;
            }
            
            if(!Extensions.ClosestPointsOnTwoLines(out var pos, 
                   minPoint.position, 
                   maxPoint.position, 
                   main_controls.Active_hand_transform.position, 
                   main_controls.Active_hand_transform.forward*20)) return;

            transform.position = pos;
        }
    }
}
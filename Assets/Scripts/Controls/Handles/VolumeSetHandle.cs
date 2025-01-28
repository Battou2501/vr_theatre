using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class VolumeSetHandle : DraggableHandle
    {
        protected override void OnDragged()
        {
            if(!video_manager.Vp_is_prepared) return;
            
            video_manager.set_volume((float)slider_position);
        }

        protected override void OnNotDragged()
        {
            if(!video_manager.Vp_is_prepared) return;
            
            transform.position = Vector3.Lerp(minPoint.position, maxPoint.position, video_manager.Audio_volume);
        }
    }
}
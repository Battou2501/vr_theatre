using System;
using System.Numerics;
using UnityEngine;
using UnityEngine.Serialization;
using Vector3 = UnityEngine.Vector3;

namespace DefaultNamespace
{
    public class VolumeSetHandle : DraggableHandle
    {
        float set_volume;
        
        //public override void init()
        //{
        //    base.init();
        //
        //    video_manager.set_volume(1);
        //    set_volume = 1;
        //    transform.position = maxPoint.position;
        //    update_value_bar();
        //}
        
        protected override void OnDragged()
        {
            if(!video_manager.Vp_is_prepared) return;

            var vol = (float) slider_position;
            
            video_manager.set_volume(vol);

            set_volume = vol;
            
            update_value_bar();
        }

        protected override void OnNotDragged()
        {
            if(Mathf.Abs(video_manager.Audio_volume - set_volume) < 0.01f) return;
            
            transform.position = Vector3.Lerp(minPoint.position, maxPoint.position, video_manager.Audio_volume);
            
            set_volume = video_manager.Audio_volume;
            
            update_value_bar();
        }
    }
}
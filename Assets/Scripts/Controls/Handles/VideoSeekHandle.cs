using System;
using TMPro;
using UnityEngine;
using Zenject;

namespace DefaultNamespace
{
    public class VideoSeekHandle : DraggableHandle
    {
        double video_length => video_manager.Video_length;
        double video_time_dragged => slider_position * video_length;

        ManageVideoPlayerAudio video_manager;
        
        [Inject]
        public void Construct(ManageVideoPlayerAudio v)
        {
            video_manager = v;
        }
        
        protected override void StopDrag_Action()
        {
            if(video_manager.is_seeking)
                video_manager.is_seeking = false;
            
            if(!video_manager.Vp_is_prepared) return;
            
            video_manager.request_skip_to_time(video_length * slider_position);
        }

        protected override void OnDragged()
        {
            if(!video_manager.is_seeking)
                video_manager.is_seeking = true;
            
            video_manager.seek_time = video_time_dragged;

            update_value_bar();
        }

        protected override void OnNotDragged()
        {
            if(video_manager.is_seeking)
                video_manager.is_seeking = false;
            
            transform.position = Vector3.Lerp(minPoint.position, maxPoint.position, video_manager.Video_time_01);

            update_value_bar();
        }

    }
}
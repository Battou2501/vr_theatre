using System;
using TMPro;
using UnityEngine;

namespace DefaultNamespace
{
    public class VideoSeekHandle : DraggableHandle
    {
        public Transform playedBar;

        Vector3 played_bar_initial_scale;
        
        double video_length => video_manager.Video_length;
        //string video_time_dragged => TimeSpan.FromSeconds(slider_position * video_length).ToString(@"hh\:mm\:ss");
        double video_time_dragged => slider_position * video_length;
        
        float slider_position_01 => Vector3.Distance(minPoint.position, transform.position) / Vector3.Distance(minPoint.position, maxPoint.position); 

        public override void init(MainControls m)
        {
            base.init(m);
            
            if(playedBar == null) return;
            
            played_bar_initial_scale = playedBar.localScale;
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
            //if(!video_manager.Vp_is_prepared) return;
            
            if(!video_manager.is_seeking)
                video_manager.is_seeking = true;
            
            video_manager.seek_time = video_time_dragged;

            update_played_bar();
        }

        protected override void OnNotDragged()
        {
            //if(!video_manager.Vp_is_prepared) return;
            
            if(video_manager.is_seeking)
                video_manager.is_seeking = false;
            
            transform.position = Vector3.Lerp(minPoint.position, maxPoint.position, video_manager.Video_time_01);

            update_played_bar();
        }

        void update_played_bar()
        {
            if(playedBar == null) return;

            playedBar.position = Vector3.Lerp(minPoint.position, transform.position, 0.5f);
            playedBar.localScale = Vector3.Scale(played_bar_initial_scale,new Vector3(slider_position_01,1,1));
        }
    }
}
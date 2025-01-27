using System;
using TMPro;
using UnityEngine;

namespace DefaultNamespace
{
    public class VideoSeekHandle : DraggableHandle
    {
        public Transform minPoint;
        public Transform maxPoint;
        public TMP_Text timeText;

        double min_max_distance_ratio;
        
        double video_length => video_manager.Video_length;
        double video_position => Vector3.Distance(minPoint.position, transform.position) * min_max_distance_ratio;
        string video_time_dragged => TimeSpan.FromSeconds(video_position * video_length).ToString(@"hh\:mm\:ss");

        public override void init(MainControls m)
        {
            base.init(m);
            
            min_max_distance_ratio = 1f / Vector3.Distance(minPoint.position, maxPoint.position);
        }

        protected override void StopDrag_Action()
        {
            video_manager.request_skip_to_time(video_length * video_position);
        }


        void Update()
        {
            if(!is_initiated) return;
            
            if(!video_manager.Vp_is_prepared) return;
            
            timeText.text = video_time_dragged;
            
            if (!isDragged)
            {
                transform.position = Vector3.Lerp(minPoint.position, maxPoint.position, video_manager.Video_time_01);
                
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
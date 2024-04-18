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

        PlayerPanel panel;

        float video_position => Vector3.Distance(minPoint.position, transform.position) / Vector3.Distance(minPoint.position, maxPoint.position);
        string video_time_dragged => TimeSpan.FromSeconds(video_position * panel.Video_length).ToString("HH:mm:ss");

        public void init(PlayerPanel p)
        {
            panel = p;
        }

        protected override void StartDrag_Action()
        {
            timeText.gameObject.SetActive(true);
        }

        protected override void StopDrag_Action()
        {
            timeText.gameObject.SetActive(false);
            
            panel.set_time(panel.Video_length * video_position);
        }


        void Update()
        {
            if (!isDragged)
            {
                transform.position = Vector3.Lerp(minPoint.position, maxPoint.position, panel.Video_time_01);
                
                return;
            }

            timeText.text = video_time_dragged;
            
            if(!Extensions.ClosestPointsOnTwoLines(out var pos, 
                   minPoint.position, 
                   maxPoint.position, 
                   panel.active_hand_transform.position, 
                   panel.active_hand_transform.forward*20)) return;

            transform.position = pos;
        }
    }
}
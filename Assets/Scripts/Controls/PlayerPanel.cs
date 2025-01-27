using System;
using UnityEngine;
using UnityEngine.Video;

namespace DefaultNamespace
{
    public class PlayerPanel : BaseControlsPanel
    {
        public override void init(MainControls m)
        {
            base.init(m);
            
            close_panel();
        }
        
        public void play_pause()
        {
            video_manager.request_pause();
        }

        public void stop()
        {
            video_manager.request_stop();
        }

        public void close_panel()
        {
            gameObject.SetActive(false);
        }

        public void show_panel()
        {
            gameObject.SetActive(true);

            transform.position = camera_transform.position + Vector3.forward;
        }
    }
}
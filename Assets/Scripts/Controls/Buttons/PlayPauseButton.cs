using System;
using UnityEditor;
using UnityEngine;

namespace DefaultNamespace
{
    public class PlayPauseButton : ClickableButton
    {
        public GameObject playButtonObject;
        public GameObject pauseButtonObject;
        
        protected override void Click_Action()
        {
            video_manager.request_play_pause();
        }

        void Update()
        {
            if(!is_initiated) return;
            
            if(playButtonObject != null && playButtonObject.activeSelf != video_manager.IsPlaying)
                playButtonObject.SetActive(video_manager.IsPlaying);
            
            if(pauseButtonObject != null && pauseButtonObject.activeSelf == video_manager.IsPlaying)
                pauseButtonObject.SetActive(!video_manager.IsPlaying);
        }

        [CustomEditor(typeof(PlayPauseButton))]
        public class PlayPauseButtonEditor : ClickableButtonEditor {}
    }
}
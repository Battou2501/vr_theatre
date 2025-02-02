﻿using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    public class AudioTrackPanel : BaseControlsPanel
    {
        public GameObject trackPrefab;
        public Transform contentBlock;
        public Transform topCup;
        public Transform middle;
        public Transform bottomCup;
        public float stepBetweenButtons;

        Vector3 middle_initial_scale;
        List<SetAudioTrackButton> track_buttons;
        string file_path;
        
        public override void init(MainControls m)
        {
            base.init(m);
            
            middle_initial_scale = middle.localScale;

            video_manager.VideoPrepared += refresh_buttons;
        }

        //public override void show()
        //{
        //    base.show();
        //    
        //    refresh_buttons();
        //}

        void refresh_buttons()
        {
            if(file_path == video_manager.FilePath)
                return;
            
            file_path = video_manager.FilePath;
            
            var tracks = video_manager.tracks;
            
            track_buttons?.for_each(x=>Destroy(x.gameObject));

            track_buttons = new List<SetAudioTrackButton>();
            
            if(tracks == null || tracks.Length == 0) return;
            
            for (var i = 0; i < tracks.Length; i++)
            {
                var track = tracks[i];

                var button = Instantiate(trackPrefab, contentBlock).GetComponent<SetAudioTrackButton>();
                
                button.init(main_controls);
                
                button.set_track_dta(i, track.lang);
                
                button.transform.localPosition += Vector3.up * stepBetweenButtons * (tracks.Length - 1 - i);
                
                track_buttons.Add(button);
            }
            
            topCup.localPosition = bottomCup.localPosition + Vector3.up * stepBetweenButtons * tracks.Length;
            middle.position = Vector3.Lerp(topCup.position,bottomCup.position,0.5f);
            middle.localScale = Vector3.Scale(middle_initial_scale,new Vector3(tracks.Length,1,1));
        }
    }
}
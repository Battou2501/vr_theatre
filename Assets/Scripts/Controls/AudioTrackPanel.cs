using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

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
        
        public Color defaultColor;
        public Color selectedColor;

        Vector3 middle_initial_scale;
        List<SetAudioTrackButton> track_buttons;
        string file_path;
        
        ManageVideoPlayerAudio video_manager;
        DiContainer container;
        
        [Inject]
        public void Construct(ManageVideoPlayerAudio v, DiContainer c)
        {
            video_manager = v;
            container = c;
        }
        
        public override void init()
        {
            base.init();
            
            middle_initial_scale = middle.localScale;

            video_manager.VideoPrepared += generate_buttons;
            video_manager.AudioTrackChanged += update_buttons_color;
        }


        void OnDestroy()
        {
            if(!is_initiated || video_manager == null) return;
            
            video_manager.VideoPrepared -= generate_buttons;
        }

        void generate_buttons()
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

                var button = container.InstantiatePrefab(trackPrefab, contentBlock).GetComponent<SetAudioTrackButton>();
                
                button.init();
                
                button.set_track_dta(i, track.lang);
                
                button.transform.localPosition += Vector3.up * stepBetweenButtons * (tracks.Length - 1 - i);
                
                button.set_color(button.track_idx == video_manager.CurrentTrackNumber ? selectedColor : defaultColor);
                
                track_buttons.Add(button);
            }
            
            topCup.localPosition = bottomCup.localPosition + Vector3.up * stepBetweenButtons * tracks.Length;
            middle.position = Vector3.Lerp(topCup.position,bottomCup.position,0.5f);
            middle.localScale = Vector3.Scale(middle_initial_scale,new Vector3(tracks.Length,1,1));
        }

        void update_buttons_color()
        {
            foreach (var button in track_buttons)
            {
                button.set_color(button.track_idx == video_manager.CurrentTrackNumber ? selectedColor : defaultColor);
            }
        }
    }
}
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    public class AudioTrackPanel : BaseControlsPanel
    {
        public GameObject trackPrefab;
        public Transform contentBlock;
        
        List<SetAudioTrackButton> track_buttons;
        string file_path;
        
        public override void init(MainControls m)
        {
            base.init(m);
            
            refresh_buttons();
        }

        public override void show()
        {
            base.show();
            
            if(file_path == video_manager.FilePath)
                return;
            
            refresh_buttons();
        }

        void refresh_buttons()
        {
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
                
                track_buttons.Add(button);
            }
        }
    }
}
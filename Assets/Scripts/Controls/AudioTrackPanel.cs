using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    public class AudioTrackPanel : InterfacePanel
    {
        public GameObject trackPrefab;
        public Transform contentBlock;
        
        PlayerPanel player_panel;


        List<SetAudioTrackButton> track_buttons;

        public void init(PlayerPanel p, ManageVideoPlayerAudio m)
        {
            player_panel = p;
            var tracks = m.tracks;
            
            track_buttons?.for_each(x=>Destroy(x.gameObject));

            track_buttons = new List<SetAudioTrackButton>();
            
            for (var i = 0; i < tracks.Length; i++)
            {
                var track = tracks[i];

                var button = Instantiate(trackPrefab, contentBlock).GetComponent<SetAudioTrackButton>();
                
                button.init(this,i, track.lang);
                
                track_buttons.Add(button);
            }
        }
        
        public void set_audio_track(int t)
        {
            player_panel.set_audio_track(t);
            close_panel();
        }

        public void show_panel()
        {
            gameObject.SetActive(true);
        }
        
        public void close_panel()
        {
            gameObject.SetActive(false);
        }
    }
}
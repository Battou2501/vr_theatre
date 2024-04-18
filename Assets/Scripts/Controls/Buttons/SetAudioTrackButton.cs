using System;
using TMPro;
using UnityEngine;

namespace DefaultNamespace
{
    public class SetAudioTrackButton : ClickableButton
    {
        public TMP_Text text;
        
        int track_idx;
        AudioTrackPanel panel;
        
        public void init(AudioTrackPanel p, int t, string lang)
        {
            panel = p;
            track_idx = t;
            text.text = $"Track {t} [{lang}]";
        }

        protected override void Click_Action()
        {
            panel.set_audio_track(track_idx);
        }
    }
}
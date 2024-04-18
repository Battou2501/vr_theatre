using UnityEngine;

namespace DefaultNamespace
{
    public class SetAudioTrackButton : ClickableButton
    {
        int track_idx;
        AudioTrackPanel panel;
        
        public void init(AudioTrackPanel p, int t)
        {
            panel = p;
            track_idx = t;
        }

        protected override void Click_Action()
        {
            panel.set_audio_track(track_idx);
        }
    }
}
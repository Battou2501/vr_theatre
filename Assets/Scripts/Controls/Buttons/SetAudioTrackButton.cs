using UnityEngine;

namespace DefaultNamespace
{
    public class SetAudioTrackButton : ClickableButton
    {
        int track_idx;
        PlayerPanel panel;
        
        public void init(PlayerPanel p)
        {
            panel = p;
        }

        public override void Click()
        {
            panel.set_audio_track(track_idx);
        }
    }
}
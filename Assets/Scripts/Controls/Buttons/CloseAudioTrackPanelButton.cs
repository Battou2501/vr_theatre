using UnityEngine;

namespace DefaultNamespace
{
    public class CloseAudioTrackPanelButton : ClickableButton
    {
        PlayerPanel panel;
        
        public void init(PlayerPanel p)
        {
            panel = p;
        }

        protected override void Click_Action()
        {
            panel.close_audio_track_panel();
        }
    }
}
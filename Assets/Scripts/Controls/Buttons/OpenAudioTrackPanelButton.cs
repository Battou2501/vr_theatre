using UnityEngine;

namespace DefaultNamespace
{
    public class OpenAudioTrackPanelButton : ClickableButton
    {
        PlayerPanel panel;
        
        public void init(PlayerPanel p)
        {
            panel = p;
        }

        protected override void Click_Action()
        {
            panel.show_audio_track_panel();
        }
    }
}
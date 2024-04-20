using UnityEngine;

namespace DefaultNamespace
{
    public class CloseAudioTrackPanelButton : ClickableButton
    { 
        AudioTrackPanel panel;
        
        public void init(AudioTrackPanel p)
        {
            panel = p;
        }

        protected override void Click_Action()
        {
            panel.close_panel();
        }
    }
}
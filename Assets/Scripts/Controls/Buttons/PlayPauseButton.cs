using UnityEngine;

namespace DefaultNamespace
{
    public class PlayPauseButton : ClickableButton
    {
        PlayerPanel panel;
        
        public void init(PlayerPanel p)
        {
            panel = p;
        }

        public override void Click()
        {
            panel.play_pause();
        }
    }
}
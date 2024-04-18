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

        protected override void Click_Action()
        {
            panel.play_pause();
        }
    }
}
using UnityEngine;

namespace DefaultNamespace
{
    public class StopButton : ClickableButton
    {
        PlayerPanel panel;
        
        public void init(PlayerPanel p)
        {
            panel = p;
        }

        public override void Click()
        {
            panel.stop();
        }
    }
}
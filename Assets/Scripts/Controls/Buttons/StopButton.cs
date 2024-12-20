﻿using UnityEngine;

namespace DefaultNamespace
{
    public class StopButton : ClickableButton
    {
        PlayerPanel panel;
        
        public void init(PlayerPanel p)
        {
            panel = p;
        }

        protected override void Click_Action()
        {
            panel.stop();
        }
    }
}
using UnityEngine;

namespace DefaultNamespace
{
    public class ClosePlayerPanelButton : ClickableButton
    {
        PlayerPanel player_panel;

        public void init(PlayerPanel c)
        {
            player_panel = c;
        }

        protected override void Click_Action()
        {
            player_panel.close_panel();
        }
    }
}
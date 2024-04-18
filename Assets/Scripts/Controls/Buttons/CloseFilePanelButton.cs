using UnityEngine;

namespace DefaultNamespace
{
    public class CloseFilePanelButton : ClickableButton
    {
        PlayerPanel player_panel;

        public void init(PlayerPanel p)
        {
            player_panel = p;
        }

        protected override void Click_Action()
        {
            player_panel.close_file_panel();
        }
    }
}
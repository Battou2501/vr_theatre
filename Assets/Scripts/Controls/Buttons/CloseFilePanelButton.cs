using UnityEngine;

namespace DefaultNamespace
{
    public class CloseFilePanelButton : ClickableButton
    {
        MainControls main_controls;

        public void init(MainControls c)
        {
            main_controls = c;
        }

        protected override void Click_Action()
        {
            main_controls.show_player_panel();
        }
    }
}
using UnityEngine;

namespace DefaultNamespace
{
    public class GoUpPathButton : ClickableButton
    {
        FileSelectPanel file_select_panel;

        public void init(FileSelectPanel p)
        {
            file_select_panel = p;
        }

        protected override void Click_Action()
        {
            file_select_panel.go_up_path();
        }
    }
}
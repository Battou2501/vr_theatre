using UnityEngine;

namespace DefaultNamespace
{
    public class OpenFileSelectPanelButton : ClickableButton
    {
        public BaseControlsPanel panel_to_open;
        BaseControlsPanel parent_panel;

        public override void init(MainControls m)
        {
            base.init(m);

            parent_panel = GetComponentInParent<BaseControlsPanel>(true);
        }

        protected override void Click_Action()
        {
            parent_panel.real_null()?.hide();
            panel_to_open.real_null()?.show();

            main_controls.check_hands_display();
        }
    }
}
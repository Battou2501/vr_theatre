#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DefaultNamespace
{
    public class OpenPanelButton : ClickableButton
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
            panel_to_open.real_null()?.show();
            parent_panel.real_null()?.hide();
            
            //main_controls.check_hands_display();
        }
#if UNITY_EDITOR
        [CustomEditor(typeof(OpenPanelButton))]
        public class OpenPanelButtonEditor : ClickableButtonEditor {}
#endif
    }
}
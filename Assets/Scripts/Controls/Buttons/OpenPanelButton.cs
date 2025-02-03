#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DefaultNamespace
{
    public class OpenPanelButton : ClickableButton
    {
        public BaseControlsPanel panel_to_open;
        BaseControlsPanel parent_panel;

        protected override void Click_Action()
        {
            if(parent_panel == null)
                parent_panel = GetComponentInParent<BaseControlsPanel>(true);
            
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
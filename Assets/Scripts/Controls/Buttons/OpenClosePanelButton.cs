
using System.Reflection.Emit;
using Cysharp.Threading.Tasks;
using Zenject;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DefaultNamespace
{
    public class OpenClosePanelButton : ClickableButton
    {
        public BaseControlsPanel panel_to_open;
        BaseControlsPanel parent_panel;
        UIManager ui_manager;
        
        [Inject]
        void Construct(UIManager u)
        {
            ui_manager = u;
        }
        
        protected override void Click_Action()
        {
            if(parent_panel == null)
                parent_panel = GetComponentInParent<BaseControlsPanel>(true);
            
            ui_manager.switch_panels(parent_panel, panel_to_open).Forget();
        }
        
#if UNITY_EDITOR
        [CustomEditor(typeof(OpenClosePanelButton))]
        public class OpenPanelButtonEditor : ClickableButtonEditor {}
#endif
    }
}
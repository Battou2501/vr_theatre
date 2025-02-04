
using Zenject;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DefaultNamespace
{
    public class GoUpPathButton : ClickableButton
    {
        
        FileNavigationManager file_navigation_manager;
        
        [Inject]
        public void Construct(FileNavigationManager f)
        {
            file_navigation_manager = f;
        }
        
        protected override void Click_Action()
        {
            file_navigation_manager.go_up_path();
        }
#if UNITY_EDITOR
        [CustomEditor(typeof(GoUpPathButton))]
        public class GoUpPathButtonEditor : ClickableButtonEditor {}
#endif
    }
}
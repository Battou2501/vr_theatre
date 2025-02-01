#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DefaultNamespace
{
    public class GoUpPathButton : ClickableButton
    {
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
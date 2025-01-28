using UnityEditor;
using UnityEngine;

namespace DefaultNamespace
{
    public class GoUpPathButton : ClickableButton
    {
        protected override void Click_Action()
        {
            main_controls.go_up_path();
        }
        
        [CustomEditor(typeof(GoUpPathButton))]
        public class GoUpPathButtonEditor : ClickableButtonEditor {}
    }
}
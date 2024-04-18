using UnityEngine;

namespace DefaultNamespace
{
    public class ClickableButton : MonoBehaviour
    {
        public bool isInteractable
        {
            get;
            private set;
        }

        public void set_interactable(bool s)
        {
            isInteractable = s;
        }

        public void Click()
        {
            if(!isInteractable) return;
            
            Click_Action();
        }

        protected virtual void Click_Action() {}
    }
}
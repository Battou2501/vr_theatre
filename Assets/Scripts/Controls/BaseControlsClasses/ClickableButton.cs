using UnityEngine;

namespace DefaultNamespace
{
    public class ClickableButton : BaseInteractableCOntrol
    {
        public void Click()
        {
            //if(!isInteractable) return;
            
            Click_Action();
        }

        protected virtual void Click_Action() {}
    }
}
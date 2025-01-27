using UnityEngine;

namespace DefaultNamespace
{
    public class DraggableHandle : BaseInteractableCOntrol
    {
        public bool isDragged;

        public void StartDrag()
        {
            if(!isInteractable) return;
            
            isDragged = true;

            StartDrag_Action();
        }

        public void StopDrag()
        {
            isDragged = false;

            StopDrag_Action();
        }

        protected virtual void StartDrag_Action()
        {
        }

        protected virtual void StopDrag_Action()
        {
        }
    }
}
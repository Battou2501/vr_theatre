using UnityEngine;

namespace DefaultNamespace
{
    public class DraggableHandle : MonoBehaviour
    {
        public bool isDragged;
        
        public bool isInteractable
        {
            get;
            private set;
        }

        public void set_interactable(bool s)
        {
            isInteractable = s;
        }

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
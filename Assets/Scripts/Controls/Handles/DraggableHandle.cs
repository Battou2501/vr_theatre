using UnityEngine;

namespace DefaultNamespace
{
    public class DraggableHandle : MonoBehaviour
    {
        public bool isDragged;

        public virtual void StartDrag()
        {
            isDragged = true;
        }

        public virtual void StopDrag()
        {
            isDragged = false;
        }
    }
}
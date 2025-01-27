using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace DefaultNamespace
{
    public class HandControls : MonoBehaviour
    {
        [FormerlySerializedAs("action")] public InputAction triggerPressedAction;

        public Transform pointer;

        Transform pointer_transform;
        
        MainControls main_controls;

        DraggableHandle dragged_handle;

        public bool Is_dragging_control => dragged_handle != null;
        
        public void init(MainControls c)
        {
            main_controls = c;
            
            triggerPressedAction.started += TriggerPressedActionOnstarted;
            triggerPressedAction.canceled+= TriggerPressedActionOncanceled;
            triggerPressedAction.Enable();
            
            pointer_transform = pointer.transform;
        }
        
        public void activate_hand()
        {
            pointer.gameObject.SetActive(true);
        }
        
        public void deactivate_hand()
        {
            pointer.gameObject.SetActive(false);
        }
        
        void TriggerPressedActionOnstarted(InputAction.CallbackContext obj)
        {
            if(!gameObject.activeInHierarchy) return;
            
            if(main_controls.Is_dragging_control && !Is_dragging_control) return;
            
            main_controls.set_active_hand(this);

            Physics.Raycast(pointer_transform.position, pointer_transform.forward, out var hit, 20);

            var button = hit.collider.GetComponent<ClickableButton>();

            if (button != null)
            {
                button.Click();
                return;
            }
            
            var handle = hit.collider.GetComponent<DraggableHandle>();

            if (handle == null) return;
            
            dragged_handle = handle;
            dragged_handle.StartDrag();

            //var rh = Physics.RaycastNonAlloc(pointer_transform.position, pointer_transform.forward, hits, 20);
            //
            //if(rh == 0) return;
            //
            //for (var i = 0; i < rh; i++)
            //{
            //    var hit = hits[i];
            //    
            //    var button = hit.collider.GetComponent<ClickableButton>();
            //
            //    button.real_null()?.Click();
            //
            //    var handle = hit.collider.GetComponent<DraggableHandle>();
            //    
            //    if(handle == null || dragged_handle != null) continue;
            //
            //    dragged_handle = handle;
            //
            //    dragged_handle.StartDrag();
            //}
        }

        void TriggerPressedActionOncanceled(InputAction.CallbackContext obj)
        {
            if(!gameObject.activeInHierarchy) return;
            
            if(!Is_dragging_control) return;
            
            dragged_handle.StopDrag();

            dragged_handle = null;
        }

        
    }
}
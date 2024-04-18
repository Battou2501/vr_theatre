using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace DefaultNamespace
{
    public class HandControls : MonoBehaviour
    {
        [FormerlySerializedAs("action")] public InputAction triggerPressedAction;

        public Collider pointer;

        Transform pointer_transform;
        
        MainControls main_controls;

        DraggableHandle dragged_handle;

        public bool Is_dragging_control => dragged_handle != null;
        
        public void init(MainControls c)
        {
            main_controls = c;
        }
        
        void Start()
        {
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
            
            Debug.Log("Started");
            
            if(main_controls.Is_dragging_control && !Is_dragging_control) return;
            
            main_controls.set_active_hand(this);

            var rh = Physics.Raycast(pointer_transform.position, pointer_transform.forward, out var hit, 20);
            
            if(!rh) return;

            var handle = hit.collider.GetComponent<DraggableHandle>();
            var button = hit.collider.GetComponent<ClickableButton>();
            
            button.real_null()?.Click();
            
            if(handle == null) return;

            dragged_handle = handle;
            
            dragged_handle.StartDrag();

        }

        void TriggerPressedActionOncanceled(InputAction.CallbackContext obj)
        {
            if(!gameObject.activeInHierarchy) return;
            
            Debug.Log("Canceled");
            
            if(!Is_dragging_control) return;
            
            dragged_handle.StopDrag();

            dragged_handle = null;
        }

        
    }
}
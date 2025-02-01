using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DefaultNamespace
{
    public class DraggableHandle : BaseControl
    {
        public Transform minPoint;
        public Transform maxPoint;
        
        bool is_dragged;

        Transform dragged_by;
        
        double min_max_distance_ratio;
        protected double slider_position => Vector3.Distance(minPoint.position, transform.position) * min_max_distance_ratio;

        public override void init(MainControls m)
        {
            base.init(m);
            
            min_max_distance_ratio = 1f / Vector3.Distance(minPoint.position, maxPoint.position);
            
            main_controls.leftTriggerPressedAction.performed += StartDrag;
            main_controls.rightTriggerPressedAction.performed += StartDrag;
            
            main_controls.leftTriggerPressedAction.canceled += StopDrag;
            main_controls.rightTriggerPressedAction.canceled += StopDrag;
        }

        void OnDestroy()
        {
            if(!is_initiated) return;
            
            main_controls.leftTriggerPressedAction.performed -= StartDrag;
            main_controls.rightTriggerPressedAction.performed -= StartDrag;
            
            main_controls.leftTriggerPressedAction.canceled -= StopDrag;
            main_controls.rightTriggerPressedAction.canceled -= StopDrag;
        }

        void StartDrag(InputAction.CallbackContext callbackContext)
        {
            if(!callbackContext.control.IsPressed()) return;
            
            if(main_controls.leftTriggerPressedAction.id == callbackContext.action.id && !hovered_by.Contains(main_controls.leftHandTriggerCollider)) return;
            if(main_controls.rightTriggerPressedAction.id == callbackContext.action.id && !hovered_by.Contains(main_controls.rightHandTriggerCollider)) return;
            
            if(is_dragged) return;
            
            is_dragged = true;

            dragged_by = main_controls.leftTriggerPressedAction.id == callbackContext.action.id ? main_controls.leftHandTriggerCollider.transform : main_controls.rightHandTriggerCollider.transform;

            StartDrag_Action();
        }

        void StopDrag(InputAction.CallbackContext callbackContext)
        {
            if(callbackContext.control.IsPressed()) return;
            
            if(!is_dragged) return;
            if(main_controls.leftTriggerPressedAction.id == callbackContext.action.id && dragged_by.gameObject != main_controls.leftHandTriggerCollider) return;
            if(main_controls.rightTriggerPressedAction.id == callbackContext.action.id && dragged_by.gameObject != main_controls.rightHandTriggerCollider) return;
            
            is_dragged = false;

            StopDrag_Action();
        }

        protected virtual void OnDragged() {}
        
        protected virtual void OnNotDragged() {}
        
        protected virtual void StartDrag_Action() {}

        protected virtual void StopDrag_Action() {}
        
        void Update()
        {
            if(!is_initiated) return;

            if (!is_dragged)
            {
                OnNotDragged();
                return;
            }

            if(!Extensions.ClosestPointsOnTwoLines(out var pos, 
                   minPoint.position, 
                   maxPoint.position, 
                   dragged_by.position, 
                   dragged_by.forward*20)) return;
            
            transform.position = pos;
            
            OnDragged();
        }
    }
}
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DefaultNamespace
{
    public class ClickableButton : BaseControl
    {
        public override void init(MainControls m)
        {
            base.init(m);

            main_controls.leftTriggerPressedAction.started += OnClick;
            main_controls.rightTriggerPressedAction.started += OnClick;
        }

        void OnDestroy()
        {
            if(!is_initiated) return;
            
            main_controls.leftTriggerPressedAction.started -= OnClick;
            main_controls.rightTriggerPressedAction.started -= OnClick;
        }

        void OnClick(InputAction.CallbackContext callbackContext)
        {
            if(main_controls.leftTriggerPressedAction.id == callbackContext.action.id && hovered_by != main_controls.leftHandTriggerCollider) return;
            if(main_controls.rightTriggerPressedAction.id == callbackContext.action.id && hovered_by != main_controls.rightHandTriggerCollider) return;
            
            Click_Action();
            
            animation_feedback.real_null()?.OnTriggerPressed();
        }

        protected virtual void Click_Action() {}
        
        [CustomEditor(typeof(ClickableButton))]
        public class ClickableButtonEditor : Editor
        {
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();
            
                if (GUILayout.Button("Click"))
                {
                    if(!Application.isPlaying || target == null) return;
                    
                    (target as ClickableButton).Click_Action();
                }
            }
        }
    }
}
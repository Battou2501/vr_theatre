#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.InputSystem;

namespace DefaultNamespace
{
    public class ClickableButton : BaseControl
    {
        public override void init(MainControls m)
        {
            base.init(m);

            main_controls.leftTriggerPressedAction.performed += OnClick;
            main_controls.rightTriggerPressedAction.performed += OnClick;
        }

        void OnDestroy()
        {
            if(!is_initiated) return;
            
            main_controls.leftTriggerPressedAction.performed -= OnClick;
            main_controls.rightTriggerPressedAction.performed -= OnClick;
        }

        void OnClick(InputAction.CallbackContext callbackContext)
        {
            if(!callbackContext.control.IsPressed()) return;
            
            if(main_controls.leftTriggerPressedAction.id == callbackContext.action.id && !hovered_by.Contains(main_controls.leftHandTriggerCollider)) return;
            if(main_controls.rightTriggerPressedAction.id == callbackContext.action.id && !hovered_by.Contains(main_controls.rightHandTriggerCollider)) return;
            
            Click_Action();
            
            animation_feedback.real_null()?.OnTriggerPressed();
        }

        protected virtual void Click_Action() {}
#if UNITY_EDITOR
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
#endif
    }
}
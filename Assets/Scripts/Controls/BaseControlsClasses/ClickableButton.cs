﻿#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace DefaultNamespace
{
    public class ClickableButton : BaseControl
    {
        VrInputSystem _inputSystem;
        
        [Inject]
        public void Construct(ManageVideoPlayerAudio v, MainControls m, VrInputSystem inputSystem)
        {
            _inputSystem = inputSystem;
        }
        
        public override void init()
        {
            base.init();

            //_inputSystem.leftLowerButtonPressedChanged += OnClickLeft;
            _inputSystem.leftTriggerPressedChanged += OnClickLeft;
            //_inputSystem.rightLowerButtonPressedChanged += OnClickRight;
            _inputSystem.rightTriggerPressedChanged += OnClickRight;
        }

        protected virtual void OnDestroy()
        {
            if(!is_initiated) return;
            
            //_inputSystem.leftLowerButtonPressedChanged -= OnClickLeft;
            //_inputSystem.rightLowerButtonPressedChanged -= OnClickRight;
            _inputSystem.leftTriggerPressedChanged -= OnClickLeft;
            _inputSystem.rightTriggerPressedChanged -= OnClickRight;
        }

        void OnClickLeft(bool is_pressed)
        {
            if(!is_pressed) return;
            
            if(!hovered_by.Contains(_leftHandTriggerCollider)) return;

            Press();
        }
        
        void OnClickRight(bool is_pressed)
        {
            if(!is_pressed) return;
            
            if(!hovered_by.Contains(_rightHandTriggerCollider)) return;

            Press();
        }

        private void Press()
        {
            if(GrabbableObject.grabbableObjects.Any(x=>x.IsHovered && x.grabbedWith == GrabbableObject.GrabbedWith.Trigger)) return;
            
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
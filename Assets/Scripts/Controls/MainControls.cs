﻿using System;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace DefaultNamespace
{
    public class MainControls : MonoBehaviour
    {
        [Header("Hand controls")]
        public GameObject leftHandTriggerCollider;
        public GameObject rightHandTriggerCollider;
        
        public InputAction leftTriggerPressedAction;
        public InputAction rightTriggerPressedAction;
        
        bool subscriber_to_trigger_actions;

        UIManager ui_manager;
        
        [Inject]
        public void Construct( UIManager u)
        {
            ui_manager = u;
        }
        
        public void init()
        {

            ui_manager.PanelCchanged += OnPanelChanged;
            
            leftTriggerPressedAction?.Enable();
            rightTriggerPressedAction?.Enable();
            
            //enable_trigger_check();
        }
        
        void OnPanelChanged(bool all_panels_closed)
        {
            if (all_panels_closed)
                hide_hands();
            else
                show_hands();
        }

        void OnDestroy()
        {
            //disable_trigger_check();
            
            if(ui_manager == null) return;
            
            ui_manager.PanelCchanged -= OnPanelChanged;
        }
        
        void show_hands()
        {
            leftHandTriggerCollider.real_null()?.SetActive(true);
            rightHandTriggerCollider.real_null()?.SetActive(true);
            //disable_trigger_check();
        }
        
        void hide_hands()
        {
            leftHandTriggerCollider.real_null()?.SetActive(false);
            rightHandTriggerCollider.real_null()?.SetActive(false);
            //enable_trigger_check();
        }

        //void enable_trigger_check()
        //{
        //    if(subscriber_to_trigger_actions) return;
        //    
        //    if (leftTriggerPressedAction != null)
        //    {
        //        leftTriggerPressedAction.performed += TriggerPressedActionOnStarted;
        //        leftTriggerPressedAction.Enable();
        //    }
        //
        //    if (rightTriggerPressedAction != null)
        //    {
        //        rightTriggerPressedAction.performed += TriggerPressedActionOnStarted;
        //        rightTriggerPressedAction.Enable();
        //    }
//
        //    subscriber_to_trigger_actions = true;
        //}
//
        //void disable_trigger_check()
        //{
        //    leftTriggerPressedAction.performed -= TriggerPressedActionOnStarted;
        //    rightTriggerPressedAction.performed -= TriggerPressedActionOnStarted;
        //    
        //    subscriber_to_trigger_actions = false;
        //}
        
        void TriggerPressedActionOnStarted(InputAction.CallbackContext callbackContext)
        {
            if (!callbackContext.control.IsPressed()) return;
            
            ui_manager.show_ui().Forget();
        }
    }
}
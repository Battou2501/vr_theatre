using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DefaultNamespace
{
    public class MainControls : MonoBehaviour
    {
        [Header("UI Controls elements")]
        public UIManager uiManager;
        
        [Header("Hand controls")]
        public GameObject leftHandTriggerCollider;
        public GameObject rightHandTriggerCollider;
        
        public InputAction leftTriggerPressedAction;
        public InputAction rightTriggerPressedAction;
        
        [Header("Video player manager")]
        public ManageVideoPlayerAudio videoManager;
        
        [Header("File navigation manager")]
        public FileNavigationManager fileNavigationManager;

        bool subscriber_to_trigger_actions;
        
        void Awake()
        {
            uiManager.init(this);

            enable_trigger_check();
        }

        void OnDestroy()
        {
            disable_trigger_check();
        }

        public void check_hands_display()
        {
            if(uiManager.is_all_panels_closed)
                hide_hands();
            else
                show_hands();
        }
        
        void show_hands()
        {
            leftHandTriggerCollider.real_null()?.SetActive(true);
            rightHandTriggerCollider.real_null()?.SetActive(true);
        }
        
        void hide_hands()
        {
            leftHandTriggerCollider.real_null()?.SetActive(false);
            rightHandTriggerCollider.real_null()?.SetActive(false);
        }

        public void enable_trigger_check()
        {
            if(subscriber_to_trigger_actions) return;
            
            if (leftTriggerPressedAction != null)
            {
                leftTriggerPressedAction.performed += TriggerPressedActionOnStarted;
                leftTriggerPressedAction.Enable();
            }

            if (rightTriggerPressedAction != null)
            {
                rightTriggerPressedAction.performed += TriggerPressedActionOnStarted;
                rightTriggerPressedAction.Enable();
            }

            subscriber_to_trigger_actions = true;
        }

        public void disable_trigger_check()
        {
            leftTriggerPressedAction.performed -= TriggerPressedActionOnStarted;
            rightTriggerPressedAction.performed -= TriggerPressedActionOnStarted;
            
            subscriber_to_trigger_actions = false;
        }
        
        void TriggerPressedActionOnStarted(InputAction.CallbackContext callbackContext)
        {
            if (!callbackContext.control.IsPressed()) return;
            
            uiManager.show_ui();
        }
    }
}
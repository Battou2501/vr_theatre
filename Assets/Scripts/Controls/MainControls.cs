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
        
        void Awake()
        {
            uiManager.init(this);
            
            if(leftTriggerPressedAction != null)
                leftTriggerPressedAction.started += TriggerPressedActionOnstarted;
            if(rightTriggerPressedAction != null)
                rightTriggerPressedAction.started += TriggerPressedActionOnstarted;
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

        void TriggerPressedActionOnstarted(InputAction.CallbackContext obj)
        {
            uiManager.show_ui();
        }
    }
}
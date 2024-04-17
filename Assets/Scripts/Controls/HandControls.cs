using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DefaultNamespace
{
    public class HandControls : MonoBehaviour
    {
        public InputAction action;

        void Start()
        {
            action.started += ActionOnstarted;
            action.performed+= ActionOnperformed;
            action.canceled+= ActionOncanceled;
            action.Enable();
        }

        void ActionOncanceled(InputAction.CallbackContext obj)
        {
            Debug.Log("Canceled");
        }

        void ActionOnstarted(InputAction.CallbackContext obj)
        {
            Debug.Log("Started");
        }

        void ActionOnperformed(InputAction.CallbackContext obj)
        {
            Debug.Log("Performed");
        }
    }
}
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace DefaultNamespace
{
    public class HandControls : MonoBehaviour
    {
        public Transform pointer;
        
        public void activate_hand()
        {
            pointer.gameObject.SetActive(true);
        }
        
        public void deactivate_hand()
        {
            pointer.gameObject.SetActive(false);
        }
        

        
    }
}
using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class TestOnTriggerStay : MonoBehaviour
    {
        public bool isActive;

        bool triggered;
        
        void FixedUpdate()
        {
            isActive = false;
            
            if(!triggered) return;

            triggered = false;

            isActive = true;
        }

        void OnTriggerStay(Collider other)
        {
            triggered = true;
        }
    }
}
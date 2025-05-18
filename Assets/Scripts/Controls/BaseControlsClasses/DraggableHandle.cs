using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace DefaultNamespace
{
    public class DraggableHandle : BaseControl
    {
        public Transform minPoint;
        public Transform maxPoint;
        public Transform valueBar;
        protected Vector3 value_bar_initial_scale;
        bool is_dragged;
        Transform dragged_by;
        double min_max_distance_ratio;
        VrInputSystem _inputSystem;
        
        protected double slider_position => Vector3.Distance(minPoint.localPosition, transform.localPosition) * min_max_distance_ratio;
        
        [Inject]
        public void Construct(ManageVideoPlayerAudio v, MainControls m, VrInputSystem inputSystem)
        {
            _inputSystem = inputSystem;
        }
        
        public override void init()
        {
            base.init();
            
            min_max_distance_ratio = 1f / Vector3.Distance(minPoint.localPosition, maxPoint.localPosition);
            
            //_inputSystem.leftLowerButtonPressedChanged += OnPressedLeft;
            //_inputSystem.rightLowerButtonPressedChanged += OnPressedRight;
            
            _inputSystem.leftTriggerPressedChanged += OnPressedLeft;
            _inputSystem.rightTriggerPressedChanged += OnPressedRight;
            
            if(valueBar == null) return;
            
            value_bar_initial_scale = valueBar.localScale;
        }

        protected void update_value_bar()
        {
            if(valueBar == null) return;

            valueBar.position = Vector3.Lerp(minPoint.position, transform.position, 0.5f);
            valueBar.localScale = Vector3.Scale(value_bar_initial_scale,new Vector3((float)slider_position,1,1));
        }
        
        void OnDestroy()
        {
            if(!is_initiated) return;
            
            //_inputSystem.leftLowerButtonPressedChanged -= OnPressedLeft;
            //_inputSystem.rightLowerButtonPressedChanged -= OnPressedRight;
            
            _inputSystem.leftTriggerPressedChanged -= OnPressedLeft;
            _inputSystem.rightTriggerPressedChanged -= OnPressedRight;
        }

        void OnPressedLeft(bool is_pressed)
        {
            OnPressed(_leftHandTriggerCollider, is_pressed);
        }
        
        void OnPressedRight(bool is_pressed)
        {
            OnPressed(_rightHandTriggerCollider, is_pressed);
        }

        void OnPressed(GameObject hand, bool is_pressed)
        {
            if(is_pressed)
                StartDrag(hand);
            else
            {
                StopDrag(hand);
            }
        }

        void StartDrag(GameObject hand)
        {
            if(GrabbableObject.grabbableObjects.Any(x=>x.IsHovered && x.grabbedWith == GrabbableObject.GrabbedWith.Trigger)) return;
            
            if(is_dragged) return;
            
            if(!hovered_by.Contains(hand)) return;
            
            is_dragged = true;

            dragged_by = hand.transform;

            StartDrag_Action();
        }

        void StopDrag(GameObject hand)
        {
            if(!is_dragged) return;
            
            if(dragged_by != hand.transform) return;
            
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
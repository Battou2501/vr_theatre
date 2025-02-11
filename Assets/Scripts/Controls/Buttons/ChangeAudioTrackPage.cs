using System;
using UnityEditor;
using UnityEngine;

namespace DefaultNamespace
{
    public class ChangeAudioTrackPage : ClickableButton
    {
        public event Action Clicked;
    
        protected override void Click_Action()
        {
            Debug.Log("Flipping");
            Clicked?.Invoke();
        }
#if UNITY_EDITOR
        [CustomEditor(typeof(ChangeAudioTrackPage))]
        public class ChangeAudioTrackPageEditor : ClickableButtonEditor {}
#endif
    }
}
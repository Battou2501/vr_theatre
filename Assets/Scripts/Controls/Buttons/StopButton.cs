using UnityEditor;
using UnityEngine;

namespace DefaultNamespace
{
    public class StopButton : ClickableButton
    {
        protected override void Click_Action()
        {
            video_manager.request_stop();
        }
        
        [CustomEditor(typeof(StopButton))]
        public class StopButtonEditor : ClickableButtonEditor {}
    }
}
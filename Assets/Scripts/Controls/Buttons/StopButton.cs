
using Zenject;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DefaultNamespace
{
    public class StopButton : ClickableButton
    {
        
        ManageVideoPlayerAudio video_manager;
        
        [Inject]
        public void Construct(ManageVideoPlayerAudio v)
        {
            video_manager = v;
        }
        
        protected override void Click_Action()
        {
            video_manager.request_stop();
        }
#if UNITY_EDITOR
        [CustomEditor(typeof(StopButton))]
        public class StopButtonEditor : ClickableButtonEditor {}
#endif
    }
}
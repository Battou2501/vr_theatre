
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DefaultNamespace
{
    public class SetAudioTrackButton : ClickableButton
    {
        public TMP_Text text;
        
        int track_idx;

        public void set_track_dta(int idx, string lang)
        {
            track_idx = idx;
            text.text = $"Track {idx} [{lang}]";
        }

        protected override void Click_Action()
        {
            video_manager.request_track_change(track_idx);
        }
#if UNITY_EDITOR
        [CustomEditor(typeof(SetAudioTrackButton))]
        public class SetAudioTrackButtonEditor : ClickableButtonEditor {}
#endif
    }
}
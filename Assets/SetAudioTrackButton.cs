using UnityEngine;

namespace DefaultNamespace
{
    public class SetAudioTrackButton : MonoBehaviour
    {
        int track_idx;
        PlayerPanel panel;
        
        public void init(PlayerPanel p)
        {
            panel = p;
        }

        public void Click()
        {
            panel.set_audio_track(track_idx);
        }
    }
}
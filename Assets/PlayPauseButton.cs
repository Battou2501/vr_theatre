using UnityEngine;

namespace DefaultNamespace
{
    public class PlayPauseButton : MonoBehaviour
    {
        PlayerPanel panel;
        
        public void init(PlayerPanel p)
        {
            panel = p;
        }

        public void Click()
        {
            panel.play_pause();
        }
    }
}
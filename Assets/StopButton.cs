using UnityEngine;

namespace DefaultNamespace
{
    public class StopButton : MonoBehaviour
    {
        PlayerPanel panel;
        
        public void init(PlayerPanel p)
        {
            panel = p;
        }

        public void Click()
        {
            panel.stop();
        }
    }
}
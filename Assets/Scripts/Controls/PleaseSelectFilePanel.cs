using UnityEngine;

namespace DefaultNamespace
{
    public class PleaseSelectFilePanel : MonoBehaviour
    {
        public OpenFileSelectPanelButton openFileSelectPanelButton;

        PlayerPanel player_panel;

        public void init(PlayerPanel p)
        {
            player_panel = p;
            openFileSelectPanelButton.init(player_panel);
        }
        
        public void show_panel()
        {
            gameObject.SetActive(true);
        }

        public void close_panel()
        {
            gameObject.SetActive(false);
        }
    }
}
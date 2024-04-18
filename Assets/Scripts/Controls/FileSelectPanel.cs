using UnityEngine;

namespace DefaultNamespace
{
    public class FileSelectPanel : InterfacePanel
    {
        MainControls main_controls;
        ManageVideoPlayerAudio video_manager;
        
        public void init(ManageVideoPlayerAudio m, MainControls c)
        {
            video_manager = m;
            main_controls = c;
            camera_transform = main_controls.cameraTransform;
        }
    }
}
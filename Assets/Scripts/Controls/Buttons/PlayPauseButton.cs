#if UNITY_EDITOR
using UnityEditor;
#endif
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features;
using UnityEngine.XR.OpenXR.Input;
using Zenject;

namespace DefaultNamespace
{
    public class PlayPauseButton : ClickableButton
    {
        enum MeshTypes
        {
            Play = 0,
            Pause
        }
        
        //public GameObject playButtonObject;
        //public GameObject pauseButtonObject;
        
        public Mesh playButtonMesh;
        public Mesh pauseButtonMesh;
        
        ManageVideoPlayerAudio video_manager;
        
        MeshFilter mesh_filter;
        
        MeshTypes current_mesh_type;
        
        [Inject]
        public void Construct(ManageVideoPlayerAudio v)
        {
            video_manager = v;
        }

        public override void init()
        {
            base.init();
            
            video_manager.PlayerStateChanged += OnPlayerStateChanged;
            
            mesh_filter = gameObject.GetComponent<MeshFilter>();
            
            if(mesh_filter == null) return;
            
            mesh_filter.mesh = playButtonMesh;
            current_mesh_type = MeshTypes.Play;
            
            change_mesh_type();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        
            if(!is_initiated) return;
            
            video_manager.PlayerStateChanged -= OnPlayerStateChanged;
        }
        
        void OnPlayerStateChanged()
        {
            change_mesh_type();
        }

        protected override void Click_Action()
        {
            video_manager.request_play_pause();
        }

        void change_mesh_type()
        {
            if(!is_initiated) return;
            
            if(mesh_filter == null) return;
            if(playButtonMesh == null) return;
            if(pauseButtonMesh == null) return;

            switch (video_manager.player_state)
            {
                case ManageVideoPlayerAudio.PlayerStates.playing when current_mesh_type != MeshTypes.Pause:
                    mesh_filter.mesh = pauseButtonMesh;
                    current_mesh_type = MeshTypes.Pause;
                    break;
                case ManageVideoPlayerAudio.PlayerStates.paused when current_mesh_type != MeshTypes.Play:
                case ManageVideoPlayerAudio.PlayerStates.stopped when current_mesh_type != MeshTypes.Play:
                    mesh_filter.mesh = playButtonMesh;
                    current_mesh_type = MeshTypes.Play;
                    break;
            }

            //if(playButtonObject != null && playButtonObject.activeSelf == video_manager.IsPlaying)
            //    playButtonObject.SetActive(!video_manager.IsPlaying);
            //
            //if(pauseButtonObject != null && pauseButtonObject.activeSelf != video_manager.IsPlaying)
            //    pauseButtonObject.SetActive(video_manager.IsPlaying);
        }
#if UNITY_EDITOR
        [CustomEditor(typeof(PlayPauseButton))]
        public class PlayPauseButtonEditor : ClickableButtonEditor {}
#endif
    }
}
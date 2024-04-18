using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class PlayerPanel : InterfacePanel
    {
        public PlayPauseButton playPauseButton;
        public StopButton stopButton;
        public OpenFileSelectPanelButton openFileSelectPanelButton;
        public OpenAudioTrackPanelButton openAudioTrackPanelButton;
        public VideoSeekHandle videoSeekHandle;
        public VolumeSetHandle volumeSetHandle;
        public ClosePlayerPanelButton closePlayerPanelButton;
        public CloseAudioTrackPanelButton closeAudioTrackPanelButton;
        public AudioTrackPanel audioTrackPanel;
        
        MainControls main_controls;
        ManageVideoPlayerAudio video_manager;

        public double Video_length => video_manager.Video_length;
        public double Video_time => video_manager.Video_time;
        public float Video_time_01 => (float)(Video_time/Video_length);
        public float Audio_volume => video_manager.Audio_volume;
        public Transform active_hand_transform => main_controls.Active_hand_transform;
        
        public void init(ManageVideoPlayerAudio m, MainControls c)
        {
            video_manager = m;
            main_controls = c;
            camera_transform = main_controls.cameraTransform;
            
            
            playPauseButton.real_null()?.init(this);
            stopButton.real_null()?.init(this);
            openFileSelectPanelButton.real_null()?.init(this);
            openAudioTrackPanelButton.real_null()?.init(this);
            videoSeekHandle.real_null()?.init(this);
            volumeSetHandle.real_null()?.init(this);
            closePlayerPanelButton.real_null()?.init(this);
            closeAudioTrackPanelButton.real_null()?.init(this);
            audioTrackPanel.real_null()?.init(this, main_controls.videoManager);
        }

        void deactivate_controls()
        {
            playPauseButton.real_null()?.set_interactable(false);
            stopButton.real_null()?.set_interactable(false);
            openFileSelectPanelButton.real_null()?.set_interactable(false);
            openAudioTrackPanelButton.real_null()?.set_interactable(false);
            videoSeekHandle.real_null()?.set_interactable(false);
            volumeSetHandle.real_null()?.set_interactable(false);
            closePlayerPanelButton.real_null()?.set_interactable(false);
        }
        
        void activate_controls()
        {
            playPauseButton.real_null()?.set_interactable(true);
            stopButton.real_null()?.set_interactable(true);
            openFileSelectPanelButton.real_null()?.set_interactable(true);
            openAudioTrackPanelButton.real_null()?.set_interactable(true);
            videoSeekHandle.real_null()?.set_interactable(true);
            volumeSetHandle.real_null()?.set_interactable(true);
            closePlayerPanelButton.real_null()?.set_interactable(true);
        }
        
        public void play_pause()
        {
            video_manager.pause(!video_manager.Vp_is_playing);
        }

        public void stop()
        {
            video_manager.stop();
        }

        public void set_time(double time)
        {
            video_manager.skip_to_time(time);
        }
        
        public void skip(float time)
        {
            video_manager.skip(time);
        }

        public void set_volume(float v)
        {
            video_manager.set_volume(v);
        }
        
        public void show_light_settings()
        {
            
        }

        public void show_audio_track_panel()
        {
            if(!video_manager.Vp_is_prepared) return;
            
            deactivate_controls();
            
            audioTrackPanel.show_panel();
        }
        
        public void close_audio_track_panel()
        {
            if(!video_manager.Vp_is_prepared) return;
            
            activate_controls();
            
            audioTrackPanel.close_panel();
        }

        public void set_audio_track(int t)
        {
            video_manager.request_track_change(t);
        }

        public void close_panel()
        {
            main_controls.hide_interface();
        }
    }
}
using UnityEngine;

namespace DefaultNamespace
{
    public class PlayerPanel : MonoBehaviour
    {
        MainControls main_controls;
        ManageVideoPlayerAudio video_manager;

        public double Video_length => video_manager.Video_length;
        public double Video_time => video_manager.Video_time;
        public float Video_time_01 => (float)(Video_time/Video_length);
        public float Audio_volume => video_manager.Audio_volume;
        public Transform active_hand_transform => main_controls.active_hand_transform;
        
        public void init(ManageVideoPlayerAudio m, MainControls c)
        {
            video_manager = m;
            main_controls = c;

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

        public void set_audio_track(int t)
        {
            video_manager.request_track_change(t);
        }
        
    }
}
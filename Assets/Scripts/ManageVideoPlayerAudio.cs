using System;
using System.Collections.Generic;
using System.Threading;
using NReco.VideoConverter;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.Audio;
using UnityEngine.Experimental.Video;
using UnityEngine.Video;
using Debug = UnityEngine.Debug;

namespace DefaultNamespace
{
    [RequireComponent(typeof(VideoPlayer))]
    public class ManageVideoPlayerAudio : MonoBehaviour
    {
        public enum PlayerCommands
        {
            none,
            play_pause,
            skip_forward,
            skip_backwards,
            skip_to_time,
            stop
        }

        enum PlayerStates
        {
            stopped,
            playing,
            paused
        }
        
        public AudioSource[] audioSources;
        public Material mat;
        public Material blitMat;
        public RenderTexture lightRT;
        public Transform[] screenLightSamplePoints;
        public bool lightAffectsScreen;
        public int buffer_iterations = 1;
        public int frame_buffer_size = 240;
        public int trimSeconds;
        public int trimAfterReachingSeconds;
        [Range(0,1)]
        public float movie_light_strength = 0.2f;
        public bool loop_video;
        public double skipSeconds;
        
        public Track[] tracks
        {
            get;
            private set;
        }
        public bool Vp_is_prepared => vp.isPrepared && vp.url != "";
        public bool Vp_file_selected => vp.url != "";
        public double Video_length => vp.length;
        public double Video_time => vp.time;
        public float Audio_volume => audioSources[0].volume;
        
        
        
        PlayerCommands requested_command; 
        PlayerStates player_state;
        AudioSampleProvider[] providers;
        bool light_affects_screen_current;
        int iterations;
        bool[] track_data_available;
        bool audio_started;
        bool no_audio;
        bool samples_received;
        double samples_receive_time;
        VideoPlayer vp;
        RenderTexture[] rt_pool;
        float frame_rate;
        int frame_to_play_idx;
        int render_pool_idx;
        bool is_delay_set;
        int delay_frames;
        int current_track_idx;
        bool set_preview_frame;
        int preview_target_index;
        int frames_til_target;
        bool seek_complete;
        float light_strength;
        bool is_prepared;
        int requested_track_idx = -1;
        bool adding_samples;
        double skip_time_target;

        bool Vp_is_playing => vp.isPlaying;
        bool tracks_in_sync
        {
            get
            {
                if (tracks is not {Length: > 0})
                    return true;

                var add_count = tracks[0].add_count;

                for (int i = 1; i < tracks.Length; i++)
                {
                    if (tracks[i].add_count != add_count)
                        return false;
                }

                return true;
            }
        }

        static int trim_seconds_static;
        static int trim_after_reaching_seconds_static;
        static double delay;
        static int sample_time;
        
        static readonly int aspect = Shader.PropertyToID("_Aspect");
        static readonly int movie_tex = Shader.PropertyToID("_MovieTex");
        static readonly int l_strength = Shader.PropertyToID("_LightStrength");
        static readonly int affected_by_light = Shader.PropertyToID("_AffectedByLight");
        static readonly int vec_arr_x = Shader.PropertyToID("_VecArrX");
        static readonly int vec_arr_y = Shader.PropertyToID("_VecArrY");
        static readonly int vec_arr_z = Shader.PropertyToID("_VecArrZ");
        
        void Start()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 900;
            
            trim_seconds_static = trimSeconds;
            trim_after_reaching_seconds_static = trimAfterReachingSeconds;

            vp = GetComponent<VideoPlayer>();
            vp.audioOutputMode = VideoAudioOutputMode.APIOnly;
            vp.prepareCompleted += Prepared;
            vp.frameReady += VpOnFrameReady;
            vp.sendFrameReadyEvents = true;
            vp.seekCompleted += VpOnSeekCompleted;
            vp.loopPointReached+= VpOnLoopPointReached;
            vp.playbackSpeed = buffer_iterations;
            vp.frameDropped+= VpOnFrameDropped;


            var points_x = new float[60];
            var points_y = new float[60];
            var points_z = new float[60];
            
            for (var i = 0; i < screenLightSamplePoints.Length; i++)
            {
                points_x[i] = screenLightSamplePoints[i].position.x;
                points_y[i] = screenLightSamplePoints[i].position.y;
                points_z[i] = screenLightSamplePoints[i].position.z;
            }
            
            Shader.SetGlobalFloatArray(vec_arr_x, points_x);
            Shader.SetGlobalFloatArray(vec_arr_y, points_y);
            Shader.SetGlobalFloatArray(vec_arr_z, points_z);

            light_strength = 1;
            
            Shader.SetGlobalFloat(l_strength, light_strength);
            Shader.SetGlobalInt(affected_by_light, lightAffectsScreen? 1 : 0);
            
            if(vp.url != "")
                vp.Prepare();

            player_state = PlayerStates.stopped;
        }
        
        void Update()
        {
            handle_change_track_request();

            update_light();
            
            update_light_affects_screen();

            check_input();
            
            handle_audio_data_available();
            
            handle_player_command_request();

            //Getting last audio samples received time to use this data in input checks and prevent input handling
            //right before next audio samples batch is ready (Unity bug that can get audio samples
            //while video is already paused, but cant consume those samples until video is playing 
            if (samples_received)
            {
                samples_received = false;
                samples_receive_time = Time.unscaledTimeAsDouble;
            }
        }

        static void VpOnFrameDropped(VideoPlayer source)
        {
            Debug.LogWarning("SKIPPED!!!!");
        }
        
        void VpOnLoopPointReached(VideoPlayer source)
        {
            reset_state();
            
            vp.Stop();

            player_state = PlayerStates.stopped;
            
            if(!loop_video) return;

            player_state = PlayerStates.playing;
            
            vp.Play();
        }

        void VpOnSeekCompleted(VideoPlayer source)
        {
            seek_complete = true;
        }
        
        void VpOnFrameReady(VideoPlayer source, long frameIdx)
        {
            Get2DTexture(vp, render_pool_idx);
            
            if (set_preview_frame && seek_complete)
            {
                render_frame_to_screen(render_pool_idx);
                
                Debug.Log("Preview ready");
                
                set_preview_frame = false;
                seek_complete = false;
            }

            render_pool_idx += 1;
            
            if (render_pool_idx >= rt_pool.Length)
                render_pool_idx = 0;
            
            if (!audio_started && !no_audio) return;
            
            render_frame_to_screen(frame_to_play_idx);
                
            frame_to_play_idx += 1;
            
            if (frame_to_play_idx >= rt_pool.Length)
                frame_to_play_idx = 0;
        }

        void render_frame_to_screen(int idx)
        {
            mat.SetTexture(movie_tex, rt_pool[idx]);
            Graphics.Blit(rt_pool[idx], lightRT, blitMat);
        }
        
        void Get2DTexture(VideoPlayer vp, int i)
        {
            Graphics.Blit(vp.texture, rt_pool[i]);
        }

        static void ProviderOnSampleFramesOverflow(AudioSampleProvider provider, uint sampleFrameCount)
        {
            Debug.Log("Track " + provider.trackIndex + " sample frames overflow");
        }
        
        void SampleFramesAvailable(AudioSampleProvider provider, uint sampleFrameCount)
        {
            using var buffer = new NativeArray<float>((int)sampleFrameCount * provider.channelCount, Allocator.Temp);

            var sf_count = provider.ConsumeSampleFrames(buffer);

            //Handling situation when samples ready callback called after video player already have been stopped or paused
            //Unity bug prevents samples consumption if video player is not playing
            if (sf_count != sampleFrameCount)
            {
                Debug.Log("Not all sample frames consumed!");
                
                while (sf_count!=sampleFrameCount)
                { 
                    sf_count = provider.ConsumeSampleFrames(buffer);
                }
                   
                Debug.Log("Finally all sample frames consumed!");
            }

            var track = tracks[provider.trackIndex];
            
            track.add_samples(sf_count, buffer, provider.channelCount);

            track_data_available[provider.trackIndex] = true;

            if (tracks_in_sync)
                samples_received = true;
            
            if(provider.trackIndex != current_track_idx) return;

            if (is_delay_set) return;
            
            if(iterations<buffer_iterations)
                iterations += 1;

            if (iterations == buffer_iterations)
                set_delay();
            
        }
        
        void request_preview()
        {
            set_preview_frame = true;
        }

        void reset_state()
        {
            sample_time = 0;

            tracks?.for_each(x=>x.clear_data());
            
            audioSources?.for_each(x => { x.clip = null; x.Stop(); } );
            
            reset_audio_data_ready_flags();

            is_delay_set = false;
            audio_started = false;
            iterations = 0;
            render_pool_idx = 0;
            frame_to_play_idx = 0;
            delay_frames = 0;
            delay = 0;
        }

        public void request_pause()
        {
            requested_command = PlayerCommands.play_pause;
        }
        
        void pause()
        {
            if(!is_prepared) return;
            
            if(vp.url is null or "") return;

            var state = Vp_is_playing;
            
            if(state && !audio_started && !no_audio) return;

            if (state)
            {
                vp.Pause();
                player_state = PlayerStates.paused;
                
                if(no_audio) return;
                
                tracks[current_track_idx].channels.for_each(x=>x?.audio_source.Pause());
                
                return;
            }
            
            vp.Play();
            player_state = PlayerStates.playing;
            
            if(no_audio) return;
            
            tracks[current_track_idx].channels.for_each(x=>x?.audio_source.Play());
        }

        public void stop()
        {
            reset_state();
            
            vp.Stop();
            player_state = PlayerStates.stopped;
        }

        public void request_skip_to_time(double t)
        {
            skip_time_target = t;
            requested_command = PlayerCommands.skip_to_time;
        }

        public void request_skip_forward()
        {
            requested_command = PlayerCommands.skip_forward;
        }
        
        public void request_skip_backwards()
        {
            requested_command = PlayerCommands.skip_backwards;
        }

        void skip()
        {
            if(!is_prepared) return;
            
            if(!audio_started && !no_audio) return;
            
            request_preview();

            if (!no_audio)
                vp.playbackSpeed = buffer_iterations;


            switch (requested_command)
            {
                case PlayerCommands.skip_forward:
                    vp.time += skipSeconds;
                    break;
                case PlayerCommands.skip_backwards:
                    vp.time -= skipSeconds;
                    break;
                case PlayerCommands.skip_to_time:
                    vp.time = skip_time_target;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            reset_state();
        }

        public void set_volume(float volume)
        {
            audioSources.for_each(volume, (x,v) => x.volume = v);
        }
        
        void reset_audio_data_ready_flags()
        {
            if(track_data_available == null) return;
            
            for (var i = 0; i < track_data_available.Length; i++)
            {
                track_data_available[i] = false;
            }
        }

        void update_light()
        {
            var target_light = player_state == PlayerStates.playing ? movie_light_strength : 1;

            if (!(Mathf.Abs(light_strength - target_light) > 0.01f)) return;
            
            light_strength = Mathf.Lerp(light_strength, target_light,Time.deltaTime);
            
            Shader.SetGlobalFloat(l_strength, light_strength);
        }

        void update_light_affects_screen()
        {
            if (lightAffectsScreen == light_affects_screen_current) return;
            
            light_affects_screen_current = lightAffectsScreen;
            
            Shader.SetGlobalInt(affected_by_light, lightAffectsScreen? 1 : 0);
        }

        //Input from keyboard for test purpose
        void check_input()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
                request_track_change(0);
            
            if (Input.GetKeyDown(KeyCode.Alpha2))
                request_track_change(1);
            
            if (Input.GetKeyDown(KeyCode.Alpha3))
                request_track_change(2);
            
            if (Input.GetKeyDown(KeyCode.Alpha4))
                request_track_change(3);
            
            if (Input.GetKeyDown(KeyCode.Alpha5))
                request_track_change(4);
            
            if (Input.GetKeyDown(KeyCode.Alpha6))
                request_track_change(5);

            if (Input.GetKeyDown(KeyCode.RightArrow))
                requested_command = PlayerCommands.skip_forward;

            if (Input.GetKeyDown(KeyCode.LeftArrow))
                requested_command = PlayerCommands.skip_backwards;

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                request_preview();
                
                if(!no_audio)
                    vp.playbackSpeed = buffer_iterations;
                
                vp.time = vp.length / 2f;

                reset_state();
            }
            
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                request_skip_to_time(15);
            }

            if (Input.GetKeyDown(KeyCode.Space))
                requested_command = PlayerCommands.play_pause;
        }

        void handle_player_command_request()
        {
            if (requested_command == PlayerCommands.none)
                return;

            if (!tracks_in_sync)
                return;
            
            if(player_state == PlayerStates.playing && Time.unscaledTimeAsDouble - samples_receive_time > 0.3f)
                return;
            
            if(!no_audio && !audio_started && player_state == PlayerStates.playing)
                return;

            switch (requested_command)
            {
                case PlayerCommands.play_pause:
                    pause();
                    break;
                case PlayerCommands.skip_forward:
                case PlayerCommands.skip_backwards:
                case PlayerCommands.skip_to_time:
                    skip();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            requested_command = PlayerCommands.none;
        }
        
        void handle_audio_data_available()
        {
            if(track_data_available == null || track_data_available.Length<=current_track_idx || !track_data_available[current_track_idx] || iterations<buffer_iterations || no_audio) return;

            if(!no_audio && !tracks_in_sync) return;
            
            reset_audio_data_ready_flags();

            handle_audio_started();
            
            update_track_data();

            set_correct_frame();

            return;
            
            void update_track_data()
            {
                if(audioSources[0].isPlaying)
                    sample_time = audioSources[0].timeSamples;

                if (sample_time > tracks[current_track_idx].trim_threshold_samples)
                    tracks.for_each(x => x?.trim());
                
                tracks[current_track_idx].update_data_track();
            }

            void handle_audio_started()
            {
                if (audio_started) return;
                
                tracks[current_track_idx].channels.for_each(x=>x?.set_clip(0));
                
                audio_started = true;

                vp.playbackSpeed = 1;
            }
            
            void set_correct_frame()
            {
                frame_to_play_idx = render_pool_idx - delay_frames;

                if (frame_to_play_idx >= 0) return;
                
                frame_to_play_idx = rt_pool.Length + frame_to_play_idx;
            }
        }

        void Prepared(VideoPlayer vp)
        {
            if(rt_pool == null || rt_pool.Length<frame_buffer_size)
                rt_pool = new RenderTexture[frame_buffer_size];

            var w = (int) vp.width;
            var h = (int) vp.height;
            
            Shader.SetGlobalFloat(aspect, (float)w/h);

            for (var i = 0; i < frame_buffer_size; i++)
            {
                Destroy(rt_pool[i]);

                rt_pool[i] = new RenderTexture(w, h, 0, RenderTextureFormat.Default,0);
            }

            frame_rate = vp.frameRate;
            
            is_prepared = true;

            render_frame_to_screen(0);

            no_audio = vp.audioTrackCount == 0;
            
            reset_state();
            
            if (!no_audio)
                vp.playbackSpeed = buffer_iterations;
            else
                set_delay();
            
            prepare_tracks();
        }

        void prepare_tracks()
        {
            if(no_audio) return;
            
            var audioTrackCount = vp.audioTrackCount;
            tracks?.for_each( x=>x?.clear());
            tracks = new Track[audioTrackCount];
            track_data_available = new bool[audioTrackCount];
            
            for (var t = 0; t < vp.audioTrackCount; t++)
            {
                var track = new Track();

                track.init(vp.GetAudioSampleProvider((ushort)t), audioSources, SampleFramesAvailable, ProviderOnSampleFramesOverflow);

                track.lang = vp.GetAudioLanguageCode((ushort)t);
                
                tracks[t] = track;
            }
        }

        public void request_track_change(int idx)
        {
            if(no_audio) return;
            
            requested_track_idx = idx;
        }
        
        void handle_change_track_request()
        {
            if(!audio_started || !tracks_in_sync || !Vp_is_playing) return;
            
            if(requested_track_idx == -1 || requested_track_idx >= vp.audioTrackCount || requested_track_idx == current_track_idx ) return;

            current_track_idx = requested_track_idx;

            requested_track_idx = -1;

            var audio_source_time = audioSources[0].time;
            
            audioSources.for_each(x=>x.Stop());
            
            tracks[current_track_idx].set_clip(audio_source_time);
        }

        void set_delay()
        {
            if (is_delay_set) return;
                
            delay = render_pool_idx / frame_rate;
                
            delay_frames = Mathf.FloorToInt(frame_rate * (float) delay);
                
            is_delay_set = true;
        }
        
        public class Track
        {
            public Channel[] channels;
            public uint add_count;
            public int trim_threshold_samples;
            public string lang;
            
            AudioSampleProvider provider; 
            int trim_seconds_samples;

            public void clear()
            {
                channels.for_each(x=>x?.clear());
            }
            
            public void init(AudioSampleProvider p, AudioSource[] audioSources, AudioSampleProvider.SampleFramesHandler samples_available_callback, AudioSampleProvider.SampleFramesHandler samples_overflow_callback)
            {
                provider = p;
                channels = new Channel[provider.channelCount];
                trim_seconds_samples = trim_seconds_static * (int)provider.sampleRate;
                trim_threshold_samples = trim_after_reaching_seconds_static * (int)provider.sampleRate;
                
                provider.sampleFramesOverflow += samples_overflow_callback;
                provider.sampleFramesAvailable += samples_available_callback;
                provider.enableSampleFramesAvailableEvents = true;
                provider.freeSampleFrameCountLowThreshold = provider.maxSampleFrameCount / 3 * 2;

                var channel_count = provider.channelCount;
                var audio_source_idx = 0;

                for (var i = 0; i < channel_count; i++)
                {
                    var channel = new Channel();
                    
                    channels[i] = channel;
                    
                    channel.init(audioSources[(channel_count == 6 && i == 3) ? 5 : audio_source_idx], provider, this);
                    
                    if (channel_count == 6 && i == 3)
                        continue;
                    
                    audio_source_idx += 1;
                }
            }
            
            public void clear_data()
            {
                channels.for_each(x=>x?.clear_data());
                add_count = 0;
            }

            public void set_clip(float t)
            {
                channels.for_each(t, (x,d) => x?.set_clip(d));
            }
            
            public void add_samples(uint sfCount, NativeArray<float> buffer, int channelCount)
            {
                add_count += 1;

                if(channelCount*sfCount>buffer.Length)
                    return;
                
                var b_length = buffer.Length;
                var buff = new float[sfCount];
                var c = 0;
                var j = 0;
                
                for (var i = 0; i < b_length; i+=1)
                {
                    buff[j] = buffer[c + j * channelCount];

                    j++;

                    if (j < sfCount) continue;

                    channels[c].add_data(buff);
                    
                    j = 0;
                    c++;
                }
            }

            public void update_data_track()
            {
                channels.for_each(x => x?.set_clip_data());

                if(sample_time <= trim_threshold_samples) return;
                
                channels.for_each(trim_seconds_samples, (x,t)=>x?.trim_audio_source(t));
            }

            public void trim()
            {
                channels.for_each(trim_seconds_samples, (x, t)=>x?.trim(t));
            }
        }

        public class Channel
        {
            public AudioSource audio_source;
            
            AudioClip audio_clip;
            int sample_rate;
            List<float> data;
            List<float> play_data;
            bool need_trim;
            int sample_length;

            public void clear()
            {
                audio_source.clip = null;
                Destroy(audio_clip);
                data.Clear();
                play_data.Clear();
            }
            
            public void set_clip(float t)
            {
                set_clip_data();
                audio_source.clip = audio_clip;
                audio_source.Play();
                audio_source.time = t;
            }
            
            public void init(AudioSource s, AudioSampleProvider p, Track t)
            {
                audio_source = s;

                sample_rate = (int) p.sampleRate;

                sample_length = 30 * sample_rate;
                
                audio_clip = AudioClip.Create("", sample_length, 1, sample_rate, false, null);

                data = new List<float>();
                play_data = new List<float>(sample_length);
            }

            public void add_data(IEnumerable<float> d)
            {
                data.AddRange(d);
            }

            public void trim(int trim_seconds_samples)
            {
                data.RemoveRange(0, trim_seconds_samples);
            }

            public void trim_audio_source(int trim_seconds_samples)
            {
                audio_source.timeSamples -= trim_seconds_samples;
            }
            
            public void set_clip_data()
            {
                play_data.Clear();
                play_data.AddRange(data);
                audio_clip.SetData(play_data.ToArray(), 0);
            }

            public void clear_data()
            {
                data.Clear();
            }
            
        }
    }
}
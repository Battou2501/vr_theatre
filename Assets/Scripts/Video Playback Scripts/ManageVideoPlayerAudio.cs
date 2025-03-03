using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using MediaInfoLib;
using NReco.VideoConverter;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Audio;
using UnityEngine.Experimental.Video;
using UnityEngine.Serialization;
using UnityEngine.Video;
using Zenject;
using Debug = UnityEngine.Debug;

namespace DefaultNamespace
{
    [RequireComponent(typeof(VideoPlayer))]
    public class ManageVideoPlayerAudio : MonoBehaviour
    {
        enum PlayerCommands
        {
            none,
            play_pause,
            skip_forward,
            skip_backwards,
            skip_to_time,
            stop
        }

        public enum PlayerStates
        {
            stopped,
            playing,
            paused
        }

        public enum StereoTypes
        {
            Flat = 0,
            halfSBS,
            fullSBS,
            halfOU,
            fullOU
        }

        public event Action AudioTrackChanged;
        public event Action VideoPrepared;
        public event Action<string> ErrorOccured;
        public event Action PlayerStateChanged;
        public event Action VideoEnded;

        public event Action StereoTypeChanged;
        
        public AudioSource[] audioSources;
        public Material mat;
        public Material blitMat;
        public Material pixelAspectFixMat;
        public RenderTexture lightRT;
        public Transform[] screenLightSamplePoints;
        public Transform screenPointsRoot;
        public bool lightAffectsScreen;
        public int buffer_iterations = 1;
        public int frame_buffer_seconds = 2;
        public int trimSeconds;
        public int trimAfterReachingSeconds;
        //public int trackAudioClipBufferLengthSec;
        public float videoAudioDesyncThreshold;
        public bool sync_audio_if_needed;
        [Range(0,1)]
        public float movie_light_strength = 0.2f;
        public bool loop_video;
        public int maxMovieTextureResolution = 1024;
        double skip_seconds = 10;
        [SerializeField]
        private StereoTypes _stereoType;
        public bool invertStereoOrder;
        [HideInInspector]
        public double seek_time;
        [HideInInspector]
        public bool is_seeking;
        [HideInInspector]
        public PlayerStates player_state;
        
        public Track[] tracks
        {
            get;
            private set;
        }
        public bool Vp_is_prepared => vp.isPrepared && vp.url != "";
        public bool Vp_file_selected => vp.url != "";
        public double Video_length => vp.length;
        public double Video_time => vp.time;
        public float Video_time_01 => Vp_is_prepared ? (float)(Video_time/Video_length) : 0;
        public float Audio_volume => audioSources[0].volume;
        public string FilePath => vp.url;
        public bool IsPlaying => vp.isPlaying;
        public int CurrentTrackNumber => tracks != null ? current_track_idx : -1;
        public string CurrentTrackLang => tracks != null && current_track_idx >= 0 && current_track_idx < tracks.Length ? tracks[current_track_idx].lang : "";

        public RenderTexture rtttt;
        
        
        PlayerCommands requested_command; 
        AudioSampleProvider[] providers;
        bool light_affects_screen_current;
        int iterations;
        bool audio_started;
        bool no_audio;
        bool samples_received;
        double samples_receive_time;
        VideoPlayer vp;
        RenderTexture[] rt_pool;
        int frame_to_play_idx;
        int render_pool_idx;
        bool is_delay_set;
        bool is_delay_set_needed;
        int delay_frames;
        float delay_seconds;
        double play_start_time;
        int current_track_idx;
        bool set_preview_frame;
        bool seek_complete;
        float light_strength;
        bool is_prepared;
        int requested_track_idx = -1;
        double skip_time_target;
        double audio_trimmed_time;
        bool is_video_playing_for_other_threads;
        private MediaInfo media_info;
        
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
        static int sample_time;
        //static int track_audio_clip_buffer_length_sec_static;
        
        static readonly int aspect = Shader.PropertyToID("_Aspect");
        static readonly int one_over_aspect = Shader.PropertyToID("_OneOverAspect");
        static readonly int movie_tex = Shader.PropertyToID("_MovieTex");
        static readonly int l_strength = Shader.PropertyToID("_LightStrength");
        static readonly int affected_by_light = Shader.PropertyToID("_AffectedByLight");
        static readonly int vec_arr_x = Shader.PropertyToID("_VecArrX");
        static readonly int vec_arr_y = Shader.PropertyToID("_VecArrY");
        static readonly int vec_arr_z = Shader.PropertyToID("_VecArrZ");
        private static readonly int x_border_coef = Shader.PropertyToID("_xBorderCoef");
        private static readonly int y_border_coef = Shader.PropertyToID("_yBorderCoef");
        private static readonly int video_type = Shader.PropertyToID("_VideoType");
        private static readonly int pixel_aspect_ratio = Shader.PropertyToID("_PixelAspectRatio");
        private static readonly int aspect_type = Shader.PropertyToID("_AspectType");
        private static readonly int invert_order = Shader.PropertyToID("_InvertOrder");

        public void init()
        {
            media_info = new MediaInfo();
            trim_seconds_static = trimSeconds;
            trim_after_reaching_seconds_static = trimAfterReachingSeconds;
            //track_audio_clip_buffer_length_sec_static = trackAudioClipBufferLengthSec;

            vp = GetComponent<VideoPlayer>();
            vp.audioOutputMode = VideoAudioOutputMode.APIOnly;
            vp.prepareCompleted += Prepared;
            vp.frameReady += VpOnFrameReady;
            vp.sendFrameReadyEvents = true;
            vp.seekCompleted += VpOnSeekCompleted;
            vp.loopPointReached+= VpOnLoopPointReached;
            vp.playbackSpeed = buffer_iterations;
            vp.frameDropped+= VpOnFrameDropped;
            vp.errorReceived += VpOnErrorReceived;


            var points_x = new float[60];
            var points_y = new float[60];
            var points_z = new float[60];
            
            for (var i = 0; i < screenLightSamplePoints.Length; i++)
            {
                points_x[i] = screenLightSamplePoints[i].position.x;
                points_y[i] = screenLightSamplePoints[i].position.y;
                points_z[i] = screenLightSamplePoints[i].position.z;
            }
            
            Destroy(screenPointsRoot.gameObject);
            
            Shader.SetGlobalFloatArray(vec_arr_x, points_x);
            Shader.SetGlobalFloatArray(vec_arr_y, points_y);
            Shader.SetGlobalFloatArray(vec_arr_z, points_z);

            light_strength = 1;
            
            Shader.SetGlobalFloat(l_strength, light_strength);
            Shader.SetGlobalInt(affected_by_light, lightAffectsScreen? 1 : 0);

            if (vp.url != "")
            {
                set_file(vp.url);
            }

            change_player_state(PlayerStates.stopped);
        }

        void change_player_state(PlayerStates state)
        {
            player_state = state;
            
            PlayerStateChanged?.Invoke();
        }
        
        void OnDestroy()
        {
            is_video_playing_for_other_threads = false;
            
            tracks?.for_each(x=>x.Dispose());
            tracks = null;
            rt_pool.for_each(Destroy);
            rt_pool = null;
            
            if(vp == null) return;
            
            vp.prepareCompleted -= Prepared;
            vp.frameReady       -= VpOnFrameReady;
            vp.seekCompleted    -= VpOnSeekCompleted;
            vp.loopPointReached -= VpOnLoopPointReached;
            vp.frameDropped     -= VpOnFrameDropped;
            vp.errorReceived    -= VpOnErrorReceived;
        }

        void VpOnErrorReceived(VideoPlayer source, string message)
        {
            vp.url = "";
            reset_state();
            vp.Stop();
            change_player_state(PlayerStates.stopped);
            ErrorOccured?.Invoke(message);
        }

        void Update()
        {
            Shader.SetGlobalInt(invert_order, invertStereoOrder ? 1 : 0);
                
            update_video_type_shader_parameters();
            
            ProfilingUtility.BeginSample("Set Delay");
            set_delay();
            ProfilingUtility.EndSample();
            
            ProfilingUtility.BeginSample("Handle check track request");
            handle_change_track_request();
            ProfilingUtility.EndSample();

            ProfilingUtility.BeginSample("Update light");
            update_light();
            ProfilingUtility.EndSample();
            
            ProfilingUtility.BeginSample("Update light affect screen");
            update_light_affects_screen();
            ProfilingUtility.EndSample();

            ProfilingUtility.BeginSample("Check input");
            check_input();
            ProfilingUtility.EndSample();
            
            ProfilingUtility.BeginSample("Handle audio available");
            handle_audio_data_available();
            ProfilingUtility.EndSample();
            
            ProfilingUtility.BeginSample("Handle command request");
            handle_player_command_request();
            ProfilingUtility.EndSample();
        }

        void Prepared(VideoPlayer source)
        {
            var w = (int) source.width;
            var h = (int) source.height;

            var a = (float) w / (float) h;// * (float)source.pixelAspectRatioNumerator / source.pixelAspectRatioDenominator);
            
            Shader.SetGlobalFloat(aspect, a);
            Shader.SetGlobalFloat(one_over_aspect, 1f/a);
            update_video_type_shader_parameters();

            var frame_buffer_size = Mathf.FloorToInt(frame_buffer_seconds * source.frameRate);
            
            rt_pool?.for_each(Destroy);

            rt_pool = new RenderTexture[frame_buffer_size];

            //var h2 = (float)h * (float)source.pixelAspectRatioNumerator / source.pixelAspectRatioDenominator;

            set_pixel_aspect_type(source);
            
            var tex_width = w;
            var tex_height = h;

            if (tex_width > maxMovieTextureResolution || tex_height > maxMovieTextureResolution)
            {
                var max_height = maxMovieTextureResolution / 1.9f;
                var ratio_bigger = (float) w / h >= 1.0f;
                tex_width = ratio_bigger ? maxMovieTextureResolution : (int) (max_height * ((float) w / h));
                tex_height = !ratio_bigger ? (int) max_height : (int) (maxMovieTextureResolution * ((float) h / w));
            }


            for (var i = 0; i < frame_buffer_size; i++)
            {
                rt_pool[i] = new RenderTexture(tex_width, tex_height, 0, RenderTextureFormat.ARGB64,0);
            }

            is_prepared = true;

            render_frame_to_screen(0);

            no_audio = source.audioTrackCount == 0;
            
            reset_state();

            if (!no_audio)
                source.playbackSpeed = buffer_iterations;
            else
                is_delay_set_needed = true;
            
            prepare_tracks();
            
            VideoPrepared?.Invoke();
            
            current_track_idx = vp.audioTrackCount>0 ? 0 : -1;
            AudioTrackChanged?.Invoke();
            
            requested_command = PlayerCommands.play_pause;
        }
        
        static void VpOnFrameDropped(VideoPlayer source)
        {
            Debug.LogWarning("SKIPPED!!!!");
        }
        
        void VpOnLoopPointReached(VideoPlayer source)
        {
            vp.Play();
            
            request_stop();

            if (!loop_video)
            {
                VideoEnded?.Invoke();
                return;
            }

            change_player_state(PlayerStates.playing);
            
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

        public void SetStereoType(StereoTypes type)
        {
            _stereoType = type;
            StereoTypeChanged?.Invoke();
        }
        
        public StereoTypes GetStereoType()
        {
            return _stereoType;
        }
        
        void set_pixel_aspect_type(VideoPlayer source)
        {
            var pixelAspectRatio =  (float) source.pixelAspectRatioNumerator /(float)source.pixelAspectRatioDenominator;
            var a_type = pixelAspectRatio > 1.001f ? 1 : pixelAspectRatio < 0.999f ? -1 : 0;
            pixelAspectRatio = pixelAspectRatio > 1 ? 1.0f / pixelAspectRatio : pixelAspectRatio;
            Shader.SetGlobalFloat(pixel_aspect_ratio, pixelAspectRatio);
            Shader.SetGlobalFloat(aspect_type, a_type);
        }
        
        void update_video_type_shader_parameters()
        {
            var x_coef = _stereoType switch
            {
                StereoTypes.Flat => 1f,
                StereoTypes.halfOU => 1f,
                StereoTypes.fullOU => 1f,
                StereoTypes.halfSBS => 0.5f,
                StereoTypes.fullSBS => 0.5f
            };
            Shader.SetGlobalFloat(x_border_coef, x_coef );
            var y_coef = _stereoType switch
            {
                StereoTypes.Flat => 1f,
                StereoTypes.halfOU => 0.5f,
                StereoTypes.fullOU => 0.5f,
                StereoTypes.halfSBS => 1f,
                StereoTypes.fullSBS => 1f
            };
            Shader.SetGlobalFloat(y_border_coef, y_coef );
            Shader.SetGlobalInt(video_type, (int)_stereoType);
        }
        
        void render_frame_to_screen(int idx)
        { 
            mat.SetTexture(movie_tex, rt_pool[idx]);
            Graphics.Blit(rt_pool[idx], lightRT, blitMat);
            //Graphics.Blit(rt_pool[idx], lightRT);
        }

        void clear_screen()
        {
            mat.SetTexture(movie_tex, null);
            Graphics.Blit(null, lightRT, blitMat);
        }
        
        void Get2DTexture(VideoPlayer vp, int i)
        {
            Graphics.Blit(vp.texture, rt_pool[i], pixelAspectFixMat);
            Graphics.Blit(vp.texture, rtttt, pixelAspectFixMat);
        }

        static void ProviderOnSampleFramesOverflow(AudioSampleProvider provider, uint sampleFrameCount)
        {
            Debug.Log("Track " + provider.trackIndex + " sample frames overflow");
        }
        
        void SampleFramesAvailable(AudioSampleProvider provider, uint sampleFrameCount)
        {
            var track = tracks[provider.trackIndex];
            var channel_count = track.channels.Length;
            //using var buffer = new NativeArray<float>((int)sampleFrameCount * provider.channelCount, Allocator.Temp);
            var buffer = track.get_sized_track_buffer((int) sampleFrameCount * channel_count);

            var sf_count = provider.ConsumeSampleFrames(buffer);

            //Handling situation when samples ready callback called after video player already have been stopped or paused
            //Unity bug prevents samples consumption if video player is not playing
            if (sf_count != sampleFrameCount)
            {
                Debug.Log("Not all sample frames consumed!");
                
                while (sf_count!=sampleFrameCount)
                { 
                    if(!is_video_playing_for_other_threads)
                        return;
                    
                    sf_count = provider.ConsumeSampleFrames(buffer);
                }
                   
                Debug.Log("Finally all sample frames consumed!");
            }
            
            track.add_samples(sf_count, buffer, channel_count);

            if (tracks_in_sync)
                samples_received = true;
            
            //Debug.Log("Track " + provider.trackIndex + " samples received: " + sf_count+ " count: " + track.add_count);
            
            if(provider.trackIndex != current_track_idx) return;

            if (is_delay_set) return;
            
            if(iterations<buffer_iterations)
                iterations += 1;

            if (iterations == buffer_iterations)
                is_delay_set_needed = true;

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

            samples_received = false;
            samples_receive_time = 0;

            is_delay_set = false;
            is_delay_set_needed = false;
            audio_started = false;
            iterations = 0;
            render_pool_idx = 0;
            frame_to_play_idx = 0;
            delay_frames = 0;
            delay_seconds = 0;
            audio_trimmed_time = 0;
            play_start_time = 0;
            is_video_playing_for_other_threads = false;
        }

        public void set_file(string file)
        {
            if (!File.Exists(file))
            {
                ErrorOccured?.Invoke("File was not found. Please check file path.");
                return;
            }
            
            vp.Stop();
            change_player_state(PlayerStates.stopped);
            vp.url = file;
            vp.Prepare();
        }
        
        public void request_play_pause()
        {
            requested_command = PlayerCommands.play_pause;
        }
        
        void play_pause()
        {
            if(!is_prepared) return;
            
            if(vp.url is null or "") return;

            var state = Vp_is_playing;
            
            if(state && !audio_started && !no_audio) return;

            if (state)
            {
                vp.Pause();
                change_player_state(PlayerStates.paused);
                
                if(no_audio) return;
                
                tracks[current_track_idx].channels.for_each(x=>x?.audio_source.Pause());
                
                return;
            }
            
            vp.Play();
            change_player_state(PlayerStates.playing);
            
            if(no_audio) return;
            
            tracks[current_track_idx].channels.for_each(x=>x?.audio_source.Play());
        }

        public void request_stop()
        {
            requested_command = PlayerCommands.stop;
        }
        
        void stop()
        {
            if(!is_prepared) return;
            
            reset_state();
            
            vp.Pause();
            vp.time = 0;
            clear_screen();
            change_player_state(PlayerStates.stopped);
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
            
            if(IsPlaying && !audio_started && !no_audio) return;
            
            request_preview();

            if (!no_audio)
                vp.playbackSpeed = buffer_iterations;


            switch (requested_command)
            {
                case PlayerCommands.skip_forward:
                    vp.time += Mathf.Min((float)skip_seconds, Mathf.Max(0,(float)vp.length - (float)vp.time - 1));
                    break;
                case PlayerCommands.skip_backwards:
                    vp.time -= Mathf.Min((float)skip_seconds, (float)vp.time);
                    break;
                case PlayerCommands.skip_to_time:
                    vp.time = skip_time_target;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            reset_state();
            
            play_start_time = vp.time;
        }

        public void set_volume(float volume)
        {
            audioSources.for_each(volume, (x,v) => x.volume = v);
        }

        void update_light()
        {
            var target_light = player_state == PlayerStates.playing ? movie_light_strength : 1;

            if (!(Mathf.Abs(light_strength - target_light) > 0.0001f)) return;
            
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
                
                play_start_time = vp.time;
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
                    play_pause();
                    break;
                case PlayerCommands.skip_forward:
                case PlayerCommands.skip_backwards:
                case PlayerCommands.skip_to_time:
                    skip();
                    break;
                case PlayerCommands.stop:
                    stop();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            requested_command = PlayerCommands.none;
        }
        
        void handle_audio_data_available()
        {
            if (no_audio) return;

            if (!samples_received) return;
            
            samples_received = false;
            
            //Getting last audio samples received time to use this data in input checks and prevent input handling
            //right before next audio samples batch is ready (Unity bug that can get audio samples
            //while video is already paused, but cant consume those samples until video is playing 
            samples_receive_time = Time.unscaledTimeAsDouble;
            
            if (iterations<buffer_iterations) return;

            ProfilingUtility.BeginSample("Handle audio started");
            handle_audio_started();
            ProfilingUtility.EndSample();
            
            ProfilingUtility.BeginSample("Update track data");
            update_track_data();
            ProfilingUtility.EndSample();

            ProfilingUtility.BeginSample("Set correct frame");
            set_correct_frame();
            ProfilingUtility.EndSample();

            return;
            
            void update_track_data()
            {
                if(audioSources[0].isPlaying)
                    sample_time = audioSources[0].timeSamples;

                ProfilingUtility.BeginSample("Trim");
                if (sample_time > tracks[current_track_idx].trim_threshold_samples)
                {
                    tracks.for_each(x => x?.trim());
                    audio_trimmed_time += trimSeconds;
                }
                ProfilingUtility.EndSample();

                
                ProfilingUtility.BeginSample("Update track data on track");
                tracks[current_track_idx].update_data_track();
                ProfilingUtility.EndSample();

                sync_audio_to_video();
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

        void sync_audio_to_video()
        {
            if (!sync_audio_if_needed || !audioSources[0].isPlaying) return;
            
            var at = audioSources[0].time + audio_trimmed_time + play_start_time;
            var vpt = vp.time - delay_seconds;

            if (Mathf.Abs((float) (at - vpt)) > videoAudioDesyncThreshold)
            {
                var correct_time = vpt - audio_trimmed_time - play_start_time;
                audioSources.for_each(correct_time, (x,t) =>
                {
                    if(x.isPlaying)
                        x.time = (float) t;
                });
            }

            //Debug.Log("At: " + at + " Vt: " + vpt);
        }

        void prepare_tracks()
        {
            if(no_audio) return;
            
            var audioTrackCount = vp.audioTrackCount;
            tracks?.for_each( x=> x?.Dispose());
            tracks = new Track[audioTrackCount];
            
            media_info.Close();
            media_info.Open(vp.url);
            
            for (var t = 0; t < vp.audioTrackCount; t++)
            {
                var track = new Track();

                track.init(vp.GetAudioSampleProvider((ushort)t), audioSources, SampleFramesAvailable, ProviderOnSampleFramesOverflow);

                var track_title = media_info.Get(StreamKind.Audio, t, "Title");
                
                track.lang = string.IsNullOrWhiteSpace(track_title) ? vp.GetAudioLanguageCode((ushort)t) : track_title;

                Debug.Log(track.lang);
                
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

            AudioTrackChanged?.Invoke();
        }

        void set_delay()
        {
            if (!is_delay_set_needed) return;
            if (is_delay_set) return;
            
            delay_frames = render_pool_idx;
            delay_seconds = (float)(vp.time - play_start_time);    
            
            is_delay_set = true;
            is_delay_set_needed = false;
            
            is_video_playing_for_other_threads = true;
            
            Debug.Log(vp.time);
        }
        
        public class Track : IDisposable
        {
            public Channel[] channels;
            public uint add_count;
            public int trim_threshold_samples;
            public string lang;
            
            AudioSampleProvider provider;
            AudioSampleProvider.SampleFramesHandler samples_available_callback;
            AudioSampleProvider.SampleFramesHandler samples_overflow_callback;
            
            int trim_seconds_samples;
            Dictionary<uint, float[]> channel_samples_buffers_dictionary;
            Dictionary<int, NativeArray<float>> track_samples_buffers_dictionary;

            public void init(AudioSampleProvider p, AudioSource[] audioSources, AudioSampleProvider.SampleFramesHandler sa_callback, AudioSampleProvider.SampleFramesHandler so_callback)
            {
                provider = p;
                channels = new Channel[provider.channelCount];
                trim_seconds_samples = trim_seconds_static * (int)provider.sampleRate;
                trim_threshold_samples = trim_after_reaching_seconds_static * (int)provider.sampleRate;
                
                samples_available_callback = sa_callback;
                samples_overflow_callback = so_callback;
                
                provider.sampleFramesOverflow += samples_overflow_callback;
                provider.sampleFramesAvailable += samples_available_callback;
                provider.enableSampleFramesAvailableEvents = true;
                provider.freeSampleFrameCountLowThreshold = provider.sampleRate/2;

                var channel_count = provider.channelCount;
                var audio_source_idx = 0;
                
                channel_samples_buffers_dictionary = new Dictionary<uint, float[]>();
                track_samples_buffers_dictionary = new Dictionary<int, NativeArray<float>>();

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

            float[] get_sized_channel_buffer(uint size)
            {
                float[] buffer;
                
                if(!channel_samples_buffers_dictionary.TryGetValue(size, out buffer))
                    channel_samples_buffers_dictionary.Add(size, buffer = new float[size]);
                
                return buffer;
            }
            
            public NativeArray<float> get_sized_track_buffer(int size)
            {
                NativeArray<float> buffer;
                
                if(!track_samples_buffers_dictionary.TryGetValue(size, out buffer))
                    track_samples_buffers_dictionary.Add(size, buffer = new NativeArray<float>(size, Allocator.Persistent));
                
                return buffer;
            }
            
            public void clear_data()
            {
                if (track_samples_buffers_dictionary != null)
                {
                    track_samples_buffers_dictionary.for_each(x => x.Value.Dispose());
                    track_samples_buffers_dictionary.Clear();
                }
                channel_samples_buffers_dictionary?.Clear();
                channels.for_each(x=>x?.clear_data());
                add_count = 0;
            }

            public void set_clip(float t)
            {
                channels.for_each(t, (x,d) => x?.set_clip(d));
            }
            
            public void add_samples(uint sfCount, NativeArray<float> samples_buffer, int channelCount)
            {
                add_count += 1;

                if(channelCount*sfCount>samples_buffer.Length)
                    return;
                
                var samples_buffer_length = samples_buffer.Length;
                //var chnnel_buffer = new float[sfCount];
                var chnnel_buffer = get_sized_channel_buffer(sfCount);
                
                for (var c = 0; c < channelCount; c++)
                {
                    var b = 0;
                    for (var i = c; i < samples_buffer_length; i += channelCount)
                    {
                        chnnel_buffer[b] = samples_buffer[i];
                        b++;
                    }
                    channels[c].add_data(chnnel_buffer);
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

            public void Dispose()
            {
                channels?.for_each(x=>x?.Dispose());

                channels = null;

                if (track_samples_buffers_dictionary != null)
                {
                    track_samples_buffers_dictionary.for_each(x => x.Value.Dispose());
                    track_samples_buffers_dictionary.Clear();
                    track_samples_buffers_dictionary = null;
                }

                channel_samples_buffers_dictionary?.Clear();
                channel_samples_buffers_dictionary = null;

                if(provider == null) return;
                
                provider.sampleFramesOverflow -= samples_overflow_callback;
                provider.sampleFramesAvailable -= samples_available_callback;

                samples_overflow_callback = null;
                samples_available_callback = null;
                provider = null;
            }
        }

        public class Channel: IDisposable
        {
            public AudioSource audio_source;
            
            AudioClip audio_clip;
            //List<float> data;
            MyFloatList data2;
            
            public void set_clip(float t)
            {
                set_clip_data();
                
                if(audio_source == null) return;
                
                audio_source.clip = audio_clip;
                audio_source.Play();
                audio_source.time = t;
            }
            
            public void init(AudioSource s, AudioSampleProvider p, Track t)
            {
                audio_source = s;

                var sample_rate = (int) p.sampleRate;

                var sample_length = trim_after_reaching_seconds_static * sample_rate * 2;
                
                audio_clip = AudioClip.Create("", sample_length, 1, sample_rate, false, null);

                //data = new List<float>(sample_length);
                data2 = new MyFloatList(sample_length);
            }

            //public void add_data(IEnumerable<float> d)
            //{
            //    data.AddRange(d);
            //}
            
            public void add_data(float[] d)
            {
                //data.AddRange(d);
                data2.AddRange(d);
            }

            public void trim(int trim_seconds_samples)
            {
                //data.RemoveRange(0, trim_seconds_samples);
                data2.RemoveFromStart(trim_seconds_samples);
            }

            public void trim_audio_source(int trim_seconds_samples)
            {
                audio_source.timeSamples -= trim_seconds_samples;
            }
            
            public void set_clip_data()
            {
                //audio_clip.SetData(data.ToArray(), 0);
                audio_clip.SetData(data2.array, 0);
            }

            public void clear_data()
            {
                //data?.Clear();
                data2?.Clear();
            }

            public void Dispose()
            {
                if(audio_source != null)
                    audio_source.clip = null;

                if (audio_clip != null)
                {
                    Destroy(audio_clip);
                    audio_clip = null;
                }

                //data?.Clear();
                data2?.Clear();
            }
        }
    }

    public class MyFloatList
    {
        public float[] array { get; }
        private int length;
        private int max_length;

        public MyFloatList(int size)
        {
            array = new float[size];
            length = 0;
        }
        
        public void AddRange(float[] array_to_add)
        {
            var arr_length = array_to_add.Length;
            Buffer.BlockCopy(array_to_add, 0, array, length*4, arr_length*4);
            length += arr_length;
            
            if(length > max_length)
                max_length = length;
        }

        public void RemoveFromStart(int remove_count)
        {
            length -= remove_count;
            Buffer.BlockCopy(array, remove_count*4, array, 0, length*4);
        }

        public void Clear()
        {
            length = 0;
            Array.Clear(array, 0, array.Length);
        }
    }
}
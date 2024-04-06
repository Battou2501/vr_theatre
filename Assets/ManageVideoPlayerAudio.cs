using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Experimental.Audio;
using UnityEngine.Experimental.Video;
using UnityEngine.Video;

namespace DefaultNamespace
{
    public class ManageVideoPlayerAudio : MonoBehaviour
    {
        public AudioSource[] audioSources;
        
        AudioSampleProvider[] providers;
        
        public Material mat;

        public Material blitMat;
        
        public RenderTexture lightRT;

        public Transform[] screenLightSamplePoints;

        public int adjustDelayFrames;

        public bool lightAffectsScreen;
        bool light_affects_screen_current;
        
        int iterations;
        
        bool[] track_data_available;
        bool audio_started;

        bool no_audio;

        VideoPlayer vp;
        RenderTexture[] rt_pool;

        int last_unplayed_frame_idx;
        int render_pool_idx;

        bool is_delay_set;
        
        int delay_frames;

        public int buffer_iterations = 1;
        public int frame_buffer_size = 240;

        public int trimSeconds;
        public int trimAfterReachingSeconds;
        
        static int trim_seconds;
        static int trim_after_reaching_seconds;
        
        int current_track_idx;

        Track[] tracks;

        bool set_preview_frame;
        int preview_target_index;
        int frames_til_target;
        bool seek_complete;
        bool first_frame_got;
        double target_time;
        int preview_renders;
        float light_strength;
        
        [Range(0,1)]
        public float movie_light_strength = 0.2f;

        int requested_track_idx = -1;





        static double delay;

        static int sample_time;
        //static bool need_sample_time_shift;

        bool adding_samples;
        static readonly int aspect = Shader.PropertyToID("_Aspect");
        static readonly int movie_tex = Shader.PropertyToID("_MovieTex");
        static readonly int l_strength = Shader.PropertyToID("_LightStrength");
        static readonly int affected_by_light = Shader.PropertyToID("_AffectedByLight");
        static readonly int vec_arr_x = Shader.PropertyToID("_VecArrX");
        static readonly int vec_arr_y = Shader.PropertyToID("_VecArrY");
        static readonly int vec_arr_z = Shader.PropertyToID("_VecArrZ");

        void Start()
        {
            trim_seconds = trimSeconds;
            trim_after_reaching_seconds = trimAfterReachingSeconds;
            
            vp = GetComponent<VideoPlayer>();
            vp.audioOutputMode = VideoAudioOutputMode.APIOnly;
            vp.prepareCompleted += Prepared;
            vp.frameReady += VpOnFrameReady;
            vp.sendFrameReadyEvents = true;
            vp.Prepare();
            vp.seekCompleted += VpOnSeekCompleted;
            vp.loopPointReached+= VpOnLoopPointReached;
            vp.playbackSpeed = buffer_iterations;
            vp.frameDropped+= VpOnFrameDropped;
            
            foreach (var audio_source in audioSources)
            {
                audio_source.spatializePostEffects = false;
            }


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
        }
        
        void Update()
        {
            get_sample_time();
            
            handle_change_track_request();
            
            update_light();
            
            update_light_affects_screen();

            check_input();
            
            handle_audio_data_available();
        }


        
        void get_sample_time()
        {
            if (adding_samples) return;
            
            if(!audioSources[0].isPlaying) return;
            
            if(!check_all_tracks_added()) return;

            sample_time = audioSources[0].timeSamples;
        }

        static void VpOnFrameDropped(VideoPlayer source)
        {
            Debug.LogWarning("SKIPPED!!!!");
        }
        
        void VpOnLoopPointReached(VideoPlayer source)
        {
            reset_state();
            foreach (var audio_source in audioSources)
            {
                audio_source.Stop();
            }
            vp.Stop();
            
            vp.Play();
        }

        void VpOnSeekCompleted(VideoPlayer source)
        {
            seek_complete = true;
        }


        void VpOnFrameReady(VideoPlayer source, long frameIdx)
        {
            if (!first_frame_got)
            {
                first_frame_got = true;
            }
            
            Get2DTexture(vp, render_pool_idx);
            
            if (set_preview_frame && seek_complete)
            {
                preview_renders -= 1;

                if (preview_renders > 0) return;
                
                render_frame_to_screen(render_pool_idx);
                
                set_preview_frame = false;
                seek_complete = false;
            }

            render_pool_idx += 1;
            
            if (render_pool_idx >= rt_pool.Length)
                render_pool_idx = 0;


            if (!audio_started && !no_audio) return;
            
            render_frame_to_screen(last_unplayed_frame_idx);
                
            last_unplayed_frame_idx += 1;
            
            if (last_unplayed_frame_idx >= rt_pool.Length)
                last_unplayed_frame_idx = 0;

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

        void request_preview()
        {
            preview_renders = 2;
            set_preview_frame = true;
        }

        void reset_state()
        {
            foreach (var track in tracks)
            {
                track.clear_data();
            }

            reset_audio_data_ready_flags();

            audio_started = false;
            iterations = 0;
            render_pool_idx = 0;
            last_unplayed_frame_idx = 0;
            delay_frames = 0;
            delay = 0;
            is_delay_set = false;
        }
        
        void pause(bool state)
        {
            if (state)
            {
                vp.Pause();
                
                foreach (var channel in tracks[current_track_idx].channels)
                {
                    channel?.audio_source.Pause();
                }
                
                return;
            }
            
            vp.Play();
            
            foreach (var channel in tracks[current_track_idx].channels)
            {
                channel?.audio_source.Play();
            }
            
        }

        void skip(float t)
        {
            request_preview();

            vp.playbackSpeed = buffer_iterations;
            
            vp.time += t;
            
            reset_state();
        }

        void reset_audio_data_ready_flags()
        {
            for (var i = 0; i < track_data_available.Length; i++)
            {
                track_data_available[i] = false;
            }
        }

        void update_light()
        {
            var target_light = vp.isPlaying ? movie_light_strength : 1;

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
                skip(5);
            
            if (Input.GetKeyDown(KeyCode.LeftArrow))
                skip(-5);

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                request_preview();
                vp.playbackSpeed = buffer_iterations;
                vp.time = vp.length / 2f;

                reset_state();

            }

            if (Input.GetKeyDown(KeyCode.Space))
                pause(vp.isPlaying);
        }

        void handle_audio_data_available()
        {
            if(track_data_available == null || !track_data_available[current_track_idx] || iterations<buffer_iterations || no_audio) return;

            reset_audio_data_ready_flags();

            handle_audio_started();
            
            update_track_data();

            set_delay();
            
            set_correct_frame();

            return;
            
            void update_track_data()
            {
                tracks[current_track_idx].update_data_track();
            }
            
            void set_delay()
            {
                if (is_delay_set) return;
                
                delay = render_pool_idx / vp.frameRate;
                
                is_delay_set = true;
            }
            
            void handle_audio_started()
            {
                if (audio_started) return;
                
                foreach (var channel in tracks[current_track_idx].channels)
                {
                    if(channel == null) continue;
                    
                    channel.audio_source.clip = channel.audio_clip;
                    
                    if(!channel.audio_source.isPlaying && vp.isPlaying) 
                        channel.audio_source.Play();
                }

                audio_started = true;

                vp.playbackSpeed = 1;
            }
            
            void set_correct_frame()
            {

                delay_frames = Mathf.FloorToInt(vp.frameRate * (float) delay) + adjustDelayFrames;
            
                var t = delay_frames;

                t = render_pool_idx - t;

                if (t < 0)
                    t = rt_pool.Length + t;

                last_unplayed_frame_idx = t;
            }
        }

        void Prepared(VideoPlayer vp)
        {
            rt_pool = new RenderTexture[frame_buffer_size];

            var w = (int) vp.width;
            var h = (int) vp.height;
            
            mat.SetFloat(aspect, (float)w/h);

            for (var i = 0; i < frame_buffer_size; i++)
            {
                rt_pool[i] = new RenderTexture(w, h, 0, RenderTextureFormat.Default,8)
                {
                    useMipMap = true
                };
            }
            
            if (vp.audioTrackCount == 0)
            {
                no_audio = true;
                return;
            }

            prepare_tracks();

            render_frame_to_screen(0);
        }

        void prepare_tracks()
        {
            var audio_track_count = vp.audioTrackCount;
            
            tracks = new Track[audio_track_count];

            track_data_available = new bool[audio_track_count];
            
            for (var t = 0; t < vp.audioTrackCount; t++)
            {
                var track = new Track
                {
                    provider = vp.GetAudioSampleProvider((ushort)t)
                };

                var provider = track.provider;
                provider.sampleFramesAvailable += SampleFramesAvailable;
                provider.enableSampleFramesAvailableEvents = true;
                provider.freeSampleFrameCountLowThreshold = provider.maxSampleFrameCount / 4;

                track.sample_rate = (int)provider.sampleRate;
                track.channels = new Channel[provider.channelCount];

                var audio_source_idx = 0;

                for (var i = 0; i < track.channels.Length; i++)
                {
                    var channel = new Channel();
                    
                    track.channels[i] = channel;
                    
                    channel.init(audioSources[(track.channels.Length == 6 && i == 3) ? 5 : audio_source_idx], provider, track, i);
                    
                    if (track.channels.Length == 6 && i == 3)
                        continue;
                    
                    audio_source_idx += 1;
                }

                tracks[t] = track;
            }
        }

        void request_track_change(int idx)
        {
            requested_track_idx = idx;
        }
        
        void handle_change_track_request()
        {
            if(requested_track_idx == -1 || requested_track_idx >= vp.audioTrackCount || requested_track_idx == current_track_idx ) return;

            if(!check_all_tracks_added()) return;

            tracks[requested_track_idx].need_sample_time_shift = tracks[current_track_idx].need_sample_time_shift;
            
            current_track_idx = requested_track_idx;

            requested_track_idx = -1;
            
            tracks[current_track_idx].set_clip();
        }

        bool check_all_tracks_added()
        {
            if (tracks.Length == 1) return true;
            
            var add_count = tracks[0].add_count;
            
            for (var i = 1; i < tracks.Length; i++)
            {
                if (tracks[i].add_count == add_count) continue;
                
                return false;
            }
            
            foreach (var track in tracks)
            {
                track.add_count = 0;
            }

            return true;
        }
        
        bool check_all_tracks_updated()
        {
            if (tracks.Length == 1) return true;
            
            var up_count = tracks[0].update_count;
            
            for (var i = 1; i < tracks.Length; i++)
            {
                if (tracks[i].update_count == up_count) continue;
                
                return false;
            }
            
            foreach (var track in tracks)
            {
                track.update_count = 0;
            }

            return true;
        }

        void SampleFramesAvailable(AudioSampleProvider provider, uint sampleFrameCount)
        {
            adding_samples = true;
            
            using var buffer = new NativeArray<float>((int)sampleFrameCount * provider.channelCount, Allocator.Temp);
            
            var sf_count = provider.ConsumeSampleFrames(buffer);

            var track = tracks[provider.trackIndex];
            
            track.add_data_track();
            
            foreach (var channel in track.channels)
            {
                if(channel == null) continue;

                var d = new float[sf_count];
                var j = 0;
                for (var i = 0; i < sf_count * provider.channelCount; i+=provider.channelCount)
                {
                    d[j] = buffer[i+channel.channel_idx];
                    j++;
                }
                    
                channel.add_data(d);
            }

            track_data_available[provider.trackIndex] = true;

            //var uc = tracks[0].add_count;
            //var update_play_time = true;
            //for (var i = 1; i < tracks.Length; i++)
            //{
            //    if(tracks[i].add_count==uc) continue;
            //
            //    update_play_time = false;
            //    break;
            //}
            //
            //if (update_play_time)
            //{
            //    if (play_time >= 30)
            //        play_time = 25;
            //    
            //    play_time = AudioSettings.dspTime - start_time;
            //    
            //}
            
            adding_samples = false;
            
            if(provider.trackIndex != current_track_idx) return;

            if(iterations<buffer_iterations)
                iterations += 1;
        }

        class Track
        {
            public AudioSampleProvider provider;
            public Channel[] channels;
            public uint add_count;
            public uint update_count;
            public int sample_rate;
            public bool need_sample_time_shift;
            
            public int get_first_channel_audio_source_time => channels[0].audio_source.timeSamples;
            
            
            
            public void clear_data()
            {
                foreach (var channel in channels)
                {
                    channel?.clear_data();
                }

                add_count = 0;
            }

            public void set_clip()
            {
                foreach (var channel in channels)
                {
                    channel?.set_clip();
                }
            }

            public void add_data_track()
            {
                add_count += 1;
                
                var t = sample_time;
                
                if (t > trim_after_reaching_seconds * sample_rate)
                {
                    need_sample_time_shift = true;
                }
            }

            public void update_data_track()
            {
                update_count += 1;
                
                foreach (var channel in channels)
                {
                    channel?.update_data();
                }

                need_sample_time_shift = false;
            }
            
        }

        class Channel
        {
            public AudioSource audio_source;
            public AudioClip audio_clip;
            Track track;
            public int channel_idx;
            int sample_rate;

            List<float> data;

            public void set_clip()
            {
                var ap = audio_source.isPlaying;
                var t = track.get_first_channel_audio_source_time;
                audio_source.clip = audio_clip;
                
                if(!ap) return;
                
                audio_source.Play();
                audio_source.timeSamples = t;
            }
            
            public void init(AudioSource s, AudioSampleProvider p, Track t, int i)
            {
                track = t;
                
                audio_source = s;

                sample_rate = (int) p.sampleRate;

                var sample_length = 30 * sample_rate;
                
                audio_clip = AudioClip.Create("", sample_length, 1, sample_rate, false, null);

                data = new List<float>();

                channel_idx = i;
            }

            public void add_data(IReadOnlyCollection<float> d)
            {
                data.AddRange(d);

                var t = sample_time;

                if (t <= trim_after_reaching_seconds * sample_rate) return;
                
                data.RemoveRange(0, trim_seconds * sample_rate);
            }

            public void update_data()
            {
                audio_clip.SetData(data.ToArray(), 0);

                if (!track.need_sample_time_shift) return;
                
                audio_source.timeSamples -= trim_seconds * sample_rate;
            }

            public void clear_data()
            {
                data.Clear();
                audio_source.time = 0;
                audio_source.timeSamples = 0;
                audio_source.Stop();
            }
            
        }
    }
}
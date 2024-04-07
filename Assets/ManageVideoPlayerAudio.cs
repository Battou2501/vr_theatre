﻿using System;
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

        float frame_rate;
        
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
        bool is_prepared;
        
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
            sample_time = 0;
            
            tracks.for_each(x=>x.clear_data());
            
            audioSources.for_each(x => { x.clip = null; x.Stop(); } );
            
            reset_audio_data_ready_flags();

            is_delay_set = false;
            audio_started = false;
            iterations = 0;
            render_pool_idx = 0;
            last_unplayed_frame_idx = 0;
            delay_frames = 0;
            delay = 0;
        }
        
        void pause(bool state)
        {
            if(!is_prepared) return;
            
            if(state && !audio_started) return;
            
            if (state)
            {
                vp.Pause();
                
                tracks[current_track_idx].channels.for_each(x=>x?.audio_source.Pause());
                
                return;
            }
            
            vp.Play();
            
            tracks[current_track_idx].channels.for_each(x=>x?.audio_source.Play());
            
        }

        void skip(float t)
        {
            if(!is_prepared) return;
            
            if(!audio_started) return;
            
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

            set_correct_frame();

            return;
            
            void update_track_data()
            {
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
                last_unplayed_frame_idx = render_pool_idx - delay_frames;

                if (last_unplayed_frame_idx >= 0) return;
                
                last_unplayed_frame_idx = rt_pool.Length + last_unplayed_frame_idx;
            }
        }

        void Prepared(VideoPlayer vp)
        {
            rt_pool = new RenderTexture[frame_buffer_size];

            var w = (int) vp.width;
            var h = (int) vp.height;
            
            Shader.SetGlobalFloat(aspect, (float)w/h);

            for (var i = 0; i < frame_buffer_size; i++)
            {
                rt_pool[i] = new RenderTexture(w, h, 0, RenderTextureFormat.Default,8)
                {
                    useMipMap = true
                };
            }

            frame_rate = vp.frameRate;
            
            is_prepared = true;
            
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
                var track = new Track();

                track.init(vp.GetAudioSampleProvider((ushort)t), audioSources).sampleFramesAvailable += SampleFramesAvailable;

                tracks[t] = track;
            }
        }

        static void ProviderOnSampleFramesOverflow(AudioSampleProvider provider, uint sampleframecount)
        {
            Debug.Log(provider.trackIndex);
        }

        void request_track_change(int idx)
        {
            requested_track_idx = idx;
        }
        
        void handle_change_track_request()
        {
            if(!audio_started || !vp.isPlaying) return;
            
            if(requested_track_idx == -1 || requested_track_idx >= vp.audioTrackCount || requested_track_idx == current_track_idx ) return;

            if(!check_all_tracks_added()) return;

            tracks[requested_track_idx].need_sample_time_shift = tracks[current_track_idx].need_sample_time_shift;
            
            current_track_idx = requested_track_idx;

            requested_track_idx = -1;

            var audio_source_time = audioSources[0].timeSamples;
            
            stop_all_audio();
            
            tracks[current_track_idx].set_clip(audio_source_time);
        }

        void stop_all_audio()
        {
            audioSources.for_each(x=>x.Stop());
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

            return true;
        }

        void SampleFramesAvailable(AudioSampleProvider provider, uint sampleFrameCount)
        {
            adding_samples = true;
            
            using var buffer = new NativeArray<float>((int)sampleFrameCount * provider.channelCount, Allocator.Temp);
            
            var sf_count = provider.ConsumeSampleFrames(buffer);

            var track = tracks[provider.trackIndex];
            
            track.get_samples(sf_count, buffer, provider.channelCount);

            track_data_available[provider.trackIndex] = true;

            adding_samples = false;
            
            if(provider.trackIndex != current_track_idx) return;

            if(iterations<buffer_iterations)
                iterations += 1;

            if (iterations == buffer_iterations)
                set_delay();
            
            return;
            
            void set_delay()
            {
                if (is_delay_set) return;
                
                delay = render_pool_idx / frame_rate;
                
                delay_frames = Mathf.FloorToInt(frame_rate * (float) delay) + adjustDelayFrames;
                
                is_delay_set = true;
            }
        }

        class Track
        {
            AudioSampleProvider provider;
            public Channel[] channels;
            public uint add_count;
            int sample_rate;
            public bool need_sample_time_shift;

            public AudioSampleProvider init(AudioSampleProvider p, AudioSource[] audioSources)
            {
                provider = p;
                sample_rate = (int)provider.sampleRate;
                channels = new Channel[provider.channelCount];
                
                provider.sampleFramesOverflow += ProviderOnSampleFramesOverflow;
                provider.enableSampleFramesAvailableEvents = true;
                provider.freeSampleFrameCountLowThreshold = provider.maxSampleFrameCount / 4;

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

                return provider;
            }
            
            public void clear_data()
            {
                channels.for_each(x=>x?.clear_data());

                need_sample_time_shift = false;
                add_count = 0;
            }

            public void set_clip(int t)
            {
                channels.for_each(t, (x,d) => x?.set_clip(d));
            }


            public void get_samples(uint sf_count, NativeArray<float> buffer, int channel_count)
            {
                add_data_track();

                var b_length = buffer.Length;
                var buff = new float[sf_count];
                var c = 0;
                var j = 0;
                
                for (var i = 0; i < b_length; i+=1)
                {
                    buff[j] = buffer[c + j * channel_count];

                    j++;

                    if (j < sf_count) continue;
                    
                    channels[c].add_data(buff);
                    
                    j = 0;
                    c++;
                }
            }
            
            
            void add_data_track()
            {
                add_count += 1;
                
                if (sample_time > trim_after_reaching_seconds * sample_rate)
                {
                    need_sample_time_shift = true;
                }
            }

            public void update_data_track()
            {
                channels.for_each(x=>x?.update_data());

                need_sample_time_shift = false;
            }
            
        }

        class Channel
        {
            public AudioSource audio_source;
            AudioClip audio_clip;
            Track track;
            int sample_rate;

            List<float> data;

            public void set_clip(int t)
            {
                set_clip_data();
                audio_source.clip = audio_clip;
                audio_source.Play();
                audio_source.timeSamples = t;
            }
            
            public void init(AudioSource s, AudioSampleProvider p, Track t)
            {
                track = t;
                
                audio_source = s;

                sample_rate = (int) p.sampleRate;

                var sample_length = 30 * sample_rate;
                
                audio_clip = AudioClip.Create("", sample_length, 1, sample_rate, false, null);

                data = new List<float>();
            }

            public void add_data(float[] d)
            {
                data.AddRange(d);
                
                if (sample_time <= trim_after_reaching_seconds * sample_rate) return;
                
                data.RemoveRange(0, trim_seconds * sample_rate);
            }

            public void update_data()
            {
                set_clip_data();

                if (!track.need_sample_time_shift) return;
                
                audio_source.timeSamples -= trim_seconds * sample_rate;
            }

            void set_clip_data()
            {
                audio_clip.SetData(data.ToArray(), 0);
            }
            
            public void clear_data()
            {
                data.Clear();
            }
            
        }
    }
}
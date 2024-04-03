using System;
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
        
        int iters;
        
        bool[] track_data_available;
        bool audio_started;

        bool no_audio;

        VideoPlayer vp;
        RenderTexture[] rt_pool;

        int last_unplayed_frame_idx;
        int render_pool_idx;

        bool is_delay_set;
        double delay;
        int delay_frames;

        public int buffer_iterations = 1;
        public int frame_buffer_size = 240;

        int current_track_idx;

        Track[] tracks;

        bool set_preview_frame;
        int preview_target_index;
        int frames_til_target;
        bool seek_complete;
        bool first_frame_got;
        int first_frame_idx;
        double target_time;
        int preview_renders;
        
        void Start()
        {
            vp = GetComponent<VideoPlayer>();
            vp.audioOutputMode = VideoAudioOutputMode.APIOnly;
            vp.prepareCompleted += Prepared;
            vp.frameReady += VpOnframeReady;
            vp.sendFrameReadyEvents = true;
            vp.Prepare();
            vp.seekCompleted += VpOnseekCompleted;
            vp.loopPointReached+= VpOnloopPointReached;
            foreach (var audio_source in audioSources)
            {
                audio_source.spatializePostEffects = false;
            }
        }

        void VpOnloopPointReached(VideoPlayer source)
        {
            reset_state();
            foreach (var audio_source in audioSources)
            {
                audio_source.Stop();
            }
            vp.Stop();
            
            vp.Play();
        }

        void VpOnseekCompleted(VideoPlayer source)
        {
            seek_complete = true;
        }


        void VpOnframeReady(VideoPlayer source, long frameidx)
        {
            if (!first_frame_got)
            {
                first_frame_got = true;
                first_frame_idx = (int)vp.frame;
            }
            
            
            Get2DTexture(vp, render_pool_idx);
            
            if (set_preview_frame && seek_complete)
            {
                preview_renders -= 1;

                if (preview_renders > 0) return;
                
                //mat.SetTexture("_MainTex", rt_pool[render_pool_idx]);
                //Graphics.Blit(rt_pool[render_pool_idx], lightRT, blitMat);
                render_frame_to_screen(render_pool_idx);
                
                set_preview_frame = false;
                seek_complete = false;
            }

            render_pool_idx += 1;
            if (render_pool_idx >= rt_pool.Length)
                render_pool_idx = 0;


            if (audio_started || no_audio)
            {
                //mat.SetTexture("_MainTex", rt_pool[last_unplayed_frame_idx]);
                //Graphics.Blit(rt_pool[last_unplayed_frame_idx], lightRT, blitMat);
                render_frame_to_screen(last_unplayed_frame_idx);
                
                last_unplayed_frame_idx += 1;
                if (last_unplayed_frame_idx >= rt_pool.Length)
                    last_unplayed_frame_idx = 0;
            }

        }

        void render_frame_to_screen(int idx)
        {
            mat.SetTexture("_MainTex", rt_pool[idx]);
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
            iters = 0;
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
                    if(channel == null)continue;
                    
                    channel.audio_source.Pause();
                }
                
                return;
            }
            
            vp.Play();
            
            foreach (var channel in tracks[current_track_idx].channels)
            {
                channel?.audio_source.Play();
            }
            
            if(first_frame_got) return;

            vp.playbackSpeed = buffer_iterations;
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
        
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))// && vp.isPlaying)
                change_track(0);
            
            if (Input.GetKeyDown(KeyCode.Alpha2))// && vp.isPlaying)
                change_track(1);
            
            if (Input.GetKeyDown(KeyCode.Alpha3))// && vp.isPlaying)
                change_track(2);
            
            if (Input.GetKeyDown(KeyCode.Alpha4))// && vp.isPlaying)
                change_track(3);
            
            if (Input.GetKeyDown(KeyCode.Alpha5))// && vp.isPlaying)
                change_track(4);
            
            if (Input.GetKeyDown(KeyCode.Alpha6))// && vp.isPlaying)
                change_track(5);
            
            if (Input.GetKeyDown(KeyCode.RightArrow))// && vp.isPlaying)
                skip(5);
            
            if (Input.GetKeyDown(KeyCode.LeftArrow))// && vp.isPlaying)
                skip(-5);

            if (Input.GetKeyDown(KeyCode.UpArrow))// && vp.isPlaying)
            {
                request_preview();
                vp.playbackSpeed = buffer_iterations;
                vp.time = vp.length / 1.15f;

                reset_state();

            }

            if (Input.GetKeyDown(KeyCode.Space))
                pause(vp.isPlaying);


            //if (tracks != null)
            //{
            //    foreach (var channel in tracks[current_track_idx].channels)
            //    {
            //        if (!audio_started) break;
            //
            //        if (channel == null) continue;
            //
            //        if (!channel.audio_source.isPlaying && vp.isPlaying)
            //        {
            //            channel.audio_source.clip = channel.audio_clip;
            //            channel.audio_source.PlayScheduled(AudioSettings.dspTime + 1f/vp.frame);
            //        }
            //    }
            //}



            if(track_data_available == null || !track_data_available[current_track_idx] || iters<buffer_iterations || no_audio) return;

            reset_audio_data_ready_flags();

            //Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            
            foreach (var channel in tracks[current_track_idx].channels)
            {
                channel?.update_data(vp);
            }

            if (!is_delay_set)
            {
                delay = render_pool_idx / vp.frameRate;
                is_delay_set = true;
            }
            
            if (!audio_started)
            {
                //foreach (var audio_source in audioSources)
                //{
                //    //audio_source.Play();
                //}
                
                foreach (var channel in tracks[current_track_idx].channels)
                {
                    if(channel == null) continue;
                    
                    channel.audio_source.clip = channel.audio_clip;
                    
                    if(!channel.audio_source.isPlaying && vp.isPlaying) 
                        channel.audio_source.Play();
                }
            }
            
            set_correct_frame();
            
            if(audio_started) return;
            
            audio_started = true;
            
            vp.playbackSpeed = 1;

        }

        void Prepared(VideoPlayer vp)
        {
            rt_pool = new RenderTexture[frame_buffer_size];

            var w = (int) vp.width;
            var h = (int) vp.height;
            
            mat.SetFloat("_Aspect", (float)w/h);
            
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
                //vp.Play();
                return;
            }

            prepare_tracks();

            render_frame_to_screen(0);
            
            //vp.Play();
        }

        void prepare_tracks()
        {

            tracks = new Track[vp.audioTrackCount];

            track_data_available = new bool[vp.audioTrackCount];
            
            for (var t = 0; t < vp.audioTrackCount; t++)
            {
                var track = new Track
                {
                    provider = vp.GetAudioSampleProvider((ushort)t),
                    player = this
                };

                var provider = track.provider;
                provider.sampleFramesAvailable += SampleFramesAvailable;
                provider.enableSampleFramesAvailableEvents = true;
                provider.freeSampleFrameCountLowThreshold = provider.maxSampleFrameCount / 4;
                provider.enableSilencePadding = true;


                track.channels = new Channel[provider.channelCount];
                track.silence = new float[Mathf.FloorToInt(Mathf.Min((float)vp.length, 10f) * provider.sampleRate)];

                var audio_source_idx = 0;

                for (var i = 0; i < track.channels.Length; i++)
                {
                    var channel = new Channel();
                    channel.track = track;
                    track.channels[i] = channel;
                    
                    
                    channel.init(audioSources[(track.channels.Length == 6 && i == 3) ? 5 : audio_source_idx], vp, provider, i);
                    
                    
                    if (track.channels.Length == 6 && i == 3)
                        continue;
                    
                    audio_source_idx += 1;
                }

                tracks[t] = track;
            }
        }

        void change_track(ushort idx)
        {
            
            if(idx>=vp.audioTrackCount) return;
            
            pause(true);

            current_track_idx = idx;
            
            tracks[current_track_idx].clear_data();

            reset_audio_data_ready_flags();
            
            pause(false);
        }
        
        double last_t;

        void SampleFramesAvailable(AudioSampleProvider provider, uint sampleFrameCount)
        {
            using NativeArray<float> buffer = new NativeArray<float>((int)sampleFrameCount * provider.channelCount, Allocator.Temp);
            
            var sfCount = provider.ConsumeSampleFrames(buffer);
                
            if(provider.trackIndex != current_track_idx) return;
                
            var track = tracks[provider.trackIndex];
            foreach (var channel in track.channels)
            {
                if(channel == null) continue;

                var d = new float[sfCount];
                var j = 0;
                for (int i = 0; i < sfCount * provider.channelCount; i+=provider.channelCount)
                {
                    d[j] = buffer[i+channel.channel_idx];
                    j++;
                }
                    
                channel.add_data(d);
            }

            track_data_available[provider.trackIndex] = true;
                
            if(iters<buffer_iterations)
                iters += 1;
        }

        void set_correct_frame()
        {

            delay_frames = Mathf.FloorToInt(vp.frameRate * (float) delay) + adjustDelayFrames;// - first_frame_idx;
            //delay_frames = Mathf.CeilToInt(vp.frameRate * (float) delay) + adjustDelayFrames - first_frame_idx;
            
            var t = delay_frames;

            t = render_pool_idx - t;

            if (t < 0)
                t = rt_pool.Length + t;

            last_unplayed_frame_idx = t;
        }

        class Track
        {
            public AudioSampleProvider provider;
            public Channel[] channels;
            public ManageVideoPlayerAudio player;

            public float[] silence;

            public void clear_data()
            {
                foreach (var channel in channels)
                {
                    if(channel == null) continue;
                    
                    channel.clear_data();
                }
            }
        }

        class Channel
        {
            public AudioSource audio_source;
            public AudioClip audio_clip;
            public Track track;
            public int channel_idx;
            int sample_rate;

            List<float> _data;
            //List<float> _data_tmp;

            public void init(AudioSource s, VideoPlayer vp, AudioSampleProvider p, int i)
            {
                audio_source = s;

                sample_rate = (int) vp.GetAudioSampleRate(p.trackIndex);

                var sample_length = Mathf.CeilToInt((float) vp.clip.length * sample_rate);
                
                audio_clip = AudioClip.Create("", sample_length, 1, sample_rate, false, null);

                audio_clip.SetData(new float[sample_length], 0);
                
                _data = new List<float>(sample_length);
                //_data_tmp = new List<float>(sample_length);

                channel_idx = i;
            }

            public void add_data(float[] data)
            {
                _data.AddRange(data);
                //_data_tmp.AddRange(data);
            }

            public void update_data(VideoPlayer vp)
            {
                //if (!vp.isPlaying)
                //{
                //    _data_tmp.Clear();
                //    return;
                //}

                //var t = audio_source.timeSamples;
                //_data.RemoveRange(0,Mathf.Min(_data.Count,t));
                //_data.AddRange(_data_tmp);
                //_data_tmp.Clear();
                
                //audio_clip.SetData(track.silence, 0);
                
                if (_data.Count > sample_rate * 15)
                {
                    Debug.Log(audio_source.timeSamples);
                    
                    _data.RemoveRange(0, sample_rate * 5);
                
                    audio_source.timeSamples -= sample_rate * 5;
                }
                
                audio_clip.SetData(_data.ToArray(), 0);
                //audio_source.time = 0;
                
                //if(!audio_source.isPlaying && vp.isPlaying) 
                //    audio_source.Play();
            }

            public void clear_data()
            {
                //_data_tmp.Clear();
                _data.Clear();
                audio_source.timeSamples = 0;
                audio_source.time = 0;
                audio_clip.SetData(track.silence, 0);
                audio_source.clip = audio_clip;
                audio_source.Stop();
            }
            
        }
    }
}
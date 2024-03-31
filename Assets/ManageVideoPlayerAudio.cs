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
        
        //AudioSampleProvider provider;
        AudioSampleProvider[] providers;
        
        public Material mat;

        public Material blitMat;
        
        //Channel[] channels;

        //int length;
        //int offset;
        int iters;
        
        
        bool data_available;

        bool[] track_data_available;
        //bool set_data;
        bool audio_started;

        bool no_audio;

        VideoPlayer vp;
        List<Texture> ready_frames;

        RenderTexture[] rt_pool;

        int last_unplayed_frame_idx;
        public int render_pool_idx;

        bool is_delay_set;
        double delay;
        int delay_frames;

        int buffer_iterations = 1;
        int frame_buffer_size = 240;
        //int frame_buffer_size = 1440;

        int current_track_idx;

        Track[] tracks;

        bool set_preview_frame;
        public bool await_skip_catchup;
        int preview_target_index;
        public int frames_til_target;
        bool seek_complete;
        
        void Start()
        {
            vp = GetComponent<VideoPlayer>();
            vp.audioOutputMode = VideoAudioOutputMode.APIOnly;
            vp.prepareCompleted += Prepared;
            vp.frameReady += VpOnframeReady;
            vp.sendFrameReadyEvents = true;
            vp.Prepare();
            ready_frames = new List<Texture>();
            ready_frames.Add(null);
            vp.seekCompleted += VpOnseekCompleted;

        }

        void VpOnseekCompleted(VideoPlayer source)
        {
            seek_complete = true;
        }


        void VpOnframeReady(VideoPlayer source, long frameidx)
        {
            Get2DTexture(vp, render_pool_idx);

            if (set_preview_frame && seek_complete)
            {
                mat.SetTexture("_MainTex", rt_pool[render_pool_idx]);
                set_preview_frame = false;
                seek_complete = false;
            }
            
            if (await_skip_catchup && render_pool_idx == preview_target_index)
            {
                await_skip_catchup = false;
            }
            
            render_pool_idx += 1;
            if (render_pool_idx >= rt_pool.Length)
                render_pool_idx = 0;


            if (audio_started || no_audio)
            {
                if(!await_skip_catchup)
                    mat.SetTexture("_MainTex", rt_pool[last_unplayed_frame_idx]);
                
                last_unplayed_frame_idx += 1;
                if (last_unplayed_frame_idx >= rt_pool.Length)
                    last_unplayed_frame_idx = 0;
            }

            if (!await_skip_catchup) return;
            
            calculate_frames_till_target();
        }

        
        void Get2DTexture(VideoPlayer vp, int i)
        {
            Graphics.Blit(vp.texture, rt_pool[i]);
        }

        void request_preview()
        {
            set_preview_frame = true;
            await_skip_catchup = true;
            preview_target_index = render_pool_idx + delay_frames;

            if (preview_target_index >= rt_pool.Length)
                preview_target_index -= rt_pool.Length;
        }

        void calculate_frames_till_target()
        {
            frames_til_target = preview_target_index - render_pool_idx;

            if (render_pool_idx > preview_target_index)
                frames_til_target = preview_target_index + (frame_buffer_size - render_pool_idx);
        }
        
        void pause(bool state)
        {
            if (state)
            {
                vp.Pause();
                //vp.Stop();
                
                foreach (var channel in tracks[current_track_idx].channels)
                {
                    if(channel == null)continue;
                    
                    channel.audio_source.Pause();
                    //channel.audio_source.Stop();
                }
                
                return;
            }
            
            vp.Play();
            
            foreach (var channel in tracks[current_track_idx].channels)
            {
                if(channel == null)continue;
                
                channel.audio_source.Play();
            }
        }

        void skip(float t)
        {
            //pause(true);

            request_preview();
            
            vp.time += t;
                
            tracks[current_track_idx].clear_data();
                
            for (var i = 0; i < track_data_available.Length; i++)
            {
                track_data_available[i] = false;
            }
            
            //pause(false);
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
                
                //pause(true);
                
                request_preview();
                
                vp.time = vp.length / 2;
                
                tracks[current_track_idx].clear_data();
                
                for (var i = 0; i < track_data_available.Length; i++)
                {
                    track_data_available[i] = false;
                }
                
                //pause(false);
                
            }

            if (Input.GetKeyDown(KeyCode.Space))
                pause(vp.isPlaying);
            
            
            
            
            
            
            
            if(track_data_available == null || !track_data_available[current_track_idx] || iters<buffer_iterations || no_audio) return;
            
            for (var i = 0; i < track_data_available.Length; i++)
            {
                track_data_available[i] = false;
            }

            foreach (var channel in tracks[current_track_idx].channels)
            {
                if(channel == null) continue;

                channel.update_data(vp);
            }

            sync_sources();

            if (!is_delay_set)
            {
                delay = ((float)render_pool_idx) / vp.frameRate;
                delay_frames = Mathf.FloorToInt(vp.frameRate * (float) delay);
                is_delay_set = true;
            }
            
            if (!audio_started)
            {
                foreach (var audio_source in audioSources)
                {
                    audio_source.Play();
                }
            }
            
            set_correct_frame();
            
            audio_started = true;

        }

        void sync_sources()
        {
            if(tracks[current_track_idx].channels.Length < 2) return;

            for (var i = 1; i < tracks[current_track_idx].channels.Length; i++)
            {
                if(tracks[current_track_idx].channels[i] == null) continue;

                tracks[current_track_idx].channels[i].audio_source.timeSamples = tracks[current_track_idx].channels[0].audio_source.timeSamples;
            }
        }
        
        void Prepared(VideoPlayer vp)
        {
            rt_pool = new RenderTexture[frame_buffer_size];

            var w = (int) vp.width;
            var h = (int) vp.height;
            
            mat.SetFloat("_Aspect", (float)w/h);
            
            for (int i = 0; i < frame_buffer_size; i++)
            {
                rt_pool[i] = new RenderTexture(w, h, 0);
            }
            
            if (vp.audioTrackCount == 0)
            {
                no_audio = true;
                vp.Play();
                return;
            }

            prepare_tracks();

            vp.Play();
        }

        void prepare_tracks()
        {

            tracks = new Track[vp.audioTrackCount];

            track_data_available = new bool[vp.audioTrackCount];
            
            for (int t = 0; t < vp.audioTrackCount; t++)
            {
                var track = new Track
                {
                    provider = vp.GetAudioSampleProvider((ushort)t),
                    track_idx = t,
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
                    if (track.channels.Length == 6 && i == 3)
                        continue;

                    var channel = new Channel();
                    channel.init(audioSources[audio_source_idx], vp, provider, i);
                    channel.track = track;
                    audio_source_idx += 1;

                    
                    track.channels[i] = channel;
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

            for (var i = 0; i < track_data_available.Length; i++)
            {
                track_data_available[i] = false;
            }
            
            pause(false);
        }
        
        double last_t;

        void SampleFramesAvailable(AudioSampleProvider provider, uint sampleFrameCount)
        {
            using (NativeArray<float> buffer = new NativeArray<float>(
                       (int)sampleFrameCount * provider.channelCount, Allocator.Temp))
            {
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
                
                data_available = true;

                track_data_available[provider.trackIndex] = true;
                
                if(iters<buffer_iterations)
                    iters += 1;
            }
        }

        void set_correct_frame()
        {

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
            public int track_idx;
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
            public int sample_rate;

            List<float> _data;
            List<float> _data_tmp;

            public void init(AudioSource s, VideoPlayer vp, AudioSampleProvider p, int i)
            {
                audio_source = s;

                sample_rate = (int) vp.GetAudioSampleRate(p.trackIndex);

                var sample_length = Mathf.CeilToInt((float) vp.clip.length * sample_rate);
                
                audio_clip = AudioClip.Create("", sample_length, 1, sample_rate, false, null);
                
                _data = new List<float>(sample_length);
                _data_tmp = new List<float>(sample_length);

                channel_idx = i;
            }

            public void add_data(float[] data)
            {
                _data_tmp.AddRange(data);
            }

            public void update_data(VideoPlayer vp)
            {
                if (!vp.isPlaying)
                {
                    _data_tmp.Clear();
                    
                    return;
                }

                var t = audio_source.timeSamples;
                
                _data.RemoveRange(0,Mathf.Min(_data.Count,t));

                _data.AddRange(_data_tmp);
                _data_tmp.Clear();

                if (track.player.await_skip_catchup)
                {
                    var samples_to_remove = ((float) track.player.frames_til_target / vp.frameRate) * sample_rate;

                    samples_to_remove = Mathf.Min(samples_to_remove, _data.Count);
                    
                    for (int i = 0; i < samples_to_remove; i++)
                    {
                        _data[i] = 0;
                    }
                }
                
                
                audio_clip.SetData(track.silence, 0);
                audio_clip.SetData(_data.ToArray(), 0);
                audio_source.clip = audio_clip;

                audio_source.timeSamples = 0;
                audio_source.time = 0;
                
                if(!audio_source.isPlaying && vp.isPlaying) 
                    audio_source.Play();
            }

            public void clear_data()
            {
                _data_tmp.Clear();
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
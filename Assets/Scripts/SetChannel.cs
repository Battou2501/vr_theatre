using System;
using UnityEditor;
using UnityEngine;

namespace DefaultNamespace
{
    public class SetChannel : MonoBehaviour
    {
        public static float[] audio_data;

        public enum channels
        {
            left = 0,
            right = 1
        }

        public channels channel_to_mute;

        public bool is_master;

        void Awake()
        {
            if (is_master)
                audio_data = new float[2048];
        }

        void OnAudioFilterRead(float[] data, int channels)
        {

            var _data = new float[2048];
            
            for (int i = 0; i < 2048; i++)
            {
                _data[i] = audio_data[i];
            }
            
            if (is_master)
            {
                for (int i = 0; i < 2048; i++)
                {
                    audio_data[i] = data[i];
                }
            }
            //else
            //{
                for (int i = 0; i < 2048; i++)
                {
                    data[i] = _data[i];
                }
            //}
            
            for (int i = 0; i < data.Length; i += channels)
            {
                //data[i+ (int)channel_to_mute] = 0; // mute left channel
                
                for(int j=0; j<channels;j++)
                {
                    data[i+j] = data[i + (int) channel_to_mute];
                }
            }
        }
    }
}
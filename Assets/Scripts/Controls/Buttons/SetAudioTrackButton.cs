
using System;
using TMPro;
using UnityEngine;
using Zenject;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DefaultNamespace
{
    public class SetAudioTrackButton : ClickableButton
    {
        static readonly int color1 = Shader.PropertyToID("_Color");
        public TMP_Text text;
        
        [HideInInspector]
        public int track_idx;

        ManageVideoPlayerAudio video_manager;
        
        Material material;
        
        [Inject]
        public void Construct(ManageVideoPlayerAudio v)
        {
            video_manager = v;
        }

        public override void init()
        {
            base.init();
            
            material = GetComponent<Renderer>().material;
        }

        public void set_color(Color color)
        {
            material.SetColor(color1, color);
        }

        public void set_track_dta(int idx, string lang)
        {
            track_idx = idx;
            text.text = $"{idx}.{lang}";
        }

        protected override void Click_Action()
        {
            video_manager.request_track_change(track_idx);
        }
#if UNITY_EDITOR
        [CustomEditor(typeof(SetAudioTrackButton))]
        public class SetAudioTrackButtonEditor : ClickableButtonEditor {}
#endif
    }
}
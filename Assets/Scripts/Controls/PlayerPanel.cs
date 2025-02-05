using System;
using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Video;
using Zenject;

namespace DefaultNamespace
{
    public class PlayerPanel : BaseControlsPanel
    {
        public TMP_Text timeText;
        public TMP_Text trackText;

        CancellationTokenSource update_time_cancellation_token_source;

        ManageVideoPlayerAudio video_manager;
        
        [Inject]
        public void Construct(ManageVideoPlayerAudio v)
        {
            video_manager = v;
        }
        
        public override void init()
        {
            base.init();
            update_time_cancellation_token_source = new CancellationTokenSource();
            update_time_cancellation_token_source.RegisterRaiseCancelOnDestroy(this);

            _ = update_time(update_time_cancellation_token_source.Token);

            set_track_text();
            
            video_manager.AudioTrackChanged += set_track_text;
        }

        public override async UniTask show()
        {
            if(video_manager.FilePath == "") return;
        
            await base.show();
        }

        void set_time_text()
        {
            timeText.text = TimeSpan.FromSeconds(video_manager.is_seeking ? video_manager.seek_time : video_manager.Video_time).ToString(@"hh\:mm\:ss");
        }

        void set_track_text()
        {
            var show_audio_track_button = video_manager.tracks is {Length: > 1} && video_manager.CurrentTrackNumber >= 0;
            
            trackText.gameObject.SetActive(show_audio_track_button);
            
            if(!show_audio_track_button) return;
            
            trackText.text = "" + video_manager.CurrentTrackNumber + "." + video_manager.CurrentTrackLang;
        }

        void OnDestroy()
        {
            update_time_cancellation_token_source?.Cancel();
            
            if(!is_initiated || video_manager == null) return;
            
            video_manager.AudioTrackChanged -= set_track_text;
        }

        async UniTask update_time(CancellationToken token)
        {
            while (true)
            {
                if(token.IsCancellationRequested)
                    return;
                
                await UniTask.WaitWhile(() => !token.IsCancellationRequested && gameObject != null && !gameObject.activeInHierarchy, cancellationToken: token);
                
                set_time_text();
                
                await UniTask.WaitForSeconds(0.5f, cancellationToken: token); 
            }
        }
    }
}
﻿using System;
using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

namespace DefaultNamespace
{
    public class PlayerPanel : BaseControlsPanel
    {
        public TMP_Text timeText;
        public TMP_Text trackText;

        CancellationTokenSource update_time_cancellation_token_source;
        
        public override void init(MainControls m)
        {
            base.init(m);
            update_time_cancellation_token_source = new CancellationTokenSource();
            update_time_cancellation_token_source.RegisterRaiseCancelOnDestroy(this);
            _ = update_time(update_time_cancellation_token_source.Token);

            set_track_text();
            
            video_manager.AudioTrackChanged += set_track_text;
        }

        void set_time_text()
        {
            timeText.text = TimeSpan.FromSeconds(video_manager.is_seeking ? video_manager.seek_time : video_manager.Video_time).ToString(@"hh\:mm\:ss");
        }

        void set_track_text()
        {
            trackText.gameObject.SetActive(video_manager.CurrentTrackNumber>=0);
            
            if(video_manager.CurrentTrackNumber < 0) return;
            
            trackText.text = "" + video_manager.CurrentTrackNumber + "." + video_manager.CurrentTrackLang;
        }

        void OnDestroy()
        {
            update_time_cancellation_token_source?.Cancel();
        }

        async UniTask update_time(CancellationToken token)
        {
            while (true)
            {
                if(token.IsCancellationRequested)
                    break;
                
                await UniTask.WaitWhile(() => !token.IsCancellationRequested && gameObject != null && !gameObject.activeInHierarchy, cancellationToken: token);
                
                set_time_text();
                
                await UniTask.WaitForSeconds(0.5f, cancellationToken: token); 
            }
        }
    }
}
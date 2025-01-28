using System;
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

        CancellationTokenSource update_time_cancellation_token_source;
        
        public override void init(MainControls m)
        {
            base.init(m);
            update_time_cancellation_token_source = new CancellationTokenSource();
            _ = update_time(update_time_cancellation_token_source.Token);
        }

        void set_time_text()
        {
            timeText.text = TimeSpan.FromSeconds(video_manager.is_seeking ? video_manager.seek_time : video_manager.Video_time).ToString(@"hh\:mm\:ss");
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
                
                await UniTask.WaitWhile(() => !token.IsCancellationRequested && !gameObject.activeInHierarchy, cancellationToken: token);
                
                set_time_text();
                
                await UniTask.WaitForSeconds(0.5f, cancellationToken: token); 
            }
        }
    }
}
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using Zenject;

namespace DefaultNamespace
{
    public class AudioTrackPanel2 : BaseControlsPanel
    {
        public GameObject trackPrefab;
        public Transform contentBlock;
        public Transform topCup;
        public Transform middle;
        public Transform bottomCup;
        public float stepBetweenButtons;
        public int buttonsPerPge;
        public ChangeAudioTrackPage nextPageButton;
        public ChangeAudioTrackPage previousPageButton;
        public TMP_Text pageText;
        
        public Color defaultColor;
        public Color selectedColor;

        Vector3 middle_initial_scale;
        List<SetAudioTrackButton> track_buttons;
        string file_path;
        private int tracks_page;
        private int page_count;
        
        ManageVideoPlayerAudio video_manager;
        DiContainer container;
        ManageVideoPlayerAudio.Track[] tracks;
        
        [Inject]
        public void Construct(ManageVideoPlayerAudio v, DiContainer c)
        {
            video_manager = v;
            container = c;
        }
        
        public override void init()
        {
            base.init();
            
            middle_initial_scale = middle.localScale;

            //video_manager.VideoPrepared += generate_buttons;
            video_manager.AudioTrackChanged += update_buttons_color;
            nextPageButton.Clicked += show_next_page;
            previousPageButton.Clicked += show_previous_page;
        }


        void OnDestroy()
        {
            if(video_manager != null)
                video_manager.AudioTrackChanged -= update_buttons_color;
            
            if(nextPageButton != null)
                nextPageButton.Clicked -= show_next_page;
            
            if(previousPageButton != null)
                previousPageButton.Clicked -= show_previous_page;
        }

        async UniTask generate_buttons()
        {
            //if(file_path == video_manager.FilePath)
            //    return;
            
            track_buttons?.for_each(x=>Destroy(x.gameObject));

            track_buttons = new List<SetAudioTrackButton>();
            
            if(tracks == null || tracks.Length == 0) return;

            var tracks_left = tracks.Length - buttonsPerPge * tracks_page;
            var tracks_to_show = Mathf.Min(buttonsPerPge, tracks_left);
            
            for (var i = 0; i < tracks_to_show; i++)
            {
                var track_index = buttonsPerPge * tracks_page + i;
                var track = tracks[track_index];

                var button = container.InstantiatePrefab(trackPrefab, contentBlock).GetComponent<SetAudioTrackButton>();
                
                button.init();
                
                button.set_track_dta(track_index, track.lang);
                
                button.transform.localPosition += Vector3.up * stepBetweenButtons * (tracks_to_show - i - 1);
                
                button.set_color(button.track_idx == video_manager.CurrentTrackNumber ? selectedColor : defaultColor);
                
                track_buttons.Add(button);
                
                await UniTask.Yield();
            }
            
            topCup.localPosition = bottomCup.localPosition + Vector3.up * (stepBetweenButtons * tracks_to_show);
            middle.localPosition = Vector3.Lerp(topCup.localPosition,bottomCup.localPosition,0.5f);
            middle.localScale = Vector3.Scale(middle_initial_scale,new Vector3(1,tracks_to_show,1));
        }

        void update_buttons_color()
        {
            if(track_buttons == null) return;
            
            foreach (var button in track_buttons)
            {
                button.set_color(button.track_idx == video_manager.CurrentTrackNumber ? selectedColor : defaultColor);
            }
        }

        public override async UniTask show()
        {
            file_path = video_manager.FilePath;
            
            tracks = video_manager.tracks;

            buttonsPerPge = buttonsPerPge <= 0 ? 1 : buttonsPerPge;

            page_count = (tracks.Length + buttonsPerPge - 1) / buttonsPerPge;

            tracks_page = -1;
            
            for (var i = 0; i < video_manager.tracks.Length; i++)
            {
                if (video_manager.CurrentTrackNumber != i) continue;

                tracks_page = i / buttonsPerPge;
                
                break;
            }

            tracks_page = tracks_page < 0 ? 0 : tracks_page;

            update_page_switch_controlls();
            
            await generate_buttons();
            
            await base.show();
        }
        
        void show_next_page()
        {
            if((tracks_page + 1) * buttonsPerPge >= tracks.Length) return;
            
            tracks_page++;

            update_page_switch_controlls();
            
            generate_buttons().Forget();
        }
        
        void show_previous_page()
        {
            if(tracks_page == 0) return;
            
            tracks_page--;

            update_page_switch_controlls();
            
            generate_buttons().Forget();
        }

        void update_page_switch_controlls()
        {
            if(page_count<=1)
            {
                pageText?.gameObject.SetActive(false);
                nextPageButton?.gameObject.SetActive(true);
                previousPageButton?.gameObject.SetActive(true);
                return;
            }
            
            pageText.text = "" + (tracks_page + 1) +" / " + (tracks.Length + buttonsPerPge - 1) / buttonsPerPge;
            
            nextPageButton?.gameObject.SetActive(tracks_page < page_count - 1);
            
            previousPageButton.gameObject.SetActive(tracks_page>0);
        }
    }
}
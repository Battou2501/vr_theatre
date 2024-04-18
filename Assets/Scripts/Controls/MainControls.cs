using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using FFmpeg.NET;
using NReco.VideoConverter;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Video;
using InputDevice = UnityEngine.XR.InputDevice;

//using UnityEngine.InputSystem.XR;

namespace DefaultNamespace
{
    public class MainControls : MonoBehaviour
    {
        public Transform cameraTransform;
        public PlayerPanel playerPanel;
        //public FileSelectPanel fileSelectPanel;
        public HandControls leftHand;
        public HandControls rightHand;
        public ManageVideoPlayerAudio videoManager;
        public HandControls defaultActiveHand;
        public InputAction triggerPressedAction;
        
        
        HandControls active_hand;

        public Transform Active_hand_transform => active_hand.transform;

        public bool Is_dragging_control => active_hand.Is_dragging_control;
        
        DriveInfo[] drives;
        DirectoryInfo[] directories;
        FileInfo[] mp4_files;
        public VideoPlayer vp;

        DirectoryInfo current_directory;
        //DriveInfo current_drive;
        //string current_path;
        //Stopwatch sw;

        //public Texture2D tex;
        //MemoryStream stream;
        
        void Awake()
        {
            triggerPressedAction.Enable();
            triggerPressedAction.started += TriggerPressedActionOnstarted;
            
            playerPanel.real_null()?.init(videoManager, this);
            //fileSelectPanel.real_null()?.init(videoManager, this);
            leftHand.real_null()?.init(this);
            rightHand.real_null()?.init(this);
            active_hand = defaultActiveHand;
            //hide_interface();
        }
        
        
        void show_hands()
        {
            leftHand.real_null()?.gameObject.SetActive(true);
            rightHand.real_null()?.gameObject.SetActive(true);
        }
        
        void hide_hands()
        {
            leftHand.real_null()?.gameObject.SetActive(false);
            rightHand.real_null()?.gameObject.SetActive(false);
        }

        public void show_player_panel()
        {
            playerPanel.real_null()?.show_panel();
            //fileSelectPanel.real_null()?.gameObject.SetActive(false);
            show_hands();
        }
        
        //public void show_file_panel()
        //{
        //    playerPanel.real_null()?.gameObject.SetActive(false);
        //    fileSelectPanel.real_null()?.gameObject.SetActive(true);
        //    show_hands();
        //}
        
        public void hide_interface()
        {
            playerPanel.real_null()?.gameObject.SetActive(false);
            hide_hands();
        }
        
        public void set_active_hand(HandControls hand)
        {
            if(hand == null || active_hand == hand) return;
            
            active_hand.real_null()?.deactivate_hand();
            active_hand = hand;
            active_hand.activate_hand();
        }

        void TriggerPressedActionOnstarted(InputAction.CallbackContext obj)
        {
            if(playerPanel.gameObject.activeSelf) return;
            
            show_player_panel();
        }


        void set_drive(DriveInfo drive)
        {
            //current_drive = drive;
            current_directory = new DirectoryInfo(drive.Name);
            
            directories = current_directory.GetDirectories().Where(d => !d.Attributes.HasFlag(FileAttributes.Hidden) && !d.Attributes.HasFlag(FileAttributes.System)).ToArray();
            mp4_files = current_directory.GetFiles("*.mp4");
        }
        
        public void set_directory(DirectoryInfo directory)
        {
            current_directory = directory;
            
            directories = current_directory.GetDirectories().Where(d => !d.Attributes.HasFlag(FileAttributes.Hidden) && !d.Attributes.HasFlag(FileAttributes.System)).ToArray();
            mp4_files = current_directory.GetFiles("*.mp4");
        }

        public void go_up_path()
        {
            if(current_directory != null)
                current_directory = Directory.GetParent(current_directory.FullName);

            directories = null;
            mp4_files = null;

            if (current_directory == null)
            {
                drives ??= DriveInfo.GetDrives();
                
                directories = drives.Select(d => new DirectoryInfo(d.Name)).ToArray();
                return;
            }
            
            directories = current_directory.GetDirectories().Where(d => !d.Attributes.HasFlag(FileAttributes.Hidden) && !d.Attributes.HasFlag(FileAttributes.System)).ToArray();
            mp4_files = current_directory.GetFiles("*.mp4");
        }


        public void get_current_path_data(out DirectoryInfo[] dirs, out FileInfo[] files)
        {
            if (current_directory == null)
            {
                go_to_drives();
            }

            dirs = directories;

            files = mp4_files;
        }
        
        
        void go_to_drives()
        {
            current_directory = null;
            mp4_files = null;
            drives ??= DriveInfo.GetDrives();
            directories = drives.Select(d => new DirectoryInfo(d.Name)).ToArray();
        }
        
        
        public void set_file(FileInfo file)
        {
            vp.url = file.FullName;
            vp.Prepare();
        }
        
        public void set_file(int file_idx)
        {
            if(mp4_files == null || mp4_files.Length<=file_idx) return;
            
            vp.url = mp4_files[file_idx].FullName;
            vp.Prepare();
        }

        bool video_started;
        int idx = 0;
        void Update()
        {
            video_started = true;
            
            if (!vp.isPlaying && Input.GetKeyDown(KeyCode.A))
            {
                idx = 0;
                video_started = false;
            } 
            
            if (!vp.isPlaying && Input.GetKeyDown(KeyCode.D))
            {
                idx = 1;
                video_started = false;
            } 
            
            if(video_started) return;

            video_started = true;

            go_to_drives();
            
            set_directory(directories[0]);
            set_directory(directories[19]);
            
            set_file(idx);
        }
    }
}
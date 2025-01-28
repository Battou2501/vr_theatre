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
        [Header("UI Controls elements")]
        public UIManager uiManager;
        
        [Header("Hand controls")]
        public GameObject leftHandTriggerCollider;
        public GameObject rightHandTriggerCollider;
        
        public InputAction leftTriggerPressedAction;
        public InputAction rightTriggerPressedAction;
        
        [Header("Video player manager")]
        public ManageVideoPlayerAudio videoManager;
        
        DriveInfo[] drives;
        DirectoryInfo[] directories;
        FileInfo[] mp4_files;
        //VideoPlayer vp => videoManager.;

        DirectoryInfo current_directory;
        //DriveInfo current_drive;
        public string Current_path => current_directory != null ? current_directory.FullName : "";
        public bool Current_directory_has_parent => current_directory != null;

        public event Action PathChanged;
        //Stopwatch sw;

        //public Texture2D tex;
        //MemoryStream stream;
        
        void Awake()
        {
            uiManager.init(this);
            
            if(leftTriggerPressedAction != null)
                leftTriggerPressedAction.started += TriggerPressedActionOnstarted;
            if(rightTriggerPressedAction != null)
                rightTriggerPressedAction.started += TriggerPressedActionOnstarted;
        }

        public void check_hands_display()
        {
            if(uiManager.is_all_panels_closed)
                hide_hands();
            else
                show_hands();
        }
        
        void show_hands()
        {
            leftHandTriggerCollider.real_null()?.SetActive(true);
            rightHandTriggerCollider.real_null()?.SetActive(true);
        }
        
        void hide_hands()
        {
            leftHandTriggerCollider.real_null()?.SetActive(false);
            rightHandTriggerCollider.real_null()?.SetActive(false);
        }

        void TriggerPressedActionOnstarted(InputAction.CallbackContext obj)
        {
            uiManager.show_ui();
        }


        void set_drive(DriveInfo drive)
        {
            current_directory = new DirectoryInfo(drive.Name);
            
            directories = current_directory.GetDirectories().Where(d => !d.Attributes.HasFlag(FileAttributes.Hidden) && !d.Attributes.HasFlag(FileAttributes.System)).ToArray();
            mp4_files = current_directory.GetFiles("*.mp4");
            
            PathChanged?.Invoke();
        }
        
        public void set_directory(DirectoryInfo directory)
        {
            current_directory = directory;
            
            directories = current_directory.GetDirectories().Where(d => !d.Attributes.HasFlag(FileAttributes.Hidden) && !d.Attributes.HasFlag(FileAttributes.System)).ToArray();
            mp4_files = current_directory.GetFiles("*.mp4");
            
            PathChanged?.Invoke();
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
                PathChanged?.Invoke();
                return;
            }
            
            directories = current_directory.GetDirectories().Where(d => !d.Attributes.HasFlag(FileAttributes.Hidden) && !d.Attributes.HasFlag(FileAttributes.System)).ToArray();
            mp4_files = current_directory.GetFiles("*.mp4");
            
            PathChanged?.Invoke();
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
        
        public void set_file(int file_idx)
        {
            if(mp4_files == null || mp4_files.Length<=file_idx) return;
            
            videoManager.set_file(mp4_files[file_idx].FullName);
        }

        bool video_started;
        int idx = 0;
        
        //void Update()
        //{
        //    video_started = true;
        //    
        //    if (!vp.isPlaying && Input.GetKeyDown(KeyCode.A))
        //    {
        //        idx = 0;
        //        video_started = false;
        //    } 
        //    
        //    if (!vp.isPlaying && Input.GetKeyDown(KeyCode.D))
        //    {
        //        idx = 1;
        //        video_started = false;
        //    } 
        //    
        //    if(video_started) return;
        //
        //    video_started = true;
        //
        //    go_to_drives();
        //    
        //    set_directory(directories[0]);
        //    set_directory(directories[19]);
        //    
        //    set_file(idx);
        //}
    }
}
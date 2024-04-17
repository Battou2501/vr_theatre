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
    public class Controlls : MonoBehaviour
    {
        DriveInfo[] drives;
        DirectoryInfo[] directories;
        FileInfo[] mp4_files;
        public VideoPlayer vp;

        DirectoryInfo current_directory;
        DriveInfo current_drive;
        //string current_path;
        Stopwatch sw;

        public Texture2D tex;
        MemoryStream stream;
        
        void Awake()
        {
            //var root_path = Path.GetPathRoot(drives[0].Name);
            //var directories = Directory.GetDirectories(root_path).Select(s => new DirectoryInfo(s)).Where(d => !d.Attributes.HasFlag(FileAttributes.Hidden) && !d.Attributes.HasFlag(FileAttributes.System)).ToArray();
            //.Select(p=> new
            //{
            //    path = p.FullName,
            //    name = p.Name
            //}).ToArray();
            //
            //var files = Directory.GetFiles(@"G:\Torrents_Movies_G", "*.mp4");
            //
            //vp.url = files[0];

            //set_drive(drives[0]);

            //set_directory(directories[19]);

            
            //sw = Stopwatch.StartNew();
            //var c = new FFMpegConverter();
            //stream = new MemoryStream();
            //c.GetVideoThumbnail("C:\\Video\\Dune.mp4", stream,10f);
            //sw.Stop();
            //
            //tex = new Texture2D(2, 2);
            //tex.LoadImage(stream.ToArray());
        }


        public void set_drive(DriveInfo drive)
        {
            current_drive = drive;
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
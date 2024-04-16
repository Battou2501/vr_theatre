using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Video;
using InputDevice = UnityEngine.XR.InputDevice;

//using UnityEngine.InputSystem.XR;

namespace DefaultNamespace
{
    public class Controlls : MonoBehaviour
    {
        public DriveInfo[] drives;
        public VideoPlayer vp;
        void Awake()
        {
            drives = System.IO.DriveInfo.GetDrives();
            var root_path = Path.GetPathRoot(drives[0].Name);
            var directories = Directory.GetDirectories(root_path).Select(s=>new DirectoryInfo(s)).Where(d => !d.Attributes.HasFlag(FileAttributes.Hidden) && !d.Attributes.HasFlag(FileAttributes.System)).Select(p=> new
            {
                path = p.FullName,
                name = p.Name
            }).ToArray();

            var files = Directory.GetFiles(@"G:\Torrents_Movies_G", "*.mp4");

            vp.url = files[0];
        }

        void Update()
        {
        }
    }
}
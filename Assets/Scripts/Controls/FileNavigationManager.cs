using System;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class FileNavigationManager// : MonoBehaviour
{
    DriveInfo[] drives;
    DirectoryInfo[] directories;
    FileInfo[] mp4_files;

    DirectoryInfo current_directory;
    public string Current_path => current_directory != null ? current_directory.FullName : "";
    public bool Current_directory_has_parent => current_directory != null;

    public event Action PathChanged;
    
    void set_drive(DriveInfo drive)
    {
        current_directory = new DirectoryInfo(drive.Name);
        
        directories = current_directory.GetDirectories().Where(d => !d.Attributes.HasFlag(FileAttributes.Hidden) && !d.Attributes.HasFlag(FileAttributes.System)).ToArray();
        mp4_files = current_directory.GetFiles("*.mp4");
        
        PathChanged?.Invoke();
    }
    
    public async UniTask set_directory(DirectoryInfo directory)
    {
        await UniTask.RunOnThreadPool(() =>
        {
            current_directory = directory;

            directories = current_directory.GetDirectories().Where(d => !d.Attributes.HasFlag(FileAttributes.Hidden) && !d.Attributes.HasFlag(FileAttributes.System)).ToArray();
            mp4_files = current_directory.GetFiles("*.mp4");
        });
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
}

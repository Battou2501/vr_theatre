using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace DefaultNamespace
{
    public class FileSelectPanel : InterfacePanel
    {
        public GameObject directoryButtonPrefab;
        public GameObject fileButtonPrefab;

        public Transform contentBlock;
        public TMP_Text pathText;

        public GoUpPathButton goUpPathButton;

        PlayerPanel player_panel;
        MainControls main_controls;

        List<DirectoryButton> directory_buttons;
        List<FileButton> file_buttons;
        
        public void init(PlayerPanel p, MainControls c)
        {
            player_panel = p;
            main_controls = c;
            camera_transform = main_controls.cameraTransform;
            goUpPathButton.real_null()?.init(this);

            redraw_data();
        }

        void redraw_data()
        {
            pathText.text = main_controls.Current_path;
            
            directory_buttons ??= new List<DirectoryButton>();
            file_buttons ??= new List<FileButton>();

            main_controls.get_current_path_data(out var dirs, out var files);
            
            file_buttons.for_each(x=>x.gameObject.SetActive(false));
            directory_buttons.for_each(x=>x.gameObject.SetActive(false));

            fill_directory_buttons(dirs);
            fill_files_buttons(files);
            
            file_buttons = file_buttons.OrderBy(x => x.File_name).ToList();
            directory_buttons = directory_buttons.OrderBy(x => x.Directory_name).ToList();
            
            for (var i = 0; i < directory_buttons.Count; i++)
            {
                directory_buttons[i].transform.SetSiblingIndex(i);
            }

            var dir_count = dirs?.Length ?? 0; 
            
            for (var i = 0; i < file_buttons.Count; i++)
            {
                file_buttons[i].transform.SetSiblingIndex(dir_count+i);
            }
        }
        
        void fill_directory_buttons(IReadOnlyList<DirectoryInfo> dirs)
        {
            if(dirs == null) return;

            for (var i = 0; i < dirs.Count; i++)
            {
                if (i >= file_buttons.Count)
                {
                    directory_buttons.Add(Instantiate(directoryButtonPrefab, contentBlock).GetComponent<DirectoryButton>());
                }
                
                directory_buttons[i].set_data(this, dirs[i]);
                directory_buttons[i].gameObject.SetActive(true);
            }
        }
        
        void fill_files_buttons(IReadOnlyList<FileInfo> files)
        {
            if(files == null) return;

            for (var i = 0; i < files.Count; i++)
            {
                if (i >= file_buttons.Count)
                {
                    file_buttons.Add(Instantiate(fileButtonPrefab, contentBlock).GetComponent<FileButton>());
                }
                
                file_buttons[i].set_data(this, files[i]);
                file_buttons[i].gameObject.SetActive(true);
            }
        }

        public void set_directory(DirectoryInfo dirInfo)
        {
            main_controls.set_directory(dirInfo);
            
            redraw_data();
        }
        
        public void select_file(FileInfo file)
        {
            main_controls.set_file(file);

            player_panel.close_file_panel();
        }

        public void show_panel()
        {
            gameObject.SetActive(true);
            redraw_data();
        }
        
        public void close_panel()
        {
            gameObject.SetActive(false);
        }

        public void go_up_path()
        {
            main_controls.go_up_path();
            redraw_data();
        }
    }
}
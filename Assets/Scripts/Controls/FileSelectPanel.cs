﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Zenject;

namespace DefaultNamespace
{
    public class FileSelectPanel : BaseControlsPanel
    {
        public GameObject directoryButtonPrefab;
        public GameObject fileButtonPrefab;

        

        public Transform contentBlock;
        public float contentBlockHeight;
        public float contentBlockWidth;
        public int horizontalElements;
        public int verticalElements;
        
        public TMP_Text pathText;
        public TMP_Text pageText;
        
        public ChangeFilePageButton nextPageButton;
        public ChangeFilePageButton previousPageButton;
        public GoUpPathButton goUpPathButton;
        
        int max_elements => horizontalElements * verticalElements;

        float element_horizontal_step;
        float element_vertical_step;
        
        List<DirectoryButton> directory_buttons = new List<DirectoryButton>();
        List<FileButton> file_buttons = new List<FileButton>();
        DirectoryInfo[] directories;
        FileInfo[] files;

        private Dictionary<string, int> pathPageDict = new ();
        private string path;
        int current_page;
        int total_pages;

        bool has_next => current_page < total_pages;
        bool has_previous => current_page > 0;

        FileNavigationManager file_navigation_manager;
        DiContainer container;
        
        [Inject]
        public void Construct(FileNavigationManager f, DiContainer c)
        {
            file_navigation_manager = f;
            container = c;
        }
        
        public override void init()
        {
            base.init();
            
            file_navigation_manager.PathChanged += OnPathChanged;

            if(nextPageButton != null)
                nextPageButton.Clicked += OnChangedToNextPage;
            if (previousPageButton != null)
                previousPageButton.Clicked += OnChangedToPreviousPage;
            
            element_horizontal_step = contentBlockWidth / (horizontalElements - 1);
            element_vertical_step = contentBlockHeight / (verticalElements - 1);

            generate_buttons();
           
            update_data();
        }

        void OnDestroy()
        {
            if(!is_initiated) return;
            
            file_navigation_manager.PathChanged -= OnPathChanged;
            
            if(nextPageButton != null)
                nextPageButton.Clicked -= OnChangedToNextPage;
            if (previousPageButton != null)
                previousPageButton.Clicked -= OnChangedToPreviousPage;
        }

        void generate_buttons()
        {
            for (var y = 0; y < verticalElements; y++)
            {
                var pos_y = contentBlockHeight*0.5f - element_vertical_step*y;
                for (var x = 0; x < horizontalElements; x++)
                {
                    var pos_x = contentBlockWidth*0.5f - element_horizontal_step*x;
                    
                    var pos = new Vector3(pos_x, pos_y, 0);
                    
                    var button = container.InstantiatePrefab(directoryButtonPrefab, contentBlock);
                    button.transform.localPosition = pos;
                    var dir_button = button.GetComponents<BaseControl>();
                    dir_button.for_each( o=> o.init());
                    directory_buttons.Add((DirectoryButton)dir_button.FirstOrDefault(o=>o.GetType() == typeof(DirectoryButton)));
                    
                    button = container.InstantiatePrefab(fileButtonPrefab, contentBlock);
                    button.transform.localPosition = pos;
                    var file_button = button.GetComponents<BaseControl>();
                    file_button.for_each(o => o.init());
                    file_buttons.Add((FileButton)file_button.FirstOrDefault(o=>o.GetType() == typeof(FileButton)));
                }
            }
        }

        void update_buttons()
        {
            fill_files_buttons(fill_directory_buttons());
            
            nextPageButton?.gameObject.SetActive(has_next);
            previousPageButton?.gameObject.SetActive(has_previous);
            
            pageText.gameObject.SetActive(total_pages > 0);
            pageText.text = ""+(current_page+1) + " / " + (total_pages+1);
        }

        void update_data()
        {
            if (path != null && pathPageDict.ContainsKey(path))
            {
                pathPageDict[path] = current_page;
            }
            
            path = file_navigation_manager.Current_path;
            
            pathText.text = path;
            
            file_navigation_manager.get_current_path_data(out directories, out files);

            directories = directories?.OrderBy(x=>x.Name).ToArray();

            files = files?.OrderBy(x=>x.Name).ToArray();

            total_pages = ((directories?.Length ?? 0) + files?.Length ?? 0) / max_elements;
            
            if (!string.IsNullOrWhiteSpace(path) && pathPageDict.TryGetValue(path, out var page))
            {
                current_page = page;
                
                if(current_page > total_pages)
                    current_page = total_pages;
            }
            else
            {
                current_page = 0;
                if(!string.IsNullOrWhiteSpace(path))
                    pathPageDict.Add(path, current_page);
            }
            
            goUpPathButton?.gameObject.SetActive(file_navigation_manager.Current_directory_has_parent);
            
            update_buttons();
        }
        
        int fill_directory_buttons()
        {
            var shown_directories = 0;
            
            for (var i = 0; i < directory_buttons.Count; i++)
            {
                var dir_index = max_elements * current_page + i;
                
                directory_buttons[i].gameObject.SetActive(false);
                
                if(directories == null || dir_index >= directories.Length) continue;
                
                directory_buttons[i].set_data(directories[dir_index]);
                directory_buttons[i].gameObject.SetActive(true);
                shown_directories++;
            }
            return shown_directories;
        }
        
        void fill_files_buttons(int used_buttons)
        {
            for (var i = 0; i < file_buttons.Count; i++)
            {
                var file_index = max_elements * current_page + i - directories?.Length ?? 0;
                
                file_buttons[i].gameObject.SetActive(false);
                
                if(i < used_buttons || files == null || file_index >= files.Length) continue;
                
                file_buttons[i].set_data(files[file_index]);
                file_buttons[i].gameObject.SetActive(true);
            }
        }

        void OnPathChanged()
        {
           update_data();
        }

        void OnChangedToNextPage()
        {
            if(!has_next) return;
            current_page++;
            update_buttons();
        }

        void OnChangedToPreviousPage()
        {
            if(!has_previous) return;
            current_page--;
            update_buttons();
        }
    }
}
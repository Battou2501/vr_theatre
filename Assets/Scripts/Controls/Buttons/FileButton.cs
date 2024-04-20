﻿using System.IO;
using TMPro;
using UnityEngine;

namespace DefaultNamespace
{
    public class FileButton : ClickableButton
    {
        public TMP_Text text;
        
        FileInfo file_info;
        FileSelectPanel file_select_panel;

        public string File_name => file_info.Name;
        
        bool is_active;

        bool triggered;
        
        public void set_data(FileSelectPanel p, FileInfo f)
        {
            file_select_panel = p;
            file_info = f;

            text.text = file_info.Name;
        }

        protected override void Click_Action()
        {
            if(!is_active) return;
            
            file_select_panel.select_file(file_info);
        }

        void FixedUpdate()
        {
            is_active = false;
            
            if(!triggered) return;

            triggered = false;

            is_active = true;
        }

        void OnTriggerStay(Collider other)
        {
            triggered = true;
        }
    }
}
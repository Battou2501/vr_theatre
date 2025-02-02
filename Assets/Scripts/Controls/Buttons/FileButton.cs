﻿using System.IO;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DefaultNamespace
{
    public class FileButton : ClickableButton
    {
        public TMP_Text text;
        
        FileInfo file_info;
        
        public void set_data(FileInfo f)
        {
            file_info = f;

            text.text = file_info.Name;
        }

        protected override void Click_Action()
        {
            video_manager.set_file(file_info.FullName);
        }
        
#if UNITY_EDITOR
        [CustomEditor(typeof(FileButton))]
        public class FileButtonEditor : ClickableButtonEditor {}
#endif
    }
}
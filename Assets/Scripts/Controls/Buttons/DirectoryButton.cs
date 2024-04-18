using System.IO;
using TMPro;
using UnityEngine;

namespace DefaultNamespace
{
    public class DirectoryButton : ClickableButton
    {
        public TMP_Text text;
        
        DirectoryInfo dir_info;
        FileSelectPanel file_select_panel;

        public string Directory_name => dir_info.Name;
        
        public void set_data(FileSelectPanel p, DirectoryInfo d)
        {
            file_select_panel = p;
            dir_info = d;

            text.text = dir_info.Name;
        }

        protected override void Click_Action()
        {
            file_select_panel.set_directory(dir_info);
        }
        
    }
}
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace DefaultNamespace
{
    public class DirectoryButton : ClickableButton
    {
        public TMP_Text text;
        
        DirectoryInfo dir_info;

        public string Directory_name => dir_info.Name;
        
        public void set_data(DirectoryInfo d)
        {
            dir_info = d;
            text.text = dir_info.Name;
        }

        protected override void Click_Action()
        {
            Debug.Log("Clicked");
            file_navigation_manager.set_directory(dir_info);
        }
        
        [CustomEditor(typeof(DirectoryButton))]
        public class DirectoryButtonEditor : ClickableButtonEditor {}
    }
}
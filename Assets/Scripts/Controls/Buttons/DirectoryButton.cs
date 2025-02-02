using System.IO;
using Cysharp.Threading.Tasks;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DefaultNamespace
{
    public class DirectoryButton : ClickableButton
    {
        public TMP_Text text;
        
        DirectoryInfo dir_info;
        
        public void set_data(DirectoryInfo d)
        {
            dir_info = d;
            text.text = dir_info.Name;
        }

        protected override void Click_Action()
        {
            file_navigation_manager.set_directory(dir_info).Forget();
        }
#if UNITY_EDITOR
        [CustomEditor(typeof(DirectoryButton))]
        public class DirectoryButtonEditor : ClickableButtonEditor {}
#endif
    }
}
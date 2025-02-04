using System.IO;
using Cysharp.Threading.Tasks;
using TMPro;
using Zenject;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DefaultNamespace
{
    public class DirectoryButton : ClickableButton
    {
        public TMP_Text text;
        
        DirectoryInfo dir_info;

        FileNavigationManager file_navigation_manager;
        
        [Inject]
        public void Construct(FileNavigationManager f)
        {
            file_navigation_manager = f;
        }
        
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
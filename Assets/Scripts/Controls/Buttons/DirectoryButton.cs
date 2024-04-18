using System.IO;
using UnityEngine;

namespace DefaultNamespace
{
    public class DirectoryButton : ClickableButton
    {

        DirectoryInfo dir_info;
        MainControls main_controls;

        public void set_data(MainControls c, DirectoryInfo d)
        {
            main_controls = c;
            dir_info = d;
        }

        protected override void Click_Action()
        {
            main_controls.set_directory(dir_info);
        }
        
    }
}
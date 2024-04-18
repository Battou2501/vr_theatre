using System.IO;
using UnityEngine;

namespace DefaultNamespace
{
    public class FileButton : ClickableButton
    {
        FileInfo file_info;
        MainControls main_controls;

        public void set_data(MainControls c, FileInfo f)
        {
            main_controls = c;
            file_info = f;
        }

        protected override void Click_Action()
        {
            main_controls.set_file(file_info);
        }
    }
}
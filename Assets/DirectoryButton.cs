using System.IO;
using UnityEngine;

namespace DefaultNamespace
{
    public class DirectoryButton : MonoBehaviour
    {

        DirectoryInfo dir_info;
        Controlls controlls;

        public void set_data(Controlls c, DirectoryInfo d)
        {
            controlls = c;
            dir_info = d;
        }

        public void Click()
        {
            controlls.set_directory(dir_info);
        }
        
    }
}
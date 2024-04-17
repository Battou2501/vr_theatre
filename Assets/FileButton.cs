using System.IO;
using UnityEngine;

namespace DefaultNamespace
{
    public class FileButton : MonoBehaviour
    {
        FileInfo file_info;
        Controlls controlls;

        public void set_data(Controlls c, FileInfo f)
        {
            controlls = c;
            file_info = f;
        }

        public void Click()
        {
            controlls.set_file(file_info);
        }
    }
}
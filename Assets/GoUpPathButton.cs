using UnityEngine;

namespace DefaultNamespace
{
    public class GoUpPathButton : MonoBehaviour
    {
        Controlls controlls;

        public void set_data(Controlls c)
        {
            controlls = c;
        }

        public void Click()
        {
            controlls.go_up_path();
        }
    }
}
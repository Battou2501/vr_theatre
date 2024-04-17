using UnityEngine;

namespace DefaultNamespace
{
    public class GoUpPathButton : ClickableButton
    {
        MainControls main_controls;

        public void set_data(MainControls c)
        {
            main_controls = c;
        }

        public override void Click()
        {
            main_controls.go_up_path();
        }
    }
}
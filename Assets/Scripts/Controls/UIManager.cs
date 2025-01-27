using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class UIManager : BaseControl
{
    public BaseControlsPanel playerPanel;
    public BaseControlsPanel fileSelectPanel;

    BaseControlsPanel[] panels;

    public override void init(MainControls m)
    {
        base.init(m);
        
        var controls = GetComponentsInChildren<BaseControl>(true);

        foreach (var control in controls)
        {
            if(control == this) continue;
            
            control.init(main_controls);
        }
        
        panels = GetComponentsInChildren<BaseControlsPanel>(true);
    }

    public void show_ui()
    {
        if(!is_all_panels_closed) return;
        
        if(video_manager.Vp_file_selected)
            playerPanel.show();
        else
            fileSelectPanel.show();
        
        main_controls.check_hands_display();
    }

    public void hide_ui()
    {
        if(is_all_panels_closed) return;

        foreach (var panel in panels)
        {
            panel.hide();
        }
        
        main_controls.check_hands_display();
    }

    public bool is_all_panels_closed
    {
        get
        {
            foreach (var panel in panels)
            {
                if (panel.gameObject.activeInHierarchy)
                    return false;
            }

            return true;
        }
    }
}

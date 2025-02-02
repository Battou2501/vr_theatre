using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class UIManager : BaseControl
{
    public BaseControlsPanel playerPanel;
    public BaseControlsPanel fileSelectPanel;
    public ErrorPanel   errorPanel;

    public bool Is_ui_open { get; private set; }

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
        
        panels.for_each(x=>
        {
            x.Closed += OnPanelClosed;
            x.gameObject.SetActive(false);
        });

        video_manager.ErrorOccured += OnVideoErrorOccured;
        
        //hide_ui();
        
        main_controls.check_hands_display();
    }

    void OnVideoErrorOccured(string message)
    {
        panels.for_each(x=>x.hide());
        
        errorPanel.showError(message);
        
        main_controls.check_hands_display();
        
        main_controls.disable_trigger_check();
    }

    void OnPanelClosed()
    {
        if(!is_all_panels_closed) return;
        
        main_controls.check_hands_display();

        main_controls.enable_trigger_check();

        //main_controls.set_ignore_next_trigger_action();
    }

    public void show_ui()
    {
        if(!is_all_panels_closed) return;
        
        if(video_manager.Vp_file_selected)
            playerPanel.show();
        else
            fileSelectPanel.show();
        
        main_controls.check_hands_display();

        main_controls.disable_trigger_check();
    }

    //public void hide_ui()
    //{
    //    if(is_all_panels_closed) return;
    //
    //    foreach (var panel in panels)
    //    {
    //        panel.hide();
    //    }
    //    
    //    main_controls.check_hands_display();
    //}

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

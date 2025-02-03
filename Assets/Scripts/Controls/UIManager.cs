using System;
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

    public override void init()
    {
        base.init();
        
        //var controls = GetComponentsInChildren<BaseControl>(true);
        //
        //foreach (var control in controls)
        //{
        //    if(control == this) continue;
        //    
        //    control.init(main_controls);
        //}
        
        panels = GetComponentsInChildren<BaseControlsPanel>(true);
        
        panels.for_each(x=>
        {
            x.Closed += OnPanelClosed;
            x.gameObject.SetActive(false);
        });

        video_manager.ErrorOccured += OnVideoErrorOccured;
        
        main_controls.check_hands_display(is_all_panels_closed);
    }

    void OnDestroy()
    {
        if(!is_initiated || video_manager == null) return;
        
        video_manager.ErrorOccured -= OnVideoErrorOccured;
    }

    void OnVideoErrorOccured(string message)
    {
        panels.for_each(x=>x.hide());
        
        errorPanel.showError(message);
        
        main_controls.check_hands_display(is_all_panels_closed);
        
        main_controls.disable_trigger_check();
    }

    void OnPanelClosed()
    {
        if(!is_all_panels_closed) return;
        
        main_controls.check_hands_display(is_all_panels_closed);

        main_controls.enable_trigger_check();
    }

    public void show_ui()
    {
        if(!is_all_panels_closed) return;
        
        if(video_manager.Vp_file_selected)
            playerPanel.show();
        else
            fileSelectPanel.show();
        
        main_controls.check_hands_display(is_all_panels_closed);

        main_controls.disable_trigger_check();
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

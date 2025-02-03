using System;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using Zenject;

public class UIManager : MonoBehaviour
{
    public BaseControlsPanel playerPanel;
    public BaseControlsPanel fileSelectPanel;
    public ErrorPanel   errorPanel;
    
    BaseControlsPanel[] panels;

    ManageVideoPlayerAudio video_manager;
    MainControls main_controls;
    
    [Inject]
    public void Construct(ManageVideoPlayerAudio v, MainControls m)
    {
        main_controls = m;
        video_manager = v;
    }
    
    public void init()
    {
        var controls = GetComponentsInChildren<BaseControl>(true);
        
        controls.for_each(x=>x.init());
        
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
        if(video_manager == null) return;
        
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

    bool is_all_panels_closed
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

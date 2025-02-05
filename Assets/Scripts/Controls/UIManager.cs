using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class UIManager : MonoBehaviour
{
    public event Action<bool> PanelCchanged;
    
    public BaseControlsPanel playerPanel;
    public BaseControlsPanel fileSelectPanel;
    public ErrorPanel   errorPanel;
    
    BaseControlsPanel[] panels;
    ManageVideoPlayerAudio video_manager;
    MainControls main_controls;
    bool is_initialized;
    
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
    
    [Inject]
    public void Construct(ManageVideoPlayerAudio v, MainControls m)
    {
        video_manager = v;
        main_controls = m;
    }
    
    public void init()
    {
        var controls = GetComponentsInChildren<BaseControl>(true);
        
        controls.for_each(x=>x.init());
        
        panels = GetComponentsInChildren<BaseControlsPanel>(true);

        if (video_manager != null)
        {
            video_manager.ErrorOccured += OnVideoErrorOccured;
            video_manager.VideoEnded += OnVideoEnded;
        }

        if (main_controls.leftTriggerPressedAction != null)
            main_controls.leftTriggerPressedAction.performed += TriggerPressedActionOnStarted;
        
        if (main_controls.rightTriggerPressedAction != null)
            main_controls.rightTriggerPressedAction.performed += TriggerPressedActionOnStarted;
        
        //if(video_manager.FilePath == "")
        //    fileSelectPanel.show();
        
        //main_controls.check_hands_display(is_all_panels_closed);
        
        is_initialized = true;
    }
    void OnDestroy()
    {
        if(!is_initialized) return;

        if (video_manager != null)
        {
            video_manager.ErrorOccured -= OnVideoErrorOccured;
            video_manager.VideoEnded -= OnVideoEnded;
        }

        if(main_controls == null) return;
        
        if (main_controls.leftTriggerPressedAction != null)
            main_controls.leftTriggerPressedAction.performed -= TriggerPressedActionOnStarted;
        
        if (main_controls.rightTriggerPressedAction != null)
            main_controls.rightTriggerPressedAction.performed -= TriggerPressedActionOnStarted;
    }

    void OnVideoEnded()
    {
        show_ui().Forget();
    }

    void OnVideoErrorOccured(string message)
    {
        panels.for_each(x=>x.hide_immediately());
        
        errorPanel.showError(message);
        
        //main_controls.check_hands_display(is_all_panels_closed);
        
        //main_controls.disable_trigger_check();
    }


    public async UniTask show_ui()
    {
        if(!is_all_panels_closed) return;

        BaseControlsPanel panel_to_open;

        panel_to_open = video_manager.Vp_file_selected ? playerPanel : fileSelectPanel;
        
        await switch_panels(null, panel_to_open);

    }

    public async UniTask switch_panels(BaseControlsPanel panel_to_close, BaseControlsPanel panel_to_open)
    {
        if (panel_to_close != null)
            await panel_to_close.hide();
        
        if(panel_to_open != null)
            await panel_to_open.show();
        
        PanelCchanged?.Invoke(is_all_panels_closed);
    }
    
    void TriggerPressedActionOnStarted(InputAction.CallbackContext callbackContext)
    {
        if (!callbackContext.control.IsPressed()) return;
            
        show_ui().Forget();
    }
}

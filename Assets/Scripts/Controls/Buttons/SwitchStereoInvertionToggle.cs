using System;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class SwitchStereoInvertionToggle : ClickableButton
{
    [SerializeField]
    private Toggle toggle;

    private ManageVideoPlayerAudio _videoManager;
    
    [Inject]
    public void Construct(ManageVideoPlayerAudio videoManager)
    {
        _videoManager = videoManager;
    }

    private void OnEnable()
    {
        UpdateToggle();
    }

    protected override void Click_Action()
    {
        if(_videoManager == null) return;
        
        _videoManager.invertStereoOrder = !_videoManager.invertStereoOrder;

        UpdateToggle();
    }

    private void UpdateToggle()
    {
        if(_videoManager == null) return;
        
        toggle.isOn = _videoManager.invertStereoOrder;
    }
}

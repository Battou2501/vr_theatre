using System;
using DefaultNamespace;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

public class StereoModeButton : ClickableButton
{
    [FormerlySerializedAs("_videoType")] [SerializeField]
    private ManageVideoPlayerAudio.StereoTypes _stereoType;
    
    private ManageVideoPlayerAudio _videoManager;

    private bool _isActive;

    [SerializeField]
    private Material _activeMaterial;
    [SerializeField]
    private Material _inactiveMaterial;
    
    private Renderer _renderer;
    
    [Inject]
    public void Construct(ManageVideoPlayerAudio videoManager)
    {
        _videoManager = videoManager;
        _renderer = GetComponent<Renderer>();
    }

    public override void init()
    {
        base.init();

        _videoManager.StereoTypeChanged += UpdateColor;
    }

    private void OnEnable()
    {
        UpdateColor();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (_videoManager == null) return;
        
        _videoManager.StereoTypeChanged -= UpdateColor;
    }

    private void UpdateColor()
    {
        if(_renderer == null) return;
        
        if(_stereoType == _videoManager.GetStereoType() == _isActive) return;
        
        _isActive = _stereoType == _videoManager.GetStereoType();
        
        _renderer.sharedMaterial = _isActive ? _activeMaterial : _inactiveMaterial;
    }
    
    protected override void Click_Action()
    {
        _videoManager.SetStereoType(_stereoType);
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(StopButton))]
    public class StopButtonEditor : ClickableButtonEditor {}
#endif
}

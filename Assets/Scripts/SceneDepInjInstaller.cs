using DefaultNamespace;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

public class SceneDepInjInstaller : MonoInstaller
{
    public MainControls mainControls;
    public UIManager uiManager;
    public ManageVideoPlayerAudio videoManager;
    public SeatChangeSystem seatChangeSystem;
    public XROrigin xrOrigin;
    
    public override void InstallBindings()
    {
        Container.Bind<MainControls>().FromInstance(mainControls).AsSingle();
        Container.Bind<UIManager>().FromInstance(uiManager).AsSingle();
        Container.Bind<ManageVideoPlayerAudio>().FromInstance(videoManager).AsSingle();
        Container.Bind<SeatChangeSystem>().FromInstance(seatChangeSystem).AsSingle();
        Container.Bind<XROrigin>().FromInstance(xrOrigin).AsSingle();
        Container.Bind<FileNavigationManager>().AsSingle();
    }
}

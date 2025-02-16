using DefaultNamespace;
using Hands;
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
    public LodSystem lodSystem;
    [FormerlySerializedAs("handPoseSystem")] public HandControlSystem handControlSystem;
    public CameraBlackOut cameraBlackOut;
    public XROrigin xrOrigin;
    public AudioSource headAudioSource;
    [FormerlySerializedAs("letHandPointer")] public LeftHandPointer leftHandPointer;
    public RightHandPointer rightHandPointer;
    public VrInputSystem vrInputSystem;
    
    public override void InstallBindings()
    {
        Container.BindInterfacesTo<SceneDepInjInstaller>().FromInstance(this).AsSingle();
        
        Container.Bind<MainControls>().FromInstance(mainControls).AsSingle();
        Container.Bind<UIManager>().FromInstance(uiManager).AsSingle();
        Container.Bind<ManageVideoPlayerAudio>().FromInstance(videoManager).AsSingle();
        Container.Bind<SeatChangeSystem>().FromInstance(seatChangeSystem).AsSingle();
        Container.Bind<LodSystem>().FromInstance(lodSystem).AsSingle();
        Container.Bind<HandControlSystem>().FromInstance(handControlSystem).AsSingle();
        Container.Bind<CameraBlackOut>().FromInstance(cameraBlackOut).AsSingle();
        Container.Bind<XROrigin>().FromInstance(xrOrigin).AsSingle();
        Container.Bind<AudioSource>().FromInstance(headAudioSource).AsSingle();
        Container.Bind<LeftHandPointer>().FromInstance(leftHandPointer).AsSingle();
        Container.Bind<RightHandPointer>().FromInstance(rightHandPointer).AsSingle();
        Container.Bind<VrInputSystem>().FromInstance(vrInputSystem).AsSingle();
        Container.Bind<FileNavigationManager>().AsSingle();
    }
}

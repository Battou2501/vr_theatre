using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DefaultNamespace;
using UnityEngine;
using Zenject;

public class BootLoader : MonoBehaviour
{
    public LoadingScreen loadingScreen;
    public GameObject movieTheatreRoom;

    MainControls main_controls;
    UIManager ui_manager;
    ManageVideoPlayerAudio video_manager;
    SeatChangeSystem seat_change_system;
    LodSystem lod_system;
    CameraBlackOut camera_black_out;
    
    CancellationTokenSource cancellation_token_source; 
    
    [Inject]
    public void Construct(
        MainControls mainControls,
        UIManager uiManager,
        ManageVideoPlayerAudio videoManager,
        SeatChangeSystem seatChangeSystem,
        LodSystem lodSystem,
        CameraBlackOut cameraBlackOut
    )
    {
        main_controls = mainControls;
        ui_manager = uiManager;
        video_manager = videoManager;
        seat_change_system = seatChangeSystem;
        lod_system = lodSystem;
        camera_black_out = cameraBlackOut;
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        cancellation_token_source = new CancellationTokenSource();
        //cancellation_token_source.RegisterRaiseCancelOnDestroy(this.gameObject);
        loadingScreen.gameObject.SetActive(true);
        loadingScreen.set_progress(0);
        
        movieTheatreRoom.SetActive(false);
        
        load_program().Forget();
    }

    void OnDestroy()
    {
        cancellation_token_source?.Cancel();
        cancellation_token_source?.Dispose();
        cancellation_token_source = null;
    }

    async UniTask load_program()
    {
        camera_black_out.show_immediately();
        Debug.Log("loading program");
        await UniTask.Delay(1000);
        await camera_black_out.fade();
        await UniTask.Delay(100);
        main_controls.init();
        loadingScreen.set_progress(0.1f);
        await UniTask.Delay(10);
        seat_change_system.init();
        loadingScreen.set_progress(0.2f);
        await UniTask.Delay(10);
        lod_system.init();
        loadingScreen.set_progress(0.4f);
        await UniTask.Delay(10);
        ui_manager.init();
        loadingScreen.set_progress(0.7f);
        await UniTask.Delay(10);
        video_manager.init();
        loadingScreen.set_progress(1f);
        await UniTask.Delay(10);
        await camera_black_out.show();
        await UniTask.Delay(100);
        loadingScreen.gameObject.SetActive(false);
        movieTheatreRoom.SetActive(true);
        await UniTask.Delay(10);
        await camera_black_out.fade();
    }
}

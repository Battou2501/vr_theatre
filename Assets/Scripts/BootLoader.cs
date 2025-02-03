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

    public int delayBetweenLoadStepsMillis;
    public int defaultRow;
    public int defaultSeat;
    
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
        loadingScreen.set_progress(0);
        lod_system.gameObject.SetActive(false);
        ui_manager.gameObject.SetActive(false);
        camera_black_out.show_immediately();
        await UniTask.Delay(delayBetweenLoadStepsMillis);
        await camera_black_out.fade();
        await UniTask.Delay(delayBetweenLoadStepsMillis);
        main_controls.init();
        await loadingScreen.move_progress_to(0.1f);
        await UniTask.Delay(delayBetweenLoadStepsMillis);
        seat_change_system.init();
        await loadingScreen.move_progress_to(0.2f);
        await UniTask.Delay(delayBetweenLoadStepsMillis);
        lod_system.init();
        await loadingScreen.move_progress_to(0.4f);
        await UniTask.Delay(delayBetweenLoadStepsMillis);
        video_manager.init();
        await loadingScreen.move_progress_to(0.7f);
        await UniTask.Delay(delayBetweenLoadStepsMillis);
        ui_manager.init();
        await loadingScreen.move_progress_to(1f);
        await UniTask.Delay(delayBetweenLoadStepsMillis);
        await camera_black_out.show();
        await UniTask.Delay(delayBetweenLoadStepsMillis);
        await seat_change_system.change_seat(defaultRow,defaultSeat, false);
        loadingScreen.gameObject.SetActive(false);
        movieTheatreRoom.SetActive(true);
        lod_system.gameObject.SetActive(true);
        ui_manager.display_initial_ui_if_needed();
        ui_manager.gameObject.SetActive(true);
        await UniTask.Delay(delayBetweenLoadStepsMillis);
        await camera_black_out.fade();
        
    }
}

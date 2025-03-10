using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DefaultNamespace;
using FFmpeg.NET.Models;
using MediaInfoLib;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
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
    HandControlSystem hand_control_system;

    private IEnumerable<IInitable> initializable_objects;
    private IEnumerable<HideDuringLoad> hide_during_load_objects;
    
    [Inject]
    public void Construct(
        MainControls mainControls,
        UIManager uiManager,
        ManageVideoPlayerAudio videoManager,
        SeatChangeSystem seatChangeSystem,
        LodSystem lodSystem,
        CameraBlackOut cameraBlackOut,
        HandControlSystem handControlSystem
    )
    {
        main_controls = mainControls;
        ui_manager = uiManager;
        video_manager = videoManager;
        seat_change_system = seatChangeSystem;
        lod_system = lodSystem;
        camera_black_out = cameraBlackOut;
        hand_control_system = handControlSystem;
    }
    
    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = -1;
        
        if(!pre_load_check()) return;
        
        camera_black_out.show_immediately();
        
        loadingScreen.gameObject.SetActive(true);
        
        movieTheatreRoom.SetActive(false);
        
        hide_during_load_objects = FindObjectsByType<HideDuringLoad>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        
        hide_during_load_objects?.for_each(x=>x.hide());
        
        load_program().Forget();
    }

    bool pre_load_check()
    {

        var result = true;
        
        result &= main_controls != null;
        result &= ui_manager != null;
        result &= video_manager != null;
        result &= seat_change_system != null;
        result &= lod_system != null;
        result &= camera_black_out != null;
        result &= hand_control_system != null;
        result &= camera_black_out != null;
        result &= loadingScreen != null;
        result &= movieTheatreRoom != null;

        if (!result) Debug.LogError("Load check failed");
        
        return result;
    }
    
    async UniTask load_program()
    {
        loadingScreen.set_progress(0);
        lod_system.gameObject.SetActive(false);
        ui_manager.gameObject.SetActive(false);
        InputSystem.actions.Enable();
        await UniTask.Delay(delayBetweenLoadStepsMillis);
        await camera_black_out.fade();
        initializable_objects = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None).OfType<IInitable>();
        initializable_objects.for_each(x=>x?.init());
        await loadingScreen.move_progress_to(0.1f);
        await UniTask.Delay(delayBetweenLoadStepsMillis);
        main_controls.init();
        await loadingScreen.move_progress_to(0.2f);
        await UniTask.Delay(delayBetweenLoadStepsMillis);
        hand_control_system.init();
        await loadingScreen.move_progress_to(0.3f);
        await UniTask.Delay(delayBetweenLoadStepsMillis);
        seat_change_system.init();
        await loadingScreen.move_progress_to(0.5f);
        await UniTask.Delay(delayBetweenLoadStepsMillis);
        lod_system.init();
        await loadingScreen.move_progress_to(0.7f);
        await UniTask.Delay(delayBetweenLoadStepsMillis);
        video_manager.init();
        await loadingScreen.move_progress_to(0.8f);
        await UniTask.Delay(delayBetweenLoadStepsMillis);
        ui_manager.init();
        await loadingScreen.move_progress_to(1f);
        await UniTask.Delay(delayBetweenLoadStepsMillis);
        await camera_black_out.show();
        await UniTask.Delay(delayBetweenLoadStepsMillis);
        await seat_change_system.change_seat(defaultRow,defaultSeat, false);
        //loadingScreen.gameObject.SetActive(false);
        Destroy(loadingScreen.gameObject);
        movieTheatreRoom.SetActive(true);
        lod_system.gameObject.SetActive(true);
        ui_manager.gameObject.SetActive(true);
        hide_during_load_objects?.for_each(x=>x.show());
        await ui_manager.show_ui();
        await UniTask.Delay(delayBetweenLoadStepsMillis);
        await camera_black_out.fade();
        
    }
}

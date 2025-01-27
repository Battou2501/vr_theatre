using DefaultNamespace;
using UnityEngine;

public abstract class BaseControl : MonoBehaviour
{
    protected MainControls main_controls;
    protected ManageVideoPlayerAudio video_manager;

    protected bool is_initiated;
    
    public virtual void init(MainControls m)
    {
        main_controls = m;
        video_manager = main_controls.videoManager;
        is_initiated = true;
    }
}

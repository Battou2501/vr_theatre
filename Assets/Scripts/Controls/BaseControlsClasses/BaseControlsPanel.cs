using DefaultNamespace;
using UnityEngine;

public abstract class BaseControlsPanel : BaseControl
{
    protected Transform camera_transform;
    public float followSpeed;

    Vector3 cam_forward;
    Vector3 pos;
    Vector3 look_vec;
    Vector3 cam_pos;

    public override void init(MainControls m)
    {
        base.init(m);
        
        camera_transform = main_controls.cameraTransform;
    }

    public virtual void show()
    {
        gameObject.SetActive(true);
    }
    
    public virtual void hide()
    {
        gameObject.SetActive(false);
    }
}

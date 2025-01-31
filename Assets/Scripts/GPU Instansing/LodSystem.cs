using DefaultNamespace;
using Unity.XR.CoreUtils;
using UnityEngine;

public class LodSystem : MonoBehaviour
{
    public ComputeShader computeShader;

    public float renderAngle { get; private set; }

    [HideInInspector]
    public Transform main_camera_transform;

    [HideInInspector]
    public int kernel_index;

    [HideInInspector]
    public uint batch_size;
    
    LodInstance[] lods;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var cam = Camera.main;
        main_camera_transform = Camera.main.transform;
        
        kernel_index = computeShader.FindKernel("CSMain");
        computeShader.GetKernelThreadGroupSizes(kernel_index, out batch_size,out _, out _);

        renderAngle = cam.GetHorizontalFieldOfView() * 1.0f;

        lods = GetComponents<LodInstance>();
        
        if(lods.Length == 0) return;
        
        lods.for_each(this, (x, s)=>x.init(s));
    }

    // Update is called once per frame
    void Update()
    {
        if(lods.Length == 0) return;

        Shader.SetGlobalVector("cam_pos", main_camera_transform.position);// - main_camera_transform.forward);
        Shader.SetGlobalVector("cam_dir", main_camera_transform.forward);
        
        lods.for_each(x=>x.render_object());
    }
}

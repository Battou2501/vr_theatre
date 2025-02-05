#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Serialization;

public class GenerateCubemapFromCamera : MonoBehaviour
{
    public Camera source_camera;
    public Cubemap cubemap;

    void capture_cubemap()
    {
        if(source_camera == null || cubemap == null) return;
        
        source_camera.RenderToCubemap(cubemap);
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(GenerateCubemapFromCamera))]
    public class GenerateCubemapFromCameraEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Capture cubemap"))
            {
                ((GenerateCubemapFromCamera)target)?.capture_cubemap();
            }
        }
    }
#endif
}

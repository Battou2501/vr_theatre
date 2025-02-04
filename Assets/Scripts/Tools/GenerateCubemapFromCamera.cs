using UnityEditor;
using UnityEngine;

public class GenerateCubemapFromCamera : MonoBehaviour
{
    public Camera camera;
    public Cubemap cubemap;

    void capture_cubemap()
    {
        if(camera == null || cubemap == null) return;
        
        camera.RenderToCubemap(cubemap);
    }

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
}

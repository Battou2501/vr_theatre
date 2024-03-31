using UnityEngine;

namespace DefaultNamespace
{
    public class TestEmission : MonoBehaviour
    {
        public Renderer renderer;
 
// An example to update the emission color & intensity (and albedo) every frame.
        void Update()
        {
            RendererExtensions.UpdateGIMaterials(renderer);
        }
    }
}
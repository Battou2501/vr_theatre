
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class SetLightOnStartUp
{
    static readonly int l_strength = Shader.PropertyToID("_LightStrength");
    
    static SetLightOnStartUp()
    {
        Shader.SetGlobalFloat(l_strength, 1);
    }
}

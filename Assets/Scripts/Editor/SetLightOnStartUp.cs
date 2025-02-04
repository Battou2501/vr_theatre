
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class SetLightOnStartUp
{
    static readonly int l_strength = Shader.PropertyToID("_LightStrength");
    static readonly int affected_by_light = Shader.PropertyToID("_AffectedByLight");
    
    static SetLightOnStartUp()
    {
        Shader.SetGlobalFloat(l_strength, 1);
        Shader.SetGlobalFloat(affected_by_light, 1);
    }
}

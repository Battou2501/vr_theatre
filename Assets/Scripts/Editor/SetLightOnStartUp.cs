
using System;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class SetLightOnStartUp
{
    static readonly int l_strength = Shader.PropertyToID("_LightStrength");
    static readonly int affected_by_light = Shader.PropertyToID("_AffectedByLight");
    
    static SetLightOnStartUp()
    {
        EditorApplication.playModeStateChanged += EditorApplicationOnplayModeStateChanged;
        set_light_values();
    }

    static void EditorApplicationOnplayModeStateChanged(PlayModeStateChange obj)
    {
        if (obj == PlayModeStateChange.ExitingPlayMode)
            set_light_values();
    }

    static void set_light_values()
    {
        Shader.SetGlobalFloat(l_strength, 1);
        Shader.SetGlobalFloat(affected_by_light, 1);
    }
}

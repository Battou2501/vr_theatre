using System;
using DefaultNamespace;
using UnityEditor;
using UnityEngine;

public class ChangeFilePageButton : ClickableButton
{
    public event Action Clicked;
    
    protected override void Click_Action()
    {
        Debug.Log("Flipping");
        Clicked?.Invoke();
    }
    
    [CustomEditor(typeof(ChangeFilePageButton))]
    public class ChangeFilePageButtonEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if(!Application.isPlaying || target == null) return;
            
            if (GUILayout.Button("Flip page"))
            {
                (target as ChangeFilePageButton).Click();
            }
        }
    }
}

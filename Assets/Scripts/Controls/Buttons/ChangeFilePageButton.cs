using System;
using DefaultNamespace;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class ChangeFilePageButton : ClickableButton
{
    public event Action Clicked;
    
    protected override void Click_Action()
    {
        Debug.Log("Flipping");
        Clicked?.Invoke();
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(ChangeFilePageButton))]
    public class ChangeFilePageButtonEditor : ClickableButtonEditor {}
#endif
}

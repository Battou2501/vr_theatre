using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(BaseControl))]
public class HighlightOnHover : MonoBehaviour
{
    public Color highlightColor;
    
    Material material;
    Color initialColor;

    private BaseControl _baseControl;
    private bool highlighted;

    private void Awake()
    {
        if(material != null) return;
        
        _baseControl = GetComponent<BaseControl>();
        material = GetComponent<Renderer>().material;
        initialColor = material.color;
    }

    void HighlightOn()
    {
        highlighted = true;
        
        material?.DOColor(highlightColor, 0.5f);
    }

    void HighlightOff()
    {
        highlighted = false;
        
        material?.DOColor(initialColor, 0.5f);
    }
    
    private void FixedUpdate()
    {
        CheckEnter();
        CheckExit();
    }
    
    void CheckEnter()
    {
        if(!highlighted && _baseControl.isHovered)
            HighlightOn();
    }
    
    void CheckExit()
    {
        if(highlighted && !_baseControl.isHovered)
            HighlightOff();
    }
}

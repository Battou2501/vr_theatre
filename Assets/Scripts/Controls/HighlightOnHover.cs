using System;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(BaseControl))]
public class HighlightOnHover : MonoBehaviour
{
    public Color highlightColor;
    
    Material material;
    Color initialColor;
    
    void OnEnable()
    {
        if(material != null) return;
        
        material = GetComponent<Renderer>().material;
        initialColor = material.color;
    }

    void OnDisable()
    {
        if(material == null) return;
        
        material.color = initialColor;
    }

    void OnTriggerEnter(Collider other)
    {
        material?.DOColor(highlightColor, 0.5f);
    }

    void OnTriggerExit(Collider other)
    {
        material?.DOColor(initialColor, 0.5f);
    }
}

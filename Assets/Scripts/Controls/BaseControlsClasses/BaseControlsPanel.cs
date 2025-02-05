using System;
using Cysharp.Threading.Tasks;
using DefaultNamespace;
using DG.Tweening;
using UnityEngine;

public abstract class BaseControlsPanel : BaseControl
{
    public float display_scale_multiplier = 1.0f;
    Vector3 initial_scale;
    
    public override void init()
    {
        base.init();
        
        initial_scale = this_transform.localScale * display_scale_multiplier;
        this_transform.localScale = initial_scale;
    }

    public virtual async UniTask show()
    {
        this_transform.localScale = Vector3.zero;
        gameObject.SetActive(true);
        
        await this_transform.DOScale(initial_scale, 0.3f).SetEase(Ease.OutBack,2).AsyncWaitForCompletion();
    }
    
    public async UniTask hide()
    {
        await this_transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack,2).AsyncWaitForCompletion();
        
        gameObject.SetActive(false);
    }
    
    public void hide_immediately()
    {
        gameObject.SetActive(false);
    }
}

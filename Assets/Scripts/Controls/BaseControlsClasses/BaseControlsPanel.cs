using System;
using DefaultNamespace;
using UnityEngine;

public abstract class BaseControlsPanel : BaseControl
{
    public event Action Closed;

    public virtual void show()
    {
        gameObject.SetActive(true);
    }
    
    public virtual void hide()
    {
        gameObject.SetActive(false);
        Closed?.Invoke();
    }
}

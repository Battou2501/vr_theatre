using System;
using DefaultNamespace;
using UnityEngine;

public abstract class BaseControlsPanel : BaseControl
{
    public event Action Closed;
    public event Action Opened;

    public virtual void show()
    {
        gameObject.SetActive(true);
        Opened?.Invoke();
    }
    
    public virtual void hide()
    {
        gameObject.SetActive(false);
        Closed?.Invoke();
    }
}

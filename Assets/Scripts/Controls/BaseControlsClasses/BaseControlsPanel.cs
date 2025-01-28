using DefaultNamespace;
using UnityEngine;

public abstract class BaseControlsPanel : BaseControl
{
    public virtual void show()
    {
        gameObject.SetActive(true);
    }
    
    public virtual void hide()
    {
        gameObject.SetActive(false);
    }
}

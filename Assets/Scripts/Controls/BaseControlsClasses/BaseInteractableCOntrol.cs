using UnityEngine;

public class BaseInteractableCOntrol : BaseControl
{
    protected bool isInteractable
    {
        get;
        private set;
    }

    public void set_interactable(bool s)
    {
        isInteractable = s;
    }
}

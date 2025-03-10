using UnityEngine;

public class HideDuringLoad : MonoBehaviour
{
    private bool _initialState;
    
    public void hide()
    {
        _initialState = gameObject.activeSelf;
        
        if(!_initialState) return;

        gameObject.SetActive(false);
    }

    public void show()
    {
        if(!_initialState) return;
        
        gameObject.SetActive(true);
    }
}

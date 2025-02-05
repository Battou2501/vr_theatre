using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class ErrorPanel : BaseControlsPanel
{
    public TMP_Text errorText;
    
    public void showError(string error)
    {
        errorText.text = error;
        show().Forget();
    }
}

using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    public Image loadingBar;
    
    public void set_progress(float p)
    {
        loadingBar.fillAmount = p;
    }

    public void fadeOut()
    {
        loadingBar.DOFade(0f, .5f).OnComplete(()=> gameObject.SetActive(false));
    }
}

using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CameraBlackOut : MonoBehaviour
{
    public Image blackScreen;
    public float fadeDuration = 1f;

    public async UniTask fade()
    {
        gameObject.SetActive(true);
        await blackScreen.DOFade(0, fadeDuration).OnComplete(() => gameObject.SetActive(false)).AsyncWaitForCompletion().AsUniTask();
    }

    public async UniTask show()
    {
        gameObject.SetActive(true); 
        await blackScreen.DOFade(1, fadeDuration).AsyncWaitForCompletion().AsUniTask();
    }

    public void show_immediately()
    {
        gameObject.SetActive(true);
        blackScreen.DOFade(1, 0.0f);
    }
}

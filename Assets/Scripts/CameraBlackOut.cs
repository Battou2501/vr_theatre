using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CameraBlackOut : MonoBehaviour
{
    public Image blackScreen;

    public async UniTask fade()
    {
        await blackScreen.DOFade(0, 0.5f).OnComplete(() => gameObject.SetActive(false)).AsyncWaitForCompletion().AsUniTask();
    }

    public async UniTask show()
    {
        gameObject.SetActive(true); 
        await blackScreen.DOFade(1, 0.5f).AsyncWaitForCompletion().AsUniTask();
    }

    public void show_immediately()
    {
        gameObject.SetActive(true);
        blackScreen.DOFade(1, 0.01f);
    }
}

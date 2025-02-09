using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    public Image loadingBar;
    float current_progress;
    
    public void set_progress(float p)
    {
        loadingBar.fillAmount = p;
        current_progress = p;
    }

    public async UniTask move_progress_to(float p)
    {
        await DOTween.To(() => current_progress, x=> current_progress = x, p, 0.5f).OnUpdate(() => loadingBar.fillAmount = current_progress).AsyncWaitForCompletion().AsUniTask();
    }
}

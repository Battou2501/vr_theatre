using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TestUniTaskCancelation : MonoBehaviour
{
    CancellationTokenSource cancellationTokenSource;
    
    private float timer;

    private bool elapsed;
    
    void Start()
    {
        cancellationTokenSource = new CancellationTokenSource();
        test1().AttachExternalCancellation(cancellationTokenSource.Token).Forget();
        test2(cancellationTokenSource.Token).Forget();
        test3().AsUniTask().AttachExternalCancellation(cancellationTokenSource.Token).Forget();
    }

    void Update()
    {
        if(elapsed) return;
        
        timer += Time.deltaTime;
        
        if(timer < 4) return;
        
        cancellationTokenSource.Cancel();
        cancellationTokenSource.Dispose();
        cancellationTokenSource = null;
        elapsed = true;
    }

    async UniTask test1()
    {
        Debug.Log("test1 Start");
        await UniTask.WaitForSeconds(10);
        Debug.Log("test1 End");
    }
    
    async UniTask test2(CancellationToken cancelToken)
    {
        Debug.Log("test2 Start");
        await UniTask.WaitForSeconds(10, cancellationToken: cancelToken);
        Debug.Log("test2 End");
    }
    
    async Task test3()
    {
        Debug.Log("test3 Start");
        await Task.Delay(10000);
        Debug.Log("test3 End");
    }
}

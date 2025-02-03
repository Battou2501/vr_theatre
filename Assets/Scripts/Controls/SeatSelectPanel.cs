using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DefaultNamespace;
using UnityEngine;
using Zenject;

public class SeatSelectPanel : BaseControlsPanel
{
    public GameObject seatSelectButtonPrefab;
    
    public Transform seatSelectButtonsContainer;

    public float rowStep;

    public float seatStep;
    
    SeatChangeSystem seat_change_system;
    
    CancellationTokenSource update_time_cancellation_token_source;
    
    [Inject]
    public void Construct(SeatChangeSystem s)
    {
        seat_change_system = s;
        
        update_time_cancellation_token_source = new CancellationTokenSource();
        update_time_cancellation_token_source.RegisterRaiseCancelOnDestroy(this);
        
        generate_buttons(update_time_cancellation_token_source.Token).Forget();
    }

    void OnDestroy()
    {
        if(update_time_cancellation_token_source == null) return;
        
        update_time_cancellation_token_source.Cancel();
    }

    async UniTask generate_buttons(CancellationToken token)
    {
        await UniTask.WaitWhile(() => !token.IsCancellationRequested && container == null && (seat_change_system.rows == null || seat_change_system.rows.Length == 0));
        
        if(token.IsCancellationRequested)
            return;
        
        for (var i = 0; i < seat_change_system.rows.Length; i++)
        {
            var row = seat_change_system.rows[i];

            var seat_count = row.seats.Length;
            
            for (var j = 0; j < seat_count; j++)
            {
                var seat = row.seats[j];
                
                var button = container.InstantiatePrefab(seatSelectButtonPrefab, seatSelectButtonsContainer).GetComponent<ChangeSeatButton>();
                
                button.transform.localPosition  = Vector3.down * rowStep * i + Vector3.down * rowStep * (i>4 ? 1 : 0) + Vector3.right * seatStep * ((float)-seat_count/2 + j);
                
                button.set_data(row.rowNumber,seat.seatNumber);
            }
        }
    }
    
}

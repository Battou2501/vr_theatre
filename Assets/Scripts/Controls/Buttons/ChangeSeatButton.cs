using DefaultNamespace;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Zenject;

public class ChangeSeatButton : ClickableButton
{
    SeatChangeSystem seat_change_system;

    int row_idx;
    int seat_idx;
    
    [Inject]
    public void Construct(SeatChangeSystem s)
    {
        seat_change_system = s;
    }

    public void set_data(int r, int s)
    {
        row_idx = r;
        seat_idx = s;
    }
    
    protected override void Click_Action()
    {
        if(seat_change_system == null) return;
        
        seat_change_system.change_seat(row_idx,seat_idx);
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(ChangeSeatButton))]
    public class ChangeSeatButtonEditor : ClickableButtonEditor {}
#endif
}

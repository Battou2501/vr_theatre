using DefaultNamespace;
using UnityEngine;
using Zenject;

public class SeatSelectPanel : BaseControlsPanel
{
    public GameObject seatSelectButtonPrefab;
    
    public Transform seatSelectButtonsContainer;

    public float rowStep;

    public float seatStep;

    public Color defaultButtonColor;
    public Color selectedButtonColor;
    
    SeatChangeSystem seat_change_system;
    
    DiContainer container;
    
    [Inject]
    public void Construct(SeatChangeSystem s, DiContainer d)
    {
        seat_change_system = s;
        container = d;
    }

    public override void init()
    {
        base.init();
        
        generate_buttons();
    }

    void generate_buttons()
    {
        for (var i = 0; i < seat_change_system.rows.Length; i++)
        {
            var row = seat_change_system.rows[i];

            var seat_count = row.seats.Length;

            var left_shift = (float) -(seat_count - 1) / 2;
            
            for (var j = 0; j < seat_count; j++)
            {
                var seat = row.seats[j];
                
                var button = container.InstantiatePrefab(seatSelectButtonPrefab, seatSelectButtonsContainer).GetComponent<ChangeSeatButton>();
                
                button.transform.localPosition  = Vector3.down * (rowStep * i) + Vector3.down * (rowStep * (i>4 ? 1 : 0)) + Vector3.left * (seatStep * (left_shift + j));
                button.init();
                button.set_data(row.rowNumber,seat.seatNumber, defaultButtonColor, selectedButtonColor);
            }
        }
    }
    
}

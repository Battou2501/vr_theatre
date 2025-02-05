using Cysharp.Threading.Tasks;
using DefaultNamespace;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Zenject;

public class ChangeSeatButton : ClickableButton
{
    static readonly int color = Shader.PropertyToID("_Color");

    public Vector3 default_scale;
    public Vector3 selected_scale;
    
    SeatChangeSystem seat_change_system;

    int row_idx;
    int seat_idx;

    Color default_color;
    Color selected_color;
    
    Material material;
    
    [Inject]
    public void Construct(SeatChangeSystem s)
    {
        seat_change_system = s;
    }

    public override void init()
    {
        base.init();
        
        material = GetComponent<Renderer>()?.material;
        
        seat_change_system.SeatChanged += OnSeatChanged;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        
        if(!is_initiated) return;
        
        seat_change_system.SeatChanged -= OnSeatChanged;
    }

    void OnSeatChanged()
    {
        var is_current_seat = seat_change_system.is_current_seat(row_idx, seat_idx);
        set_color(is_current_seat ? selected_color : default_color);
        this_transform.localScale = is_current_seat ? selected_scale : default_scale;
    }

    public void set_data(int r, int s, Color dc, Color sc)
    {
        row_idx = r;
        seat_idx = s;
        default_color = dc;
        selected_color = sc;
    }
    
    protected override void Click_Action()
    {
        if(seat_change_system == null) return;
        
        if(seat_change_system.is_current_seat(row_idx, seat_idx)) return;
        
        seat_change_system.change_seat(row_idx,seat_idx).Forget();
    }

    void set_color(Color c)
    {
        material.SetColor(color, c);
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(ChangeSeatButton))]
    public class ChangeSeatButtonEditor : ClickableButtonEditor {}
#endif
}

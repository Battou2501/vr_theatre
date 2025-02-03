﻿using System.Linq;
using Cysharp.Threading.Tasks;
using Unity.XR.CoreUtils;
using UnityEngine;
using Zenject;

namespace DefaultNamespace
{
    public class SeatChangeSystem : MonoBehaviour
    {
        public float forwardOffsetFromSeat;
        
        [HideInInspector]
        public Row[] rows;
        
        //MainControls main_controls;
        
        CameraBlackOut camera_black_out;
        XROrigin xr_origin;

        [Inject]
        public void Construct(XROrigin xro, CameraBlackOut cb)
        {
            xr_origin = xro;
            camera_black_out = cb;
        }
        
        public void init()
        {
            var seat_rows = FindObjectsByType<SeatRow>(FindObjectsInactive.Include, FindObjectsSortMode.None).OrderBy(x=>x.rowNumber);

            rows = new Row[seat_rows.Count()];
            var i = 0;
            foreach (var seatRow in seat_rows)
            {
                var seat_points = seatRow.GetComponentsInChildren<SeatPoint>(true).OrderBy(x=>x.transform.position.x);
                
                var row = new Row();

                rows[i] = row;
                
                row.rowNumber = i;
                
                row.seats = new Seat[seat_points.Count()];

                var j = 0;
                foreach (var seatPoint in seat_points)
                {
                    var seat = new Seat();
                    
                    row.seats[j] = seat;

                    seat.seatNumber = j;
                    
                    seat.position = seatPoint.transform.position + seatPoint.transform.forward * forwardOffsetFromSeat;
                    seat.rotation = seatPoint.transform.rotation;
                    
                    j++;
                }
                
                i++;
            }
        }

        public async UniTask change_seat(int row, int seat, bool black_out = true)
        {
            if(black_out)
                await camera_black_out.show();
            
            xr_origin.transform.position = rows[row].seats[seat].position;
            xr_origin.transform.rotation = rows[row].seats[seat].rotation;
            
            if(black_out)
                await camera_black_out.fade();
        }

        public class Row
        {
            public int rowNumber;
            public Seat[] seats;
        }

        public class Seat
        {
            public int seatNumber;
            public Vector3 position;
            public Quaternion rotation;
        }
    }
}
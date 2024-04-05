using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Palmmedia.ReportGenerator.Core.Reporting.Builders;
using UnityEngine;

public class LodSwitcher : MonoBehaviour
{
    public GameObject lod0;
    public GameObject lod1;
    public GameObject lod2;
    public GameObject lod3;

    public float lod0MaxDistance;
    public float lod1MaxDistance;
    public float lod2MaxDistance;

    float lod0_max_distance_sqr;
    float lod1_max_distance_sqr;
    float lod2_max_distance_sqr;
    
    Transform cam_transform;

    int current_lod_level = -1;
    
    // Start is called before the first frame update
    void Awake()
    {
        cam_transform = Camera.main.transform;
        lod0_max_distance_sqr = lod0MaxDistance * lod0MaxDistance;
        lod1_max_distance_sqr = lod1MaxDistance * lod1MaxDistance;
        lod2_max_distance_sqr = lod2MaxDistance * lod2MaxDistance;
    }

    // Update is called once per frame
    void Update()
    {
        var cam_dist_sqr = (cam_transform.position - transform.position).sqrMagnitude;

        var needed_lod_level = current_lod_level;

        if (cam_dist_sqr > lod2_max_distance_sqr)
        {
            needed_lod_level = 3;
        }
        else if (cam_dist_sqr > lod1_max_distance_sqr)
        {
            needed_lod_level = 2;
        }
        else if (cam_dist_sqr > lod0_max_distance_sqr)
        {
            needed_lod_level = 1;
        }
        else
        {
            needed_lod_level = 0;
        }
        
        if(needed_lod_level == current_lod_level) return;
        
        set_active_lod(needed_lod_level);

        current_lod_level = needed_lod_level;
    }

    void set_active_lod(int level)
    {
        var lod0_state = false;
        var lod1_state = false;
        var lod2_state = false;
        var lod3_state = false;
        
        switch (level)
        {
            case 0:
                lod0_state = true;
                break;
            case 1:
                lod1_state = true;
                break;
            case 2:
                lod2_state = true;
                break;
            case 3:
                lod3_state = true;
                break;
        }
        
        lod0.SetActive(lod0_state);
        lod1.SetActive(lod1_state);
        lod2.SetActive(lod2_state);
        lod3.SetActive(lod3_state);
    }
}

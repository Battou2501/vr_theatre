using System.Collections;
using System.Collections.Generic;
using Palmmedia.ReportGenerator.Core.Reporting.Builders;
using UnityEngine;

public class LodSwitcher : MonoBehaviour
{
    public Mesh lod1;
    public Mesh lod2;

    public MeshFilter meshFilter;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            meshFilter.mesh = lod2;
        }
    }
}

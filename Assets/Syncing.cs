using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Syncing : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SyncSources());
    }

    public AudioSource master;
    public AudioSource[] slaves;


    private IEnumerator SyncSources()
    {
        while (true)
        {
            foreach (var slave in slaves)
            {
                //if (master.clip != slave.clip)
                //{
                //    slave.clip = master.clip;
                //}
                
                if (master.isPlaying != slave.isPlaying)
                {
                    if(master.isPlaying)
                        slave.Play();
                    else
                        slave.Stop();
                }
                
                //slave.timeSamples = master.timeSamples;
                yield return null;
            }
        }    
    }

}

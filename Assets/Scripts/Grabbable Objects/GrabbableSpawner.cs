using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GrabbableSpawner : GrabbableObject
{
    [SerializeField]
    private GrabbableObjectsPool grabbableObjectsPool;

    [SerializeField] 
    private AudioClip[] spawnAudioClips;
    
    [SerializeField] 
    private AudioSource audioSource;
    
    private int lastUsedAudioClipIndex = -1;
    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        
        //Debug.Log("Spawner trigger entered");
    }

    protected override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);
        
        //Debug.Log("Spawner trigger exited");
    }

    public override void OnGrabbed(HandController hand_controller)
    {
        if(hand_controller == null || hand_controller.is_grabbing) return;
        
        spawn_grabbable(hand_controller);
    }
    
    private void spawn_grabbable(HandController hand_controller)
    {
        if(hand_controller.is_grabbing) return;
        
        var spawned = grabbableObjectsPool.GetObject();
        
        if(spawned == null) return;

        playSpawnAudio();
        
        spawned.OnGrabbed(hand_controller);
    }

    private void playSpawnAudio()
    {
        if(audioSource == null) return;
        
        if(spawnAudioClips is not {Length: > 0}) return;
        
        var clipIndex = -1;
        var iterations = 0;
        Random.InitState(Time.frameCount);
        
        while (clipIndex == -1 || lastUsedAudioClipIndex == clipIndex || iterations++ > 10)
        {
            clipIndex = Random.Range(0, spawnAudioClips.Length);
        }
        
        audioSource.PlayOneShot(spawnAudioClips[clipIndex]);
        lastUsedAudioClipIndex = clipIndex;
        
        Debug.Log("Spawner played clip: "+ clipIndex);
    }
}

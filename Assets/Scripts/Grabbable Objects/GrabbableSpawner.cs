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

    public override bool OnGrabbed(HandController hand_controller)
    {
        if(hand_controller == null || hand_controller.is_grabbing) return false;
        
        spawn_grabbable(hand_controller);
        
        return true;
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

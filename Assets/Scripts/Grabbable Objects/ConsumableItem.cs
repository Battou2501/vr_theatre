using System;
using UnityEngine;
using Zenject;

public class ConsumableItem : MonoBehaviour, IInitable
{
    private enum ConsumableItemType
    {
        Destroy_On_Consume = 0,
        Play_Sound_On_Consume = 1,
    }
    
    [SerializeField]
    private ConsumableItemType itemType;
    [SerializeField]
    private string consumerTag;
    [SerializeField]
    private AudioClip consumedClip;
    [SerializeField]
    private bool loopAudio;

    private TagHandle consumer_tag_handle;
    
    private GrabbableObject grabbable_object;

    private GameObject triggered_by;

    private AudioSource head_audio_source;

    private bool is_initialized;
    
    [Inject]
    public void Construct(AudioSource headAudioSource)
    {
        head_audio_source = headAudioSource;    
    }
    
    public void init()
    {
        consumer_tag_handle = TagHandle.GetExistingTag(consumerTag);
        grabbable_object = GetComponent<GrabbableObject>();
        is_initialized = true;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if(!is_initialized) return;
        
        if(!other.CompareTag(consumer_tag_handle)) return;
        
        triggered_by = other.gameObject;
        
        release_from_grab();

        play_audio_clip();
        
        if(itemType != ConsumableItemType.Destroy_On_Consume) return;
        
        Destroy(gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if(!is_initialized) return;
        
        if(itemType != ConsumableItemType.Play_Sound_On_Consume) return;
        
        if(other.gameObject != triggered_by) return;
        
        if(head_audio_source == null) return;
        
        if(head_audio_source.clip != consumedClip) return;

        head_audio_source.loop = false;

        //head_audio_source.Stop();

        //head_audio_source.clip = null;
    }

    void play_audio_clip()
    {
        if (head_audio_source == null || consumedClip == null) return;
        
        if (itemType == ConsumableItemType.Destroy_On_Consume || !loopAudio)
        {
            head_audio_source.PlayOneShot(consumedClip);
            return;
        }
        
        head_audio_source.clip = consumedClip;
        head_audio_source.loop = true;
        head_audio_source.Play();

    }
    
    private void release_from_grab()
    {
        if(grabbable_object == null) return;
        
        grabbable_object.release();
    }
}

using System;
using DG.Tweening;
using Grabbable_Objects;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

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
    private AudioClip[] consumedClips;
    [SerializeField]
    private bool loopAudio;
    
    [SerializeField][Range(0,1)]
    private float volume;
    
    private int lastUsedAudioClipIndex = -1;

    private TagHandle consumer_tag_handle;
    
    private GrabbableObject grabbable_object;

    private GameObject triggered_by;

    private AudioSource head_audio_source;

    private PooledObject _pooledObject;
    
    private bool is_initialized;
    
    [Inject]
    public void Construct(AudioSource headAudioSource)
    {
        head_audio_source = headAudioSource;
        grabbable_object = GetComponent<GrabbableObject>();
        _pooledObject = GetComponent<PooledObject>();
    }
    
    public void init()
    {
        consumer_tag_handle = TagHandle.GetExistingTag(consumerTag);
        is_initialized = true;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.tag);
        
        if(!is_initialized) return;
        
        if(!other.CompareTag(consumer_tag_handle)) return;
        
        triggered_by = other.gameObject;
        
        release_from_grab();

        play_audio_clip();
        
        if(itemType != ConsumableItemType.Destroy_On_Consume) return;

        if (_pooledObject != null)
        {
            transform.position = _pooledObject.pool.transform.position;
            transform.SetParent(_pooledObject.pool.transform);
            gameObject.SetActive(false);
            return;
        }
        
        Destroy(gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!is_initialized) return;
        
        if (itemType != ConsumableItemType.Play_Sound_On_Consume) return;
        
        if (other.gameObject != triggered_by) return;
        
        if(head_audio_source == null) return;

        if (consumedClips is not {Length: > 0}) return;
        
        if (head_audio_source.clip != consumedClips[0]) return;
        
        head_audio_source.DOKill();
        
        head_audio_source.DOFade(0, 0.2f).onComplete += () =>
        {
            head_audio_source.Stop();
            head_audio_source.clip = null;
            head_audio_source.loop = false;
        };

        //head_audio_source.clip = null;
    }

    void play_audio_clip()
    {
        if (head_audio_source == null || consumedClips is not {Length: > 0}) return;
        
        if (itemType == ConsumableItemType.Destroy_On_Consume || !loopAudio)
        {
            head_audio_source.volume = 1;
            //head_audio_source.PlayOneShot(consumedClip);
            playConsumeAudio();
            return;
        }

        head_audio_source.DOKill();
        head_audio_source.volume = 0;
        head_audio_source.clip = consumedClips[0];
        head_audio_source.loop = true;
        head_audio_source.Play();
        head_audio_source.time = consumedClips[0].length * Random.value;
        head_audio_source.DOFade(volume, 0.2f);

    }
    
    private void release_from_grab()
    {
        if(grabbable_object == null) return;
        
        grabbable_object.release();
    }
    
    private void playConsumeAudio()
    {
        if(head_audio_source == null) return;
        
        if(consumedClips is not {Length: > 0}) return;
        
        var clipIndex = -1;
        var iterations = 0;
        Random.InitState(Time.frameCount);

        if (_pooledObject != null)
            lastUsedAudioClipIndex = _pooledObject.lastPooledConsumeAudioIndex;
        
        while (clipIndex == -1 || lastUsedAudioClipIndex == clipIndex || iterations++ > 10)
        {
            clipIndex = Random.Range(0, consumedClips.Length);
        }
        
        head_audio_source.PlayOneShot(consumedClips[clipIndex], volume * (0.9f + Random.value * 0.1f));
        lastUsedAudioClipIndex = clipIndex;
        
        if (_pooledObject != null)
            _pooledObject.lastPooledConsumeAudioIndex = clipIndex;
    }
}

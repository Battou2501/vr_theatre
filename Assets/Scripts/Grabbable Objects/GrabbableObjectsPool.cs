using UnityEngine;
using Zenject;

public class GrabbableObjectsPool : MonoBehaviour, IInitable
{
    [SerializeField]
    private GameObject objectPrefab;
    [SerializeField]
    private int size;
    private GrabbableObject[] pool;
    private int nextFreeIndex;
    private DiContainer _container;

    [Inject]
    public void Construct(DiContainer c)
    {
        _container = c;
    }
    
    public void init()
    {
        if(objectPrefab == null) return;
        
        pool = new GrabbableObject[size];
        
        for (int i = 0; i < size; i++)
        {
            var obj = _container.InstantiatePrefab(objectPrefab);
            obj.transform.SetParent(transform);
            obj.SetActive(false);
            pool[i] = obj.GetComponent<GrabbableObject>();
        }
    }
    
    public GrabbableObject GetObject()
    {
        for (var i = 0; i < pool.Length; i++)
        {
            var obj = pool[i];

            if (obj.IsGRabbed) continue;
            
            nextFreeIndex = i+1;
            
            if(nextFreeIndex >= size)
                nextFreeIndex = 0;

            return obj;
        }
        
        return null;
    }

    public void Reset()
    {
        nextFreeIndex = 0;
        
        for (var i = 0; i < pool.Length; i++)
        {
            var obj = pool[i];

            if (obj.IsGRabbed)
                nextFreeIndex = i + 1;
            
            if(obj.IsGRabbed) continue;
            
            obj.transform.SetParent(transform);
            obj.transform.localPosition = Vector3.zero;
            obj.gameObject.SetActive(false);
        }
        
        if(nextFreeIndex >= size)
            nextFreeIndex = 0;
    }
}

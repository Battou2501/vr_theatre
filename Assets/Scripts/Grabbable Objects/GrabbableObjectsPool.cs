using System.Collections.Generic;
using DefaultNamespace;
using Grabbable_Objects;
using UnityEngine;
using Zenject;

public class GrabbableObjectsPool : MonoBehaviour, IInitable
{
    [SerializeField]
    private GameObject objectPrefab;
    [SerializeField]
    private int size;
    private List<GrabbableObject> pool;
    private DiContainer _container;

    [Inject]
    public void Construct(DiContainer c)
    {
        _container = c;
    }
    
    public void init()
    {
        if(objectPrefab == null) return;
        
        pool = new List<GrabbableObject>();
        
        for (int i = 0; i < size; i++)
        {
            var obj = _container.InstantiatePrefab(objectPrefab);
            obj.transform.SetParent(transform);
            obj.SetActive(false);
            obj.name = i.ToString();
            pool.Add(obj.GetComponent<GrabbableObject>());
            pool[i].init();
            pool[i].GetComponent<ConsumableItem>().real_null()?.init();
            var pooledObject = pool[i].GetComponent<PooledObject>().real_null();
            
            if(pooledObject == null) return;
            
            pooledObject.pool = this;
        }
    }
    
    public GrabbableObject GetObject()
    {

        GrabbableObject takenObj = null;
        int takenIndex = -1;
        for (var i = 0; i < size; i++)
        {
            var obj = pool[i];
            
            if (obj.IsGrabbed) continue;

            takenObj = obj;
            
            takenIndex = i;
            
            break;
        }
        
        if(takenObj == null) return null;
        
        pool.RemoveAt(takenIndex);
        
        pool.Insert(size-2, takenObj);
        
        return takenObj;
    }

    public void Reset()
    {
        for (var i = 0; i < size; i++)
        {
            var obj = pool[i];
            
            if(obj.IsGrabbed) continue;
            
            obj.transform.SetParent(transform);
            obj.transform.localPosition = Vector3.zero;
            obj.gameObject.SetActive(false);
        }
    }
}

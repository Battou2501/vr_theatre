using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BaseHandPointer : MonoBehaviour, IHandPointer
{
    private Rigidbody _rigidbody;
    
    public void SetActive(bool isActive)
    {
        if(_rigidbody == null)
            _rigidbody = GetComponent<Rigidbody>();
        
        _rigidbody.detectCollisions = isActive;
        gameObject.SetActive(isActive);
    }
}

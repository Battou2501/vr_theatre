using UnityEngine;

public class TestAlign : MonoBehaviour
{
    public Transform child;
    public Transform parent;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.localRotation = Quaternion.Inverse(child.localRotation);
        transform.localPosition = Vector3.zero;
        transform.position += transform.position - child.position;
        //transform.localPosition = -child.localPosition;
    }
}

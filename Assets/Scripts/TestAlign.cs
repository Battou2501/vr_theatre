using UnityEngine;

public class TestAlign : MonoBehaviour
{
    public Transform child;
    public Transform parent;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.localRotation = Quaternion.Inverse(child.localRotation);
        if (transform.localScale.x < 0)
        {
            var r = transform.localRotation.eulerAngles;
            transform.localRotation = Quaternion.Euler(r.x, -r.y, -r.z);
            //transform.localRotation *= Quaternion.Euler(0, 180, 0);
        }
        transform.localPosition = Vector3.zero;
        transform.position += transform.position - child.position;
        //transform.localPosition = -child.localPosition;
    }
}

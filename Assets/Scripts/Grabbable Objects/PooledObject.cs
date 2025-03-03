using UnityEngine;

namespace Grabbable_Objects
{
    public class PooledObject : MonoBehaviour
    {
        [SerializeField] public GrabbableObjectsPool pool { get; private set; }
    }
}
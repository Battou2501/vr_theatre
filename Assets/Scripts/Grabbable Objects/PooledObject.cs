using System;
using UnityEngine;

namespace Grabbable_Objects
{
    public class PooledObject : MonoBehaviour
    {
        public GrabbableObjectsPool pool { get; set; }
        
        [NonSerialized]
        public int lastPooledConsumeAudioIndex = -1;
    }
}
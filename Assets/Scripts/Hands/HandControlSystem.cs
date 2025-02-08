using DefaultNamespace;
using UnityEngine;

public class HandControlSystem : MonoBehaviour
{
    public enum Handedness
    {
        Left = 0,
        Right = 1
    }
    
    [SerializeField]
    HandController leftHandController;
    [SerializeField]
    HandController rightHandController;
    [SerializeField]
    public float poseUpdateSpeed;

    public void init()
    {
        leftHandController.real_null()?.init();
        rightHandController.real_null()?.init();
    }
}

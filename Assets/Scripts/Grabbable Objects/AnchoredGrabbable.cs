using System;
using DG.Tweening;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class AnchoredGrabbable : GrabbableObject
{
    [SerializeField]
    private Transform anchorPoint;

    protected override void OnDisable()
    {
        base.OnDisable();
        
        transform.position = anchorPoint.position;
        transform.rotation = anchorPoint.rotation;
        transform.SetParent(anchorPoint);
        //anchorPoint.SetParent(transform);
    }

    private void OnEnable()
    {
        transform.position = anchorPoint.position;
        transform.rotation = anchorPoint.rotation;
        transform.SetParent(anchorPoint);
    }

    //public override void OnGrabbed(HandController hand_controller)
    //{
    //    anchorPoint.SetParent(null);
    //    base.OnGrabbed(hand_controller);
    //}

    protected override void OnReleased(HandController hand_controller)
    {
        base.OnReleased(hand_controller);

        ReturnToAnchorPoint();

        //transform.position = anchorPoint.position;
        //transform.rotation = anchorPoint.rotation;
        //anchorPoint.SetParent(transform);
    }
    
    private void ReturnToAnchorPoint()
    {
        transform.SetParent(anchorPoint);
        
        Sequence moveSequence = DOTween.Sequence();
        moveSequence
            .Append(transform.DOMove(anchorPoint.position, 0.5f).SetEase( Ease.OutElastic, 0.1f, 0.4f))
            .Join(transform.DORotate(anchorPoint.rotation.eulerAngles, 0.5f).SetEase( Ease.OutElastic, 0.5f, 0.5f));
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(AnchoredGrabbable))]
    class AnchoredGrabbableEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var obj = (AnchoredGrabbable) target;

            if (!Application.isPlaying) return;
            
            if (GUILayout.Button("Return to Anchor Point"))
                obj.ReturnToAnchorPoint();
        }
    }
#endif
}

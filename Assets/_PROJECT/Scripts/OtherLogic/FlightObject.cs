using System;
using System.Threading;
using _PROJECT.Scripts.Helpers;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class FlightObject : MonoBehaviour {
    [SerializeField] protected Transform _transformForRotate;
    public AnimationCurve CurrentCurve { get; protected set; }
    public float SegmentDuration { get; protected set; }
    public float ExpandedTime { get; protected set; } = 0;
    protected Vector3 _initialPos;
    protected CancellationTokenSource _tokenSource;
    protected Quaternion _defaultModelRotation;
    
    
    
    public Vector3 TargetPos { get; protected set; }
    
    protected void ResetModelRotation() {
        _transformForRotate.localRotation = _defaultModelRotation;
    }
        
    private void OnDestroy() {
        UniTaskHelper.DisposeTask(ref _tokenSource);
    }

}

using System;
using System.Threading;
using _PROJECT.Scripts.Helpers;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class FlightObject : MonoBehaviour {
    public AnimationCurve CurrentCurve { get; protected set; }
    public float SegmentDuration { get; protected set; }
    public float ExpandedTime { get; protected set; } = 0;
    protected Vector3 _initialPos;
    protected CancellationTokenSource _tokenSource;
    
    public Vector3 TargetPos { get; protected set; }
    
        
    private void OnDestroy() {
        UniTaskHelper.DisposeTask(ref _tokenSource);
    }

}

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class FlightObject : MonoBehaviour {
    protected AnimationCurve _currentCurve;
    protected float _segmentDuration;
    protected float _expandedTime = 0;
    protected Vector3 _initialPos;
    protected CancellationTokenSource _tokenSource;
    protected CancellationToken _token;
    
    public Vector3 TargetPos { get; protected set; }
    
        
    private void OnDestroy() {
        _tokenSource?.Cancel();
        _tokenSource?.Dispose();
    }

}

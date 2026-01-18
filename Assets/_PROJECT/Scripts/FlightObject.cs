using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class FlightObject : MonoBehaviour {
    protected AnimationCurve _currentCurve;
    protected float _segmentDuration;
    protected float _expandedTime = 0;
    protected Vector3 _initialPos;
    protected CancellationTokenSource _tokenSource;
    
    public Vector3 TargetPos { get; protected set; }

    protected CancellationToken CreateNewToken() {
        _tokenSource?.Cancel();
        _tokenSource?.Dispose();
        _tokenSource =  new CancellationTokenSource();
        return _tokenSource.Token;
    }
    
        
    private void OnDestroy() {
        _tokenSource?.Cancel();
        _tokenSource?.Dispose();
    }

}

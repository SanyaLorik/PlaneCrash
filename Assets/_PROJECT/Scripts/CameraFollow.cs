using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;

public class CameraFollow : MonoBehaviour {
    [SerializeField] private PlayerMovement _player;
    [SerializeField] private CinemachineCamera _camera;
    [SerializeField] private Vector3 _walkingOffset;
    [SerializeField] private Vector3 _flightOffset;

    //  0 0.2 0.82 // damping
    
    private CinemachineFollow _cinemachineFollow;
    private CancellationTokenSource _cameraCTS;
    private void Awake() {
        _cameraCTS = new CancellationTokenSource();
        // _player.OnStateChange += OnStateChange;
        _cinemachineFollow = _camera.GetComponent<CinemachineFollow>();
    }



    private void OnDestroy() {
        _cameraCTS?.Cancel();
        _cameraCTS?.Dispose();
    }

    private void OnStateChange(PlayerMovement.PlayerState state) {
        _cameraCTS?.Cancel();
        _cameraCTS?.Dispose();
        _cameraCTS = new CancellationTokenSource();
        if (state == PlayerMovement.PlayerState.Flight) {
            ChangeCameraOffset(_flightOffset);
        }
        else if (state == PlayerMovement.PlayerState.Walking) {
            ChangeCameraOffset(_walkingOffset);
        }
    }

    private async UniTaskVoid ChangeCameraOffset(Vector3 offset) {
        Debug.Log(_cameraCTS.Token.GetHashCode());
        Vector3 oldOffset = _cinemachineFollow.FollowOffset;
        float duration = 1f; // 1секунду меняется приближение
        float elapsedTime = 0f;
        while (elapsedTime < duration) {
            float t = elapsedTime / duration;
            _cinemachineFollow.FollowOffset = Vector3.Lerp(oldOffset, offset, t);
            
            elapsedTime += Time.deltaTime;
            await UniTask.Yield(_cameraCTS.Token);
        }
        _cinemachineFollow.FollowOffset = offset;
    }
}

using System;
using System.Threading;
using _PROJECT.Scripts.Helpers;
using Cysharp.Threading.Tasks;
using SanyaBeerExtension;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class Speedometer : MonoBehaviour {
    [SerializeField] private Image _image;
    [SerializeField] private AnimationCurve _speedVisualCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Чем меньше - тем медленнее реагирует стрелка")]
    [SerializeField] private float _smoothFactor = 3f;
    
    
    
    float _normalizedSpeed;
    float _slopeFactor = 0.5f; // насколько сильно подъем/спуск влияет
    float _minSpeedVisual = 0.2f; // минимальное значение спидометра
    float _maxSpeedVisual = 1f;   // максимальное значение спидометра


    
    private CancellationTokenSource _tokenSource;
    private PlayerStateManager _playerStateManager;

    [Inject] private PlayerMovement _playerMovement;
    
    [Inject]
    private void Init(PlayerStateManager playerStateManager) {
        _playerStateManager = playerStateManager;
        _playerStateManager.ChangeState += PlayerStateManagerOnChangeState;
    }

    private void PlayerStateManagerOnChangeState(PlayerState state) {
        if (state == PlayerState.Flight) {
            _image.ActiveSelf();
            _tokenSource?.Cancel();
            _tokenSource = new CancellationTokenSource();
            FlightAsync(_tokenSource.Token);
        }
        else if (state == PlayerState.Cruisered || state == PlayerState.Grounded) {
            _image.DisactiveSelf();
            _tokenSource?.Cancel();
        }
    }

    private Vector3 _lastPosition;
    private float _currentVisualSpeed;

    private async UniTask FlightAsync(CancellationToken token)
    {
        _lastPosition = _playerMovement.Transform.position;

        while (!token.IsCancellationRequested)
        {
            Vector3 verticalVelocityVector;

            // --- Вычисляем вертикальную скорость ---
            if (_playerMovement.IsBusted) // прыжок по кривой
            {
                float normalizedTime = _playerMovement.ExpandedTime / _playerMovement.SegmentDuration;
                float currentHeight = _playerMovement.CurrentCurve.Evaluate(normalizedTime) * _playerMovement.JumpHeight;

                // производная по времени (скорость подъема/спуска)
                float nextHeight = _playerMovement.CurrentCurve.Evaluate(normalizedTime + Time.fixedDeltaTime / _playerMovement.SegmentDuration)
                                   * _playerMovement.JumpHeight;
                verticalVelocityVector = Vector3.up * ((nextHeight - currentHeight) / Time.fixedDeltaTime);
            }
            else // обычный полет
            {
                Vector3 velocity = (_playerMovement.Transform.position - _lastPosition) / Time.deltaTime;
                verticalVelocityVector = new Vector3(0f, velocity.y, 0f);
            }

            _lastPosition = _playerMovement.Transform.position;

            float verticalVelocity = verticalVelocityVector.y;

            // --- Нормализуем для спидометра ---
            float targetSpeed = 0.5f - verticalVelocity * _slopeFactor;
            targetSpeed = Mathf.Clamp01(targetSpeed);

            // применяем кривую для плавного визуального отклика
            _normalizedSpeed = _speedVisualCurve.Evaluate(targetSpeed);

            // --- Плавное обновление UI ---
            
            _currentVisualSpeed = Mathf.MoveTowards(_currentVisualSpeed, _normalizedSpeed, Time.deltaTime * _smoothFactor);

            _image.fillAmount = _currentVisualSpeed;

            await UniTask.Yield(token);
        }
    }



    
    

    private void OnDestroy() {
        UniTaskHelper.DisposeTask(ref _tokenSource);
    }
}

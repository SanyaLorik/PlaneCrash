using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class Lift : MonoBehaviour {
    [SerializeField] private Renderer _rend;
    [SerializeField] private Renderer _transformRend;
    [SerializeField] private GameObject _liftPhysical;
    [SerializeField] private Rigidbody _playerRb;
    [SerializeField]  private float _speedUp = 10f; 
    
    private Vector3 _liftDownPosition; 
    private Vector3 _moneyEndPos;
    private CancellationTokenSource _tokenSource;
    private Rigidbody _liftRb;
    public bool _inLift;
    private Coroutine _liftDownRoutine;



    [Inject]
    public void Init(PlayerBank bank) {
        bank.BankChanged += BankOnBankChanged;
    }

    private void BankOnBankChanged(float amount) {
        CalculateMoneyDistance();
        
    }

    private void Awake() {
        _liftRb = _liftPhysical.GetComponent<Rigidbody>();
        _transformRend = transform.GetComponent<Renderer>();
        _liftDownPosition = transform.position;
    }

    private void Start() {
        CalculateMoneyDistance();
    }

    private void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.TryGetComponent(out PlayerMovement _)) {
            _inLift = true;
            
            if (_liftDownRoutine != null) {
                StopCoroutine(_liftDownRoutine);
            }

            if (_tokenSource == null && _liftGoingDown) {
                _liftGoingDown = false;
                ReadyLiftWork();
                LiftUp(_tokenSource.Token).Forget();
            }
        }    
    }

    
    private void OnTriggerExit(Collider collider) {
        if (collider.gameObject.TryGetComponent(out PlayerMovement _)) {
            _inLift = false;
            
            if (_liftDownRoutine != null) {
                StopCoroutine(_liftDownRoutine);
            }

            _liftDownRoutine = StartCoroutine(WaitChangeStateRoutine());
        }    
    }

    private IEnumerator WaitChangeStateRoutine() {
        yield return new WaitForSeconds(1.5f);
        if (!_inLift ) {
            
            ReadyLiftWork();
            LiftDown(_tokenSource.Token).Forget();
        }
        _liftDownRoutine =  null;
    }


    private void CalculateMoneyDistance() {
        StartCoroutine(CalculateMoneyHeightRoutine());
    }

    private IEnumerator CalculateMoneyHeightRoutine() {
        yield return new WaitForSeconds(1f);
        float targetTop = _rend.bounds.max.y;           // куда хотим приехать
        float liftBottom = _transformRend.bounds.min.y;  // где сейчас низ лифта
        float delta = targetTop - liftBottom;          // сколько реально надо ехать вверх
        _moneyEndPos = _liftDownPosition + Vector3.up * delta;
        Debug.Log("Высота куба для лифта: " + (_moneyEndPos.y));
        
        _liftRb.MovePosition(_liftDownPosition);
        _liftPhysical.transform.position = _liftDownPosition;
        _tokenSource = null;
    }

    
    private void ReadyLiftWork() {
        _tokenSource?.Cancel();
        _tokenSource?.Dispose();
        _tokenSource = new CancellationTokenSource();
    }
    

    private async UniTask LiftUp(CancellationToken token) {
        await UniTask.Delay(700, cancellationToken: token);
        if (Mathf.Approximately(transform.position.y, _moneyEndPos.y) || !_inLift) {
            return;
        }
        
        
        
        Vector3 _startPos = transform.position;
        Vector3 _endPosition = _moneyEndPos;
        
        
        Debug.Log($"Подьем лифта из {_startPos.y} в {_endPosition.y}");
       
        float duration = Math.Abs(_startPos.y - _endPosition.y) / _speedUp;
        float elapsedTime = 0f;
        
        Debug.Log("LiftUp duration" + duration);
        Debug.Log("LiftUp _endPosition" + _endPosition.y);

        while (elapsedTime < duration) {
            float t = elapsedTime / duration;
            t = Mathf.SmoothStep(0, 1, t);
            Vector3 newPos = Vector3.Lerp(_startPos, _endPosition, t);
            _liftRb.MovePosition(newPos);
            elapsedTime += Time.fixedDeltaTime;
            await UniTask.WaitForFixedUpdate(token);
        }
        ResetPlayerVelocity();
        _liftRb.MovePosition(_endPosition);
        _tokenSource = null;
    }

    private bool _liftGoingDown = true;
    private async UniTask LiftDown(CancellationToken token) {
        if (Mathf.Approximately(transform.position.y, _liftDownPosition.y) || _inLift) {
            return;
        }

        _liftGoingDown = true;
        Vector3 _startPos = transform.position;
        Vector3 _endPosition = _liftDownPosition;

        
        Debug.Log($"Опуск лифта из {_startPos.y} в {_endPosition.y}");
        
        
        float duration = Math.Abs(_startPos.y - _endPosition.y) / _speedUp;

        float elapsedTime = 0f;
        float durationForDown = duration / 1.5f;

        while (elapsedTime < durationForDown) {
            float t = elapsedTime / durationForDown;
            t = Mathf.SmoothStep(0, 1, t);
            Vector3 newPos = Vector3.Lerp(_startPos, _endPosition, t);
            _liftRb.MovePosition(newPos);
            
            elapsedTime += Time.fixedDeltaTime;
            await UniTask.WaitForFixedUpdate(token);
        }
        _liftRb.MovePosition(_endPosition);
        _liftPhysical.transform.position = _liftDownPosition;
        Debug.Log("Лифт опустился");
        _tokenSource = null;
    }

    private void ResetPlayerVelocity() {
        _playerRb.linearVelocity = new Vector3(_playerRb.linearVelocity.x, 0f, _playerRb.linearVelocity.z);
    }
    
    
}

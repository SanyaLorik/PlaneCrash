using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Renderer))]
public class Lift : MonoBehaviour {
    [SerializeField] private Renderer _rend;
    [SerializeField] private Renderer _transformRend;
    [SerializeField] private GameObject _liftPhysical;
    [SerializeField] private Rigidbody _playerRb;
    [SerializeField]  private float _timeToUp = 10f; 
    
    private Vector3 _liftDownPosition; 
    private Vector3 _moneyEndPos;
    private CancellationTokenSource _tokenSource;
    private Rigidbody _liftRb;
    public bool _inLift;



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
            ReadyLiftWork();
            LiftUp(_tokenSource.Token).Forget();
        }    
    }

    
    private void OnTriggerExit(Collider collider) {
        if (collider.gameObject.TryGetComponent(out PlayerMovement _)) {
            _inLift = false;
            ReadyLiftWork();
            LiftDown(_tokenSource.Token).Forget();
        }    
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

        float elapsedTime = 0f;


        while (elapsedTime < _timeToUp) {
            float t = elapsedTime / _timeToUp;
            float y = Mathf.Lerp(_startPos.y, _endPosition.y, t);
            Vector3 newPos = new Vector3(_startPos.x, y, _startPos.z);
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
        
        

        float elapsedTime = 0f;

        while (elapsedTime < _timeToUp/2) {
            float t = elapsedTime / _timeToUp/2;
            float y = Mathf.Lerp(_startPos.y, _endPosition.y, t);
            Vector3 newPos = new Vector3(_startPos.x, y, _startPos.z);
            _liftRb.MovePosition(newPos);
            
            elapsedTime += Time.fixedDeltaTime;
            await UniTask.WaitForFixedUpdate(token);
        }
        _liftRb.MovePosition(_endPosition);
        ResetPlayerVelocity();
        _liftPhysical.transform.position = _liftDownPosition;
        Debug.Log("Лифт опустился");
        _tokenSource = null;
    }

    private void ResetPlayerVelocity() {
        _playerRb.linearVelocity = new Vector3(_playerRb.linearVelocity.x, 0f, _playerRb.linearVelocity.z);
    }
    
    
}

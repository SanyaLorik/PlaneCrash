using UnityEngine;
using Zenject;

[RequireComponent(typeof(Renderer))]
public class Lift : MonoBehaviour {
    [SerializeField] private Transform _topPointMoneyCube;
    [SerializeField] private Transform _liftTransform;

    [SerializeField] private float _speed = 10f;
    [SerializeField] private float _downSpeed = 10f;
    [SerializeField] private float _waitTimerToGo = 0.7f;


    private Vector3 _liftDownPosition;
    private Vector3 _moneyEndPos => new Vector3(_liftDownPosition.x, _topPointMoneyCube.position.y, _liftDownPosition.z);

    private PlayerMovement _currentPlayer;
    
    [Inject] private PlayerBank _bank;

    private void Awake()
    {
        _liftDownPosition = _liftTransform.position;
    }
    

    private enum LiftState
    {
        Idle,
        WaitingUp,
        MovingUp,
        MovingDown
    }

    private LiftState _state = LiftState.Idle;

    private float _moveTimer;
    private float _waitTimer;

    private Vector3 _startPos;
    private Vector3 _targetPos;
    private Vector3 _previousPos;



    private void Update()
    {
        switch (_state)
        {
            case LiftState.WaitingUp:
                _waitTimer += Time.deltaTime;
                if (_waitTimer >= _waitTimerToGo)
                {
                    StartMove(_moneyEndPos, _speed);
                    _state = LiftState.MovingUp;
                }
                break;

            case LiftState.MovingUp:
                MoveLift();
                break;

            case LiftState.MovingDown:
                MoveLift();
                break;
        }
    }

    private void StartMove(Vector3 target, float speed) {
        _moveTimer = 0f;
        _startPos = _liftTransform.position;
        _targetPos = target;
        _previousPos = _startPos;
        _currentMoveDuration = Mathf.Abs(_startPos.y - _targetPos.y) / speed;
    }

    private float _currentMoveDuration;

    private void MoveLift()
    {
        _moveTimer += Time.deltaTime;
        float t = Mathf.Clamp01(_moveTimer / _currentMoveDuration);

        float y = Mathf.Lerp(_startPos.y, _targetPos.y, t);
        Vector3 newPos = new Vector3(_startPos.x, y, _startPos.z);

        Vector3 delta = newPos - _previousPos;

        _liftTransform.position = newPos;

        if (_currentPlayer != null)
        {
            _currentPlayer.AddExternalMotion(delta);
        }

        _previousPos = newPos;

        if (t >= 1f)
        {
            if (_state == LiftState.MovingUp)
                _state = LiftState.Idle;
            else if (_state == LiftState.MovingDown)
                _state = LiftState.Idle;
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.TryGetComponent(out PlayerMovement player))
        {
            _currentPlayer = player;
            _waitTimer = 0f;
            _state = LiftState.WaitingUp;
            player.SetLiftState(true);

        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.TryGetComponent(out PlayerMovement player) && player == _currentPlayer)
        {
            _currentPlayer = null;
            StartMove(_liftDownPosition, _downSpeed);
            _state = LiftState.MovingDown;
            player.SetLiftState(false);
        }
    }
}

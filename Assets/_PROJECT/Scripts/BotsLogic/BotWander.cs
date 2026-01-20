using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using SanyaBeerExtension;
using UnityEngine;
using UnityEngine.AI;
using Zenject;
using Random = UnityEngine.Random;

public class BotWander : MonoBehaviour, IBotBehaviour {
    [SerializeField] private bool _eblaning = true;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private PairedValue<float> _timeToStay;
    
    [SerializeField] private Transform _moneyCube;
    [SerializeField] private Transform _betZoneCube;
    
    
    private NavMeshAgent _agent;
    
    private PlayerMovement _playerMovement;
    private PlayerStateManager _playerStateManager;
    private PlayerConfig _playerConfig;
    private CancellationTokenSource _botTokenSource;
    private Transform _chooseCube;
    private Rigidbody _rb;

    
    
    [Inject] 
    public void Init(PlayerMovement playerMovement, PlayerStateManager playerStateManager, PlayerConfig playerConfig) {
        _playerMovement = playerMovement;
        _playerStateManager = playerStateManager;
        _playerConfig = playerConfig;
    }



    private void Awake() {
        _agent = GetComponent<NavMeshAgent>();
        _rb =  GetComponent<Rigidbody>();
    }

    private void Start() {
        Enter();
    }


    public void Enter() {
        Exit();
        _agent.enabled = true;
        _botTokenSource = new CancellationTokenSource();
        _eblaning = true;
        LifeCycleAsync(_botTokenSource.Token).Forget();
    }
    
    public void Exit() {
        _botTokenSource?.Cancel();
        _botTokenSource?.Dispose();
        _botTokenSource =  null;
        _agent.SafeStop();
        _agent.enabled = false;
        _eblaning = false;
    }



    private async UniTask LifeCycleAsync(CancellationToken token) {
        while (!token.IsCancellationRequested) {
            Vector3 target = ChooseNextTarget();

            _agent.SetDestination(target);
            
            Jump(token).Forget();

            await UniTask.WaitUntil(() => !_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance,
                cancellationToken: token);
            await RotateTowardsAsync(target, _rotationSpeed, token);

            float waitTime = Random.Range(_timeToStay.From, _timeToStay.To);
            await UniTask.Delay(TimeSpan.FromSeconds(waitTime), cancellationToken: token);
        }
    }

    private async UniTask Jump(CancellationToken token) {
        await UniTask.WaitUntil(() => !_agent.pathPending && _agent.hasPath, cancellationToken: token);
            
        float startPathLength = _agent.remainingDistance;
        float jumpLength = startPathLength / Random.Range(1.2f, 3f);

        await UniTask.WaitUntil(() => 
                !_agent.pathPending &&
                _agent.remainingDistance <= jumpLength &&
                _agent.remainingDistance > _agent.stoppingDistance, 
            cancellationToken: token);
            
        _agent.updatePosition = false;
        _agent.updateRotation = false;

        FakeJump(token);
        Debug.Log("Прыжок");
        
        await UniTask.Delay(200, cancellationToken: token);

        _agent.updatePosition = true;
        _agent.updateRotation = true;
    }
    
    [SerializeField] private float _jumpDuration;
    private async UniTask FakeJump(CancellationToken token) {
        float height = _playerConfig.JumpHeight/1.5f;

        float t = 0f;
        Vector3 basePos;

        while (t < _jumpDuration) {
            t += Time.deltaTime;
            float normalized = t / _jumpDuration;

            float yOffset = Mathf.Sin(normalized * Mathf.PI) * height;

            basePos = _agent.nextPosition;
            transform.position = new Vector3(basePos.x, basePos.y + yOffset, basePos.z);

            await UniTask.Yield(PlayerLoopTiming.Update, token);
        }
    }


    private Vector3 ChooseNextTarget() {
        float rv = Random.value;
        if (_playerStateManager.CurrentState == PlayerState.Walking &&  rv > 0.7f)
            return _playerMovement.transform.position;

        // Иначе выбираем случайный куб
        return Random.value > 0.5f ? GetTargetPointNearCube(_moneyCube) : GetTargetPointNearCube(_betZoneCube);
    }
    
    private Vector3 GetTargetPointNearCube(Transform cube) {
        Vector3 size = cube.localScale;

        float offsetX = Random.Range(-size.x/2f - 2f, size.x/2f + 2f);
        float offsetZ = Random.Range(-size.z/2f - 2f, size.z/2f + 2f);

        Vector3 target = cube.position + new Vector3(offsetX, 0f, offsetZ);

        NavMeshHit hit;
        if (NavMesh.SamplePosition(target, out hit, 1f, NavMesh.AllAreas)) {
            return hit.position;
        }

        // Если не нашли на навмеш, просто центр куба
        return cube.position;
    }

    
    private async UniTask RotateTowardsAsync(Vector3 target, float rotationSpeed, CancellationToken token) {
        Vector3 direction = (target - transform.position).normalized;
        direction.y = 0; // Игнорируем разницу по высоте
    
        if (direction == Vector3.zero) return;
    
        Quaternion targetRotation = Quaternion.LookRotation(direction);
    
        // Плавный поворот
        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.5f && !token.IsCancellationRequested) {
            transform.rotation = Quaternion.Slerp(
                transform.rotation, 
                targetRotation, 
                rotationSpeed * Time.deltaTime
            );
            await UniTask.Yield(token);
        }
    }
    
    private void OnDestroy() {
        _botTokenSource?.Cancel();
        _botTokenSource?.Dispose();
    }
    
}

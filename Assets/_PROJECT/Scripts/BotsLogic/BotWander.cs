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
    private CancellationTokenSource _botTokenSource;
    private Transform _chooseCube;

    
    
    [Inject] 
    public void Init(PlayerMovement playerMovement, PlayerStateManager playerStateManager) {
        _playerMovement = playerMovement;
        _playerStateManager = playerStateManager;
    }



    private void Awake() {
        _agent = GetComponent<NavMeshAgent>();
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

            await UniTask.WaitUntil(() => !_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance,
                cancellationToken: token);
            await RotateTowardsAsync(target, _rotationSpeed, token);

            float waitTime = Random.Range(_timeToStay.From, _timeToStay.To);
            await UniTask.Delay(TimeSpan.FromSeconds(waitTime), cancellationToken: token);
        }
    }

    
    private Vector3 ChooseNextTarget() {
        if (_playerStateManager.CurrentState == PlayerState.Walking && Random.value > 0.6f)
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

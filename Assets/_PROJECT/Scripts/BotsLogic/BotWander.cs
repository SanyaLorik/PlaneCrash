using System;
using System.Collections.Generic;
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
    
    [SerializeField] private List<Transform> _pointsToWalks;
    
    [Header("Партиклы")]
    [SerializeField] private JumpParticlesController _jumpParticlesController;
    [SerializeField] private JumpParticlesController _landParticleController;
    [SerializeField] private DualLegParticles _walkingParticles;
    [Range(0,1), SerializeField] private float _botChanceToJump = 0.85f; 
    
    
    
    public Action<bool> StartWandering;
    public Action OnJump;
    
    
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
        MonitorMovementAsync(_botTokenSource.Token).Forget();
    }
    
    public void Exit() {
        _botTokenSource?.Cancel();
        _botTokenSource?.Dispose();
        _botTokenSource =  null;
        _agent.SafeStop();
        _agent.enabled = false;
        _eblaning = false;
    }

    private async UniTask MonitorMovementAsync(CancellationToken token) {
        while (!token.IsCancellationRequested) {
            if (_agent.enabled && _agent.velocity.sqrMagnitude > 0.05f) {
                if (!_walkingParticles.IsPlaying) {
                    _walkingParticles.Play();
                    StartWandering?.Invoke(true);
                }
            }
            else {
                if (_walkingParticles.IsPlaying) {
                    _walkingParticles.Stop();
                    StartWandering?.Invoke(false);
                }
            }

            await UniTask.Yield(token);
        }
    }


    private async UniTask LifeCycleAsync(CancellationToken token) {
        while (!token.IsCancellationRequested) {
            
            Vector3 target = ChooseNextTarget();
            _agent.SetDestination(target);
            
            await UniTask.WaitUntil(() => !_agent.pathPending && _agent.hasPath, cancellationToken: token);
            Jump(token).Forget(
                );

            await UniTask.WaitUntil(() => 
                !_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance,
                cancellationToken: token);
            
            await RotateTowardsAsync(target, _rotationSpeed, token);

            float waitTime = Random.Range(_timeToStay.From, _timeToStay.To);
            await UniTask.Delay(TimeSpan.FromSeconds(waitTime), cancellationToken: token);
        }
    }

    private async UniTask Jump(CancellationToken token) {
        if (Random.value > _botChanceToJump) return;
        
        float startPathLength = _agent.remainingDistance;
        float jumpLength = startPathLength / Random.Range(1.5f, 2f);

        await UniTask.WaitUntil(() => 
                !_agent.pathPending &&
                _agent.remainingDistance <= jumpLength &&
                _agent.remainingDistance > _agent.stoppingDistance, 
            cancellationToken: token);

        FakeJump(token).Forget();
    }
    
    [SerializeField] private float _jumpDuration;
    private async UniTask FakeJump(CancellationToken token) {
        float height = _playerConfig.JumpHeight / 1.5f;
        float t = 0f;

        _jumpParticlesController.Play();
        OnJump?.Invoke();
        while (t < _jumpDuration) {
            t += Time.deltaTime;
            float normalized = t / _jumpDuration;
            float yOffset = Mathf.Sin(normalized * Mathf.PI) * height;

            Vector3 pos = transform.position;
            pos.y = _agent.nextPosition.y + yOffset;

            transform.position = pos;

            await UniTask.Yield(token);
        }

        _landParticleController.Play();
    }

    

    private Vector3 ChooseNextTarget() {
        float rv = Random.value;
        if (_playerStateManager.CurrentState == PlayerState.Walking &&  rv > 0.7f)
            return _playerMovement.transform.position;

        // Иначе выбираем случайный куб
        return GetTargetPoint(_pointsToWalks[Random.Range(0, _pointsToWalks.Count)]);
    }
    
    private Vector3 GetTargetPoint(Transform point) {
        Vector3 size = point.localScale;

        float offsetX = Random.Range(-size.x/2f - 2f, size.x/2f + 2f);
        float offsetZ = Random.Range(-size.z/2f - 2f, size.z/2f + 2f);

        Vector3 target = point.position + new Vector3(offsetX, 0f, offsetZ);

        NavMeshHit hit;
        if (NavMesh.SamplePosition(target, out hit, 1f, NavMesh.AllAreas)) {
            return hit.position;
        }

        // Если не нашли на навмеш, просто центр куба
        return point.position;
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

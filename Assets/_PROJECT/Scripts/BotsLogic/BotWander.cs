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
    [SerializeField] private PairedValue<float> _nextTimeChangeTarget;
    
    [SerializeField] private Transform _moneyCube;
    [SerializeField] private Transform _betZoneCube;
    
    
    private NavMeshAgent agent;
    private Vector3 spawnAreaCenter;
    private float wanderRadius = 10f;
    
    private PlayerMovement _playerMovement;
    private PlayerStateManager _playerStateManager;
    private CancellationTokenSource _botTokenSource;
    private Transform chooseCube;

    [Inject] 
    public void Init(PlayerMovement playerMovement, PlayerStateManager playerStateManager) {
        _playerMovement = playerMovement;
        _playerStateManager = playerStateManager;
    }
    
    
    private void Awake() {
        agent = GetComponent<NavMeshAgent>();
        Debug.Log("GetComponent<NavMeshAgent>();");
    }

    private void Start() {
        Enter();
    }


    public void Enter() {
        Exit();
        agent.enabled = true;
        _botTokenSource = new CancellationTokenSource();
        _eblaning = true;
        LifeCycleAsync(_botTokenSource.Token).Forget();
    }
    
    public void Exit() {
        _botTokenSource?.Cancel();
        _botTokenSource?.Dispose();
        _botTokenSource =  null;
        agent.SafeStop();
        agent.enabled = false;
        _eblaning = false;
    }



    private async UniTask LifeCycleAsync(CancellationToken token) {
        while (_eblaning && !token.IsCancellationRequested) {
            if (Random.value > 0.6 && _playerStateManager.CurrentState != PlayerState.Flight) {
                GoToPlayer(token);
                await UniTask.Delay(
                    (int)(1000 * Random.Range(_nextTimeChangeTarget.From, _nextTimeChangeTarget.To)),
                    cancellationToken: token
                );
                await RotateTowardsAsync(_playerMovement.transform, _rotationSpeed, token);
            }
            else {
                GoToCube(token);
                await UniTask.Delay(
                    (int)(1000 * Random.Range(_nextTimeChangeTarget.From, _nextTimeChangeTarget.To)),
                    cancellationToken: token
                );
                await RotateTowardsAsync(chooseCube, _rotationSpeed, token);
            }
        }
    }

    private void GoToPlayer(CancellationToken token) {
        Debug.Log("Идем за игроком");
        agent.SetDestinationSafety(_playerMovement.transform.position, token);
    }


    private void GoToCube(CancellationToken token) {
        Debug.Log("Идем к кубу");
        chooseCube = Random.value > 0.5 ?  _moneyCube : _betZoneCube;
        Vector3 cubePosition = chooseCube.position;
       
        float _minFigureDistance = Mathf.Max(chooseCube.localScale.x, chooseCube.localScale.z);
        float distance = _minFigureDistance;

        Vector3 direction = Random.onUnitSphere; // случайное направление
        direction.y = 0; 
        Vector3 newCubeSpawn = cubePosition + direction.normalized * distance;
        
        agent.SetDestinationSafety(newCubeSpawn, token);
    }
    
    
    private async UniTask RotateTowardsAsync(Transform target, float rotationSpeed, CancellationToken token) {
        Vector3 direction = (target.position - transform.position).normalized;
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

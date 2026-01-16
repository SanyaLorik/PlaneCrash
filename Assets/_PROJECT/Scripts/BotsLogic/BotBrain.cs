using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using SanyaBeerExtension;
using UnityEngine;
using UnityEngine.AI;
using Zenject;
using Random = UnityEngine.Random;

public class BotBrain : MonoBehaviour {
    [SerializeField] private bool _eblaning = true;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private PairedValue<float> _nextTimeChangeTarget;
    
    
    [SerializeField] private Transform _moneyCube;
    [SerializeField] private Transform _betZoneCube;
    
    
    
    private NavMeshAgent agent;
    private Vector3 spawnAreaCenter;
    private float wanderRadius = 10f;
    
    private PlayerMovement _playerMovement;
    private CancellationTokenSource _botTokenSource;

    [Inject]
    public void Init(PlayerMovement playerMovement) {
        _playerMovement = playerMovement;
    }

    public void StopBotEblaning() {
        _botTokenSource?.Cancel();
        _botTokenSource?.Dispose();
        _botTokenSource =  null;
        agent.SafeStop();
        agent.enabled = false;
        _eblaning = false;
        Debug.Log("StopBotEblaning");
    }

    public void StartBotEblaning() {
        StopBotEblaning();
        agent.enabled = true;
        _botTokenSource = new CancellationTokenSource();
        _eblaning = true;
        LifeCycle(_botTokenSource.Token).Forget();
    }

    private void Start() {
        agent = GetComponent<NavMeshAgent>();
        StartBotEblaning(); 
    }

    private async UniTask LifeCycle(CancellationToken token) {
        while (_eblaning && !token.IsCancellationRequested) {
            if (Random.value > 0.6) {
                GoToPlayer(token);
                await UniTask.Delay(
                    (int)(1000 * Random.Range(_nextTimeChangeTarget.From, _nextTimeChangeTarget.To)),
                    cancellationToken: token
                );
                await RotateTowards(_playerMovement.transform, _rotationSpeed, token);
            }
            else {
                GoToCube(token);
                await UniTask.Delay(
                    (int)(1000 * Random.Range(_nextTimeChangeTarget.From, _nextTimeChangeTarget.To)),
                    cancellationToken: token
                );
                await RotateTowards(chooseCube, _rotationSpeed, token);
            }
        }
    }

    private void GoToPlayer(CancellationToken token) {
        Debug.Log("Идем за игроком");
        agent.SetDestinationSafety(_playerMovement.transform.position, token);
    }


    private Transform chooseCube;
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
    
    
    private async UniTask RotateTowards(Transform target, float rotationSpeed, CancellationToken token) {
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

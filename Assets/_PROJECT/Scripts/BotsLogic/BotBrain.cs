using System;
using Cysharp.Threading.Tasks;
using SanyaBeerExtension;
using UnityEngine;
using UnityEngine.AI;
using Zenject;
using Random = UnityEngine.Random;

public class BotBrain : MonoBehaviour {
    [SerializeField] private bool _followPlayer = true;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private PairedValue<float> _nextTimeChangeTarget;
    
    
    [SerializeField] private Transform _moneyCube;
    [SerializeField] private Transform _betZoneCube;
    
    
    
    private NavMeshAgent agent;
    private Vector3 spawnAreaCenter;
    private float wanderRadius = 10f;
    
    private PlayerMovement _playerMovement;
    

    [Inject]
    public void Init(PlayerMovement playerMovement) {
        _playerMovement = playerMovement;
    }
    

    private void Start() {
        agent = GetComponent<NavMeshAgent>();
        
        LifeCycle().Forget();
    }

    private async UniTask LifeCycle() {
        while (_followPlayer) {
            if (Random.value > 0.6) {
                GoToPlayer();
                await UniTask.Delay((int)(1000 * Random.Range(_nextTimeChangeTarget.From, _nextTimeChangeTarget.To)));
                await RotateTowards(_playerMovement.transform, _rotationSpeed);
                
            }

            GoToCube();
            await UniTask.Delay((int)(1000 * Random.Range(_nextTimeChangeTarget.From, _nextTimeChangeTarget.To)));
            await RotateTowards(chooseCube, _rotationSpeed);
            
        }
    }

    private void GoToPlayer() {
        Debug.Log("Идем за игроком");
        RotateTowards(_playerMovement.transform, _rotationSpeed);
        agent.SetDestination(_playerMovement.transform.position);
    }


    private Transform chooseCube;
    private void GoToCube() {
        Debug.Log("Идем к кубу");
        chooseCube = Random.value > 0.5 ?  _moneyCube : _betZoneCube;
        Vector3 cubePosition = chooseCube.position;
       
        float _minFigureDistance = Mathf.Max(chooseCube.localScale.x, chooseCube.localScale.z);
        float distance = _minFigureDistance;

        Vector3 direction = Random.onUnitSphere; // случайное направление
        direction.y = 0; 
        Vector3 newCubeSpawn = cubePosition + direction.normalized * distance;
        
        agent.SetDestination(newCubeSpawn);
    }
    
    
    private async UniTask RotateTowards(Transform target, float rotationSpeed = 5f) {
        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0; // Игнорируем разницу по высоте
    
        if (direction == Vector3.zero) return;
    
        Quaternion targetRotation = Quaternion.LookRotation(direction);
    
        // Плавный поворот
        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.5f) {
            Debug.Log("Поворот: " + transform.rotation);
            transform.rotation = Quaternion.Slerp(
                transform.rotation, 
                targetRotation, 
                rotationSpeed * Time.deltaTime
            );
            await UniTask.Yield();
        }
    }
    
}

using System;
using System.Collections;
using System.Collections.Generic;
using ModestTree;
using SanyaBeerExtension;
using UnityEditor;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class MoneyRadiusSpawn : MonoBehaviour {
    [SerializeField] private float _spawnRadius;
    [SerializeField] private float _spawnCount;
    [SerializeField] private GameObject _spawnObject;
    [SerializeField] private Transform _bottomPoint;
    [SerializeField] private float _yCorrection;
    [SerializeField] private float _minDistance;
    [SerializeField] private float _countLevels;
    [Range(0,1), SerializeField] private float _newLevelProll;


    private Vector3 _spawnPoint;
    
    
    [Inject] ObjectPoolManager _poolManager;
    
    private void Awake() {
        _spawnPoint = transform.position;
        SpawnInAllGame();
    }

    private List<Transform> _moneys = new ();
    private void SpawnInAllGame() {
        for (int i = 0; i < _spawnCount; i++) {
            var money = _poolManager.Spawn<Transform>(_spawnObject, Vector3.zero, PoolType.MoneyNearCube);
            _moneys.Add(money);
        }
    }


    public void SpawnMoney() {
        _minDistance = Mathf.Max(transform.localScale.x, transform.localScale.z);
        // Debug.Log("SpawnMoney");
        for (int i = 0; i < _spawnCount; i++) {
            float minDist = Mathf.Min(_minDistance, _spawnRadius * 0.99f); // чтобы не выйти за предел
            float distance = Random.Range(minDist, _spawnRadius);

            Vector3 direction = Random.onUnitSphere; // случайное направление
            direction.y = 0; 
            Vector3 newSpawn = _spawnPoint + direction.normalized * distance;

            newSpawn.y = _bottomPoint.position.y + _yCorrection + Random.Range(-0.0005f, 0.0005f);

            Transform newObj = _moneys[i];
            newObj.position = newSpawn;
            
            newObj.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
            
            for (int j = 1; j < _countLevels; j++) {
                if (Random.value > _newLevelProll && i < _spawnCount) {
                    newObj = SpawnUpperObject(newObj, i);
                    i++;
                }
            }
        }
    }
    
    private Transform SpawnUpperObject(Transform obj, int index) {
        Physics.SyncTransforms();
        var prefabRenderer = obj.GetComponentInChildren<Renderer>();
        Bounds bounds1 = prefabRenderer.bounds;
        
        
        var objRenderer = obj.GetComponentInChildren<Renderer>();
        Bounds bounds2 = objRenderer.bounds;
        
        
        float topY = bounds1.max.y + bounds2.extents.y;
        Vector3 spawnAbove = new Vector3(
            bounds1.center.x,
            topY,
            bounds1.center.z
        );
        
        // Я чето не понял нахуй его еще один брать и присваивать туда ну лан
        obj = _moneys[index];
        obj.transform.position = spawnAbove;
        obj.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
        return obj;
    }
}

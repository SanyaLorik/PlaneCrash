using System;
using System.Collections.Generic;
using ModestTree;
using UnityEngine;
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
    private List<GameObject> _spawnList = new();
    
    private void Start() {
        _spawnPoint = transform.position;
    }
    

    private void DeleteOldObjects() {
        foreach (var obj in _spawnList) {
            Destroy(obj);
        }
        _spawnList.Clear();
    }

    public void SpawnMoney() {
        DeleteOldObjects();
        
        _minDistance = Mathf.Max(transform.localScale.x, transform.localScale.z);
        for (int i = 0; i < _spawnCount; i++) {
            float minDist = Mathf.Min(_minDistance, _spawnRadius * 0.99f); // чтобы не выйти за предел
            float distance = Random.Range(minDist, _spawnRadius);

            Vector3 direction = Random.onUnitSphere; // случайное направление
            direction.y = 0; 
            Vector3 newSpawn = _spawnPoint + direction.normalized * distance;

            newSpawn.y = _bottomPoint.position.y + _yCorrection + Random.Range(-0.005f, 0.005f);

            GameObject newObj = Instantiate(_spawnObject, newSpawn, Quaternion.identity);
            newObj.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
            _spawnList.Add(newObj);
            
            for (int j = 1; j < _countLevels; j++) {
                if (Random.value > _newLevelProll) {
                    newObj = SpawnUpperObject(newObj);
                }
            }
        }
    }
    
    private GameObject SpawnUpperObject(GameObject obj) {
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
        obj = Instantiate(_spawnObject, spawnAbove, Quaternion.identity);
        obj.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
        _spawnList.Add(obj);
        return obj;
    }
}

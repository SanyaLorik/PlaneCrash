using System;
using System.Collections;
using System.Collections.Generic;
using ModestTree;
using SanyaBeerExtension;
using UnityEditor;
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
    
    
    private Queue<GameObject> _moneyPool = new ();
    
    
    private void Awake() {
        _spawnPoint = transform.position;

        StartCoroutine(SpawnMoneyRoutine());
    }

    private IEnumerator SpawnMoneyRoutine() {
        for (int i = 0; i < _spawnCount; i++) {
            GameObject money = Instantiate(_spawnObject, Vector3.zero, Quaternion.identity, _bottomPoint);
         
            
            _moneyPool.Enqueue(money);
            yield return null;
        }
    }
    
    private GameObject GetMoneyFromPool() {
        GameObject obj;
        if (_moneyPool.Count > 0) {
            obj = _moneyPool.Dequeue();
            obj.ActiveSelf();
        }
        else {
            obj = Instantiate(_spawnObject, Vector3.zero, Quaternion.identity, _bottomPoint);
        }

        obj.transform.localScale = Vector3.one;
        obj.transform.rotation = Quaternion.identity;

        return obj;
    }
    
    private void ReturnToPool(GameObject money) {
        _moneyPool.Enqueue(money);
    }



    public void SpawnMoney() {
        _minDistance = Mathf.Max(transform.localScale.x, transform.localScale.z);
        for (int i = 0; i < _spawnCount; i++) {
            float minDist = Mathf.Min(_minDistance, _spawnRadius * 0.99f); // чтобы не выйти за предел
            float distance = Random.Range(minDist, _spawnRadius);

            Vector3 direction = Random.onUnitSphere; // случайное направление
            direction.y = 0; 
            Vector3 newSpawn = _spawnPoint + direction.normalized * distance;

            newSpawn.y = _bottomPoint.position.y + _yCorrection + Random.Range(-0.005f, 0.005f);


            GameObject newObj = GetMoneyFromPool();
            newObj.transform.position = newSpawn;
            
            
            newObj.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
            
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
        
        // Я чето не понял нахуй его еще один брать и присваивать туда ну лан
        obj = GetMoneyFromPool();
        obj.transform.position = spawnAbove;
        
        obj.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
        return obj;
    }
}

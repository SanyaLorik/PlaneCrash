using System;
using System.Collections.Generic;
using ModestTree;
using UnityEngine;
using Random = UnityEngine.Random;

public class MoneyRadiusSpawn : MonoBehaviour {
    [SerializeField] private PlayerBank _bank;
    [SerializeField] private float _spawnRadius;
    [SerializeField] private float _spawnCount;
    [SerializeField] private GameObject _spawnObject;
    [SerializeField] private Transform _bottomPoint;
    [SerializeField] private float _yCorrection;
    [SerializeField] private float _minDistance;


    private Vector3 _spawnPoint;
    private List<GameObject> _spawnList = new();
    
    private void Start() {
        _bank.OnBankChanged += OnBankChanged;
        _spawnPoint = transform.position;
    }

    private void OnBankChanged(float money) {
        DeleteOldObjects();
        SpawnMoney();
    }

    private void DeleteOldObjects() {
        foreach (var obj in _spawnList) {
            Destroy(obj);
        }
        _spawnList.Clear();
    }

    private void SpawnMoney() {
        for (int i = 0; i < _spawnCount; i++) {
            float minDist = Mathf.Min(_minDistance, _spawnRadius * 0.99f); // чтобы не выйти за предел
            float distance = Random.Range(minDist, _spawnRadius);

            Vector3 direction = Random.onUnitSphere; // случайное направление
            direction.y = 0; 
            Vector3 newSpawn = _spawnPoint + direction.normalized * distance;

            newSpawn.y = _bottomPoint.position.y + _yCorrection + Random.Range(-0.005f, 0.005f);

            Debug.Log(newSpawn);
            GameObject newObj = Instantiate(_spawnObject, newSpawn, Quaternion.identity);
            newObj.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
            _spawnList.Add(newObj);
        }
    }
}

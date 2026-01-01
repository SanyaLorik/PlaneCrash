using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoostSpawner : MonoBehaviour {
    [SerializeField] private Transform _player;

    [SerializeField] private GameObject _boostPrefab;
    [SerializeField] private GameObject _unboostPrefab;
    
    [Header("Граница спауна")]
    [SerializeField] private float _xRightMax;
    [SerializeField] private float _xLeftMax;
    
    [SerializeField] private float _yMax;
    [SerializeField] private float _yMin;

    [SerializeField] private float _deleteDistance = 50f;
    
    
    private List<GameObject> _boostList = new ();
    private float spawnAhead = 300f;
    private float spawnZ;
    
    private void Update() {
        while (spawnZ < _player.position.z + spawnAhead) {
            SpawnBoost(spawnZ);
            spawnZ += 20f;
        }
        DeleteOldBoosts();
    }


    
    private void SpawnBoost(float z) {
        float x = Random.Range(_xLeftMax, _xRightMax);
        
        // Эту нужно находить относительно высоты пользователя
        float y = Random.Range(Mathf.Max(_yMin, _player.position.y - 10f), Mathf.Min(_yMax, _player.position.y + 10f));
    
        GameObject prefab = Random.value > 0.4f ? _boostPrefab : _unboostPrefab;
        
        GameObject newboost =  Instantiate(prefab, new Vector3(x, y, z), prefab.transform.rotation);
        _boostList.Add(newboost);
    }


    private void DeleteOldBoosts() {
        for (int i = _boostList.Count-1; i >= 0; i--) {
            if (_boostList[i] != null) {
                _boostList.RemoveAt(i);
                return;
            }
            if (_boostList[i].transform.position.z < _player.position.z - _deleteDistance) {
                Destroy(_boostList[i]);
                _boostList.RemoveAt(i);
            }
        }
    }

    
}

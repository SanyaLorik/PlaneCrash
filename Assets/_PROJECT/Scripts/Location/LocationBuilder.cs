using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using NaughtyAttributes;
using Zenject;


public class LocationBuilder : MonoBehaviour {
    [SerializeField] private List<LocationModule>  _buildingModulesPrefabs;
    [SerializeField] private float _distanceToFill = 500f;
    [SerializeField] private Transform _firstPoint;

    private Vector3 _lastEnd;
    [Inject] private ObjectPoolManager _poolManager;
    [Inject] private DiContainer _diContainer;
    
    
    
    private void Start() {
        _lastEnd = _firstPoint.position;
        ReBuildPoints();
    }

    
    
    private IEnumerator ConstructRoutine() {
        while (_lastEnd.z < _distanceToFill) {
            SpawnNext();
            yield return null;
        }

        Debug.Log("_createdModules.count = " +  _buildingModulesPrefabs.Count);
        foreach (var module in _createdModules) {
            yield return null;
            module.GenerateProps();
        }
    }

    
    [Button]
    public void ReBuildPoints() {
        StartCoroutine(ConstructRoutine());
    } 
    
    
    private List<LocationModule> _createdModules = new ();
    private void SpawnNext() {
        var prefab = _buildingModulesPrefabs[Random.Range(0, _buildingModulesPrefabs.Count)];
        
        LocationModule module = _poolManager.Spawn<LocationModule>(prefab.gameObject, Vector3.zero, PoolType.LocationObject);
        _createdModules.Add(module);
        module.Init(_diContainer);
        
        // Offset
        Vector3 offset = module.transform.position - module.Start.position;
        module.transform.position = _lastEnd + offset;
        
        _lastEnd = module.End.position;
    }
    

}

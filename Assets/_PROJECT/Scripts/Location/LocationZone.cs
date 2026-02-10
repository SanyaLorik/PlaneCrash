using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public enum CubeFace {
    Floor,
    LeftWall,
    RightWall
}
public class LocationZone : MonoBehaviour {
    [SerializeField] private CubeFace _locationType;
    [SerializeField] private int _spawnPointCount;
    [SerializeField] private float _minPointDistance;
    [SerializeField] private Renderer _renderer;
    [SerializeField] private List<GameObject> _objectsToSpawn;
    
    
    private Bounds _bounds;

    [Inject] private ObjectPoolManager _poolManager;
    
    
    private void Start() {
        _bounds = _renderer.bounds;
        Debug.Log(_poolManager);
    }
    
    

    private List<GameObject> _objects = new ();

    public void GenerateProps() {
        foreach (var obj in _objects) {
            _poolManager.ReturnObjectToPool(obj, PoolType.Props);
        }
        _objects.Clear();
        GeneratePoints(_bounds, _spawnPointCount, _minPointDistance);
    }
    
    
    private void GeneratePoints(Bounds bounds, int count, float minDistance) {
        int attempts = 0;
        List<Vector3> points = new ();

        while (_objects.Count < count && attempts < count * 50) {
            attempts++;

            Vector3 point = RandomPointOnFace(bounds, _locationType);

            bool valid = true;
            foreach (var other in points) {
                if (Vector3.Distance(point, other) < minDistance) {
                    valid = false;
                    break;
                }
            }

            if (valid) {
                Debug.Log("Спавн точки в " + point);
                points.Add(point);
                var obj = _poolManager.Spawn<Transform>(
                    _objectsToSpawn[Random.Range(0, _objectsToSpawn.Count)], 
                    point, 
                    PoolType.Props
                );
                
                // obj.localPosition = p;
                _objects.Add(obj.gameObject);
            }
        }
    }
    


    private Vector3 RandomPointOnFace(Bounds b, CubeFace face) {
        return face switch
        {
            CubeFace.Floor => new Vector3(
                Random.Range(b.min.x, b.max.x),
                b.max.y,
                Random.Range(b.min.z, b.max.z)
            ),

            CubeFace.LeftWall => new Vector3(
                b.min.x,
                Random.Range(b.min.y, b.max.y),
                Random.Range(b.min.z, b.max.z)
            ),

            CubeFace.RightWall => new Vector3(
                b.max.x,
                Random.Range(b.min.y, b.max.y),
                Random.Range(b.min.z, b.max.z)
            ),

            _ => b.center
        };
    }
    
    
    
}

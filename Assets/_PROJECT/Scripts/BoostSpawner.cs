using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;




public class BoostSpawner : MonoBehaviour {
    [SerializeField] private Transform _player;
    [SerializeField] private Transform _cruiser;
    
    [Header("Граница спауна")]
    [SerializeField] private float _xRightMax;
    [SerializeField] private float _xLeftMax;
    [SerializeField] private Boost _bustPrefab;
    
    
    public AnimationCurve[] _сurves;
    public List<Boost> _boosts;
    

    private void Start() {
         // Spawn(_player.position,_player.position, 3);
         SpawnRightWay(_player.transform.position, _cruiser.transform.position);
    }

    // У нас спавнятся бусты от игрока до крейсера на определенном расстоянии
    
    // Т.е у нас есть массив из этих бустов


    private void SpawnRightWay(Vector3 playerPosition, Vector3 cruiserPosition) {
        int countBusts = Random.Range(2, 6);
        float startZ = playerPosition.z;
        float endZ = cruiserPosition.z;
    
        // Создаем список позиций Z для равномерного распределения
        List<float> spawnPoints = new List<float>();
    
        // Вычисляем базовый шаг
        float segment = (endZ - startZ) / (countBusts + 1);
    
        for (int i = 1; i <= countBusts; i++) {
            float baseZ = startZ + (segment * i);
            // Добавляем случайность в пределах половины сегмента
            float minZ = baseZ - (segment * 0.3f);
            float maxZ = baseZ + (segment * 0.3f);
            float randomZ = Random.Range(minZ, maxZ);
        
            spawnPoints.Add(randomZ);
        }
    
        // Сортируем для гарантии порядка
        spawnPoints.Sort();
    
        // Спавним бусты
        foreach (float zPos in spawnPoints) {
            Vector3 spawnPosition = new Vector3(
                Random.Range(_xLeftMax, _xRightMax), 
                20f,                  
                zPos                  
            );
        
            Instantiate(_bustPrefab, spawnPosition, Quaternion.identity);
        }
    }


    private IReadOnlyList<Boost> Spawn(Vector3 playerPosition, Vector3 cruiserPosition, int count) {
        
        List<Boost> boost = new List<Boost>();
        for (int i = 0; i < count; i++) {
            Boost _newBoost = new Boost {
                randomTrajectory = _сurves[Random.Range(0, _сurves.Length-1)],
                height = Random.Range(0, _сurves.Length - 1)
            };
            boost.Add(_newBoost);
        }
        



        return boost;
    }
}

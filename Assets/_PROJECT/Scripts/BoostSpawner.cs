using System;
using System.Collections;
using System.Collections.Generic;
using SanyaBeerExtension;
using Unity.Collections;
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
    
    
    public AnimationCurve[] _curves;
    public List<Boost> _boosts;

    private PlayerMovement _playerMovement;
    
    
    private void Start() {
        _playerMovement = _player.GetComponent<PlayerMovement>();
         // Spawn(_player.position,_player.position, 3);
         SpawnRightWay(_player.transform.position, _cruiser.transform.position);
         SpawnRightWay(_player.transform.position, _cruiser.transform.position);
         SpawnEntranceBoost();
    }

    // У нас спавнятся бусты от игрока до крейсера на определенном расстоянии
    
    // Т.е у нас есть массив из этих бустов


    private void SpawnEntranceBoost() {
        _playerMovement.SetBooster(_curves[0], _boosts[0].transform.position);
    }
    

    private void SpawnRightWay(Vector3 playerPosition, Vector3 cruiserPosition) {
        int countBusts = Random.Range(6, 8);
        float startZ = playerPosition.z+50f; // щоб успел сообразить
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
                Random.Range(10,30),                  
                zPos                  
            );
        
            _boosts.Add(Instantiate(_bustPrefab, spawnPosition, Quaternion.identity)); 
        }

        for (int i = 0; i < _boosts.Count; i++) {
            if (i != _boosts.Count - 1) {
                _boosts[i].nextBooster = _boosts[i + 1].transform.position;
                _boosts[i].randomTrajectory = _curves[Random.Range(0, _curves.Length)];
                Debug.Log("Следующий буст в " + _boosts[i].nextBooster.z);
            }
            else {
                _boosts[i].nextBooster = cruiserPosition;
                _boosts[i].randomTrajectory = _curves[1];
                Debug.Log("Следующий буст в " + cruiserPosition.z);
            }
        }
    }


}

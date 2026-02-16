using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Architecture_M;
using Unity.Collections;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;


[Serializable]
public class InstancePets {
    public GameObject PetInstance;
    public PetItemConfig PetInfo;
}



public class PetsManager : MonoBehaviour {
    [SerializeField] private List<Transform> _petsPoints;
    [SerializeField] private int _maxPetCount;
    [Range(0,1), SerializeField] private float _chanceToSpawnBotPet;
    
    [Inject] private IGameSave<GameSavePC> _gameSave;
    [Inject] private List<PetItemConfig> _petsItems;
    
    
    private Dictionary<PetItemConfig, int> _petToCountDict = new();
    public List<InstancePets> PetsInstances { get; private set; } = new();
    public List<InstancePets> PetsInstancesForBots { get; private set; } = new();


    public event Action BuyPet;
    
    private void Start() {
        LoadDataToDict();
        UpdatePets();
    }

    public void SetRandomPets(List<Transform> points) {
        int maxCount = points.Count;

        // 1. Берём всех питомцев, которых бот может иметь
        List<PetItemConfig> availablePets = _petsItems; // либо другой источник
    
        // 2. Создаём случайную выборку
        List<PetItemConfig> randomSelection = new List<PetItemConfig>();
    
        for (int i = 0; i < maxCount; i++) {
            // Выбираем случайного питомца из доступных
            PetItemConfig pet = availablePets[Random.Range(0, availablePets.Count)];
            randomSelection.Add(pet);
        }

        // 3. Сортируем по модификатору, от сильного к слабому
        randomSelection = randomSelection
            .OrderByDescending(p => p.Modifier)
            .ToList();

        // 4. Спавним на точках
        StartCoroutine(SpawnBotsBuisnessBirds(points, randomSelection));
    }

    private IEnumerator SpawnBotsBuisnessBirds(List<Transform> points, List<PetItemConfig> randomSelection) {
        for (int i = 0; i < randomSelection.Count; i++) {
            PetItemConfig pet = randomSelection[i];
            GameObject instance = Instantiate(pet.Prefab, points[i].position, Quaternion.identity, points[i]);
            PetsInstancesForBots.Add(new InstancePets {
                PetInstance = instance,
                PetInfo = pet
            });
            if (Random.value >= _chanceToSpawnBotPet) break;
            yield return null;
        }
        PetsInstancesForBots.Clear();
    }


    public void AddPet(PetItemConfig petItem) {
        // Сохранить 
        int count = _gameSave.GetSave.AddNewPet(petItem.Id);
        var pet = GetPetItemById(petItem.Id);
        _petToCountDict[pet] = count;
        _gameSave.Save();
        if (CheckPetsNeedUpdate(pet)) {
            UpdatePets();
        }
    }
    
    private void LoadDataToDict() {
        List<PetsData> boughtPets = _gameSave.GetSave.Pets;
        // Нужно загружать лучшие
        foreach (var pet in boughtPets) {
            _petToCountDict[GetPetItemById(pet.Id)] = pet.Count;
        }
    }

    
    private bool CheckPetsNeedUpdate(PetItemConfig petItem) {
        if (PetsInstances.Count < _maxPetCount) {
            return true;
        }

        return PetsInstances.Any(pet => pet.PetInfo.Modifier < petItem.Modifier);
    }
    

    private PetItemConfig GetPetItemById(int id) =>
        _petsItems.First(pet => pet.Id == id);


    private void UpdatePets() {
        var topPets = GetBestPets(_petToCountDict);

        foreach (var pet in PetsInstances) {
            Destroy(pet.PetInstance);   
        }
        PetsInstances.Clear();

        for (var i = 0; i < topPets.Count; i++) {
            PetItemConfig pet = topPets[i];
            GameObject instance = Instantiate(pet.Prefab, _petsPoints[i].position,  Quaternion.identity, _petsPoints[i]);
            PetsInstances.Add(new InstancePets {
                PetInstance = instance,
                PetInfo = pet
            });
        }
        BuyPet?.Invoke();
        
    }

    private List<PetItemConfig> GetBestPets(Dictionary<PetItemConfig, int> petToCountDict) {
        IEnumerable<PetItemConfig> expanded = _petToCountDict
            .SelectMany(pair => 
                Enumerable.Repeat(pair.Key, pair.Value)
            );

        List<PetItemConfig> topPets = expanded
            .OrderByDescending(p => p.Modifier)
            .Take(_maxPetCount)
            .ToList();

        for (var i = 0; i < topPets.Count; i++) {
            Debug.Log($"Питомец {i+1} modifier = {topPets[i].Modifier}");
        }
        return topPets;
    }
}

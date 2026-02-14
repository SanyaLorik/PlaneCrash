using System;
using System.Collections.Generic;
using System.Linq;
using Architecture_M;
using UnityEngine;

[Serializable]
public class GameSavePC : GameSaveBase {
    public long Money = 0;
    public List<UpgradeData> Upgrades = new ();
    public List<PetsData> Pets = new ();
    public bool TutorialPassed = false;



    public int AddNewPet(int id) {
        bool exist = Pets.Any(pet => pet.Id == id);
        if (!exist) {
            Pets.Add(new PetsData() {
                Id = id,
                Count = 1,
            });
            return 1;
        }
        var pet = Pets.First(pet => pet.Id == id);
        pet.Count++;
        return pet.Count;
    }
    
}


[Serializable]
public class UpgradeData {
    public int Level = 1;
    public int Id = 0;
}


[Serializable]
public class PetsData {
    public int Id = 0;
    public int Count = 0;
}
using System;
using System.Collections.Generic;
using System.Linq;
using Architecture_M;
using UnityEngine;

[Serializable]
public class GameSavePC : GameSaveBase {
    public long Money = 0;
    public long RecordMoney = 0;
    public int RecordDistance = 0;
    public List<UpgradeData> Upgrades = new ();
    public List<PetsData> Pets = new ();
    public List<Skin> Skins = new ();
    public string SkinWearId = "";
    
    
    public bool TutorialPassed = false;
    public int CountBatutJumps = 0;
    public int CountBaskets = 0;



    public int AddNewPet(string id, int count) {
        var pet = Pets.FirstOrDefault(pet => pet.Id == id);
        if (pet == null) {
            Pets.Add(new PetsData() {
                Id = id,
                Count = count,
            });
            return count;
        }
        pet.Count+=count;
        return pet.Count;
    }
    
    
    public void AddNewSkin(string id) {
        if(Skins.Any(s => s.Id == id)) return;
        Skins.Add(new Skin {
            Id = id,
        });
    }
    
    
    public int SetNewUpgrade(int id, int level) {
        var upgrade = Upgrades.FirstOrDefault(u => u.Id == id);

        // Еще нет
        if (upgrade == null) {
            upgrade = new UpgradeData { Id = id, Level = level+1 };
            Upgrades.Add(upgrade);
        }
        else {
            upgrade.Level = level;
        }
        return upgrade.Level;
    }

    public int GetUpgradeLevel(int id) {
        bool exist = Upgrades.Any(upgrade => upgrade.Id == id);
        if (exist) {
            return Upgrades.First(upgrade => upgrade.Id == id).Level;
        }

        SetNewUpgrade(id, 0);
        return 1;
    }
    
}


[Serializable]
public class UpgradeData {
    public int Id = 0;
    public int Level = 1;
}


[Serializable]
public class PetsData {
    public string Id = "";
    public int Count = 0;
}


[Serializable]
public class Skin {
    public string Id = "";
}
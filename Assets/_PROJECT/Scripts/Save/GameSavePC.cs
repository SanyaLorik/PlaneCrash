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
    
    
    public void AddNewSkin(string id) {
        Skins.Add(new Skin {
            Id = id,
        });
    }

    public bool SkinIsBought(string id) 
        => Skins.Any(skin => skin.Id == id);
    
    
    public int AddNewUpgrade(int id) {
        bool exist = Upgrades.Any(upgrade => upgrade.Id == id);
        if (!exist) {
            Upgrades.Add(new UpgradeData() {
                Id = id,
                Level = 1,
            });
            Debug.Log(1);
            return 1;
        }
        var upgrade = Upgrades.First(upgrade => upgrade.Id == id);
        upgrade.Level++;
        Debug.Log(upgrade.Level);
        return upgrade.Level;
    }

    public int GetUpgradeLevel(int id) {
        bool exist = Upgrades.Any(upgrade => upgrade.Id == id);
        if (exist) {
            return Upgrades.First(upgrade => upgrade.Id == id).Level;
        }

        AddNewUpgrade(id);
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
    public int Id = 0;
    public int Count = 0;
}


[Serializable]
public class Skin {
    public string Id = "";
}
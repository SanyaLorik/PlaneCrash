using System;
using System.Linq;
using Architecture_M;
using UnityEngine;
using Zenject;

public class PlayerSkinInventory : IInitializable {
    public event Action<SkinItemConfig> SkinUnlocked;
    public event Action<SkinItemConfig> SkinEquipped;
    private readonly SkinItemConfig _defaultSkinConfig; 
    
    [Inject] private IGameSave<GameSavePC> _saver; 

    public PlayerSkinInventory(SkinItemConfig defaultSkinConfig) {
        _defaultSkinConfig = defaultSkinConfig;
    }
    
    public void Initialize() {
        // Если никого скина нет вообще
        if (!SkinIsBought(_defaultSkinConfig.Id)) {
            _saver.GetSave.AddNewSkin(_defaultSkinConfig.Id);
            EquipSkin(_defaultSkinConfig);
        }
    }
    
    public bool SkinIsBought(string id) 
        => _saver.GetSave.Skins.Any(s => s.Id == id);

    public void UnlockSkin(SkinItemConfig skinItemConfig) {
        _saver.GetSave.AddNewSkin(skinItemConfig.Id);
        _saver.Save();
        SkinUnlocked?.Invoke(skinItemConfig);
    }

    public void EquipSkin(SkinItemConfig skinItemConfig) {
        _saver.GetSave.SkinWearId = skinItemConfig.Id;
        _saver.Save();
        SkinEquipped?.Invoke(skinItemConfig);
    }

    public string CurrentSkinId => _saver.GetSave.SkinWearId;
    
}

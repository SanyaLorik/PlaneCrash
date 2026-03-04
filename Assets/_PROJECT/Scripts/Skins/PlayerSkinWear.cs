using System;
using System.Collections;
using System.Collections.Generic;
using Architecture_M;
using UnityEngine;
using Zenject;

public class PlayerSkinWear : MonoBehaviour { 
    [field: SerializeField] public SkinItemConfig DefaultSkinItemConfig;
    [SerializeField] private Transform _playerWearSkinParent;
    [SerializeField] private Animator _animator;
    [SerializeField] private SkinItemViewBase[] _skinItemViews;
    [SerializeField] private GameObject _currentSkin;
   
    

    [Inject] private IGameSave<GameSavePC> _saver; 
    [Inject] private IInputActivity _inputActivity; 
    public event Action NewSkinWear;


    private void Awake() {
        if (!_saver.GetSave.SkinIsBought(DefaultSkinItemConfig.Id)) {
            _saver.GetSave.AddNewSkin(DefaultSkinItemConfig.Id);
            _saver.GetSave.SkinWearId = DefaultSkinItemConfig.Id;
            _saver.Save();
           
            WearNewSkin(DefaultSkinItemConfig);
        }
    }


    private void Start() {
       // Нет еще скинов
       foreach (var skinItemView in _skinItemViews) {
           skinItemView.InitSkinData();
       }
    }
   
   
    private string _idWearedSkin;
   public void WearNewSkin(SkinItemConfig playerSkin) {
       if (_idWearedSkin == playerSkin.Id) {
            return;
       }
       _idWearedSkin = playerSkin.Id;
       
       if (_currentSkin != null) {
           Destroy(_currentSkin);
       }
       _currentSkin = _currentSkin = Instantiate(
           playerSkin.SkinPrefab,
           _playerWearSkinParent
        );

       StartCoroutine(ChangeSkinRoutine(playerSkin));
       NewSkinWear?.Invoke();
   }


   private IEnumerator ChangeSkinRoutine(SkinItemConfig skin) {
       _inputActivity.Disable();
       if (_currentSkin != null) {
           Destroy(_currentSkin);
           _animator.avatar = null;
       }
       yield return null; // дождаться конца кадра

       _currentSkin = Instantiate(skin.SkinPrefab, _playerWearSkinParent);

       _animator.avatar = skin.Avatar;
       _inputActivity.Enable();
   }
}

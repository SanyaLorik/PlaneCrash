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
    [SerializeField] private PlayerAnimator _playerAnimator;
   
    

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
    private Coroutine _changeSkinRoutine;
    public void WearNewSkin(SkinItemConfig playerSkin) {
        if (_idWearedSkin == playerSkin.Id) {
            return;
        }
        _idWearedSkin = playerSkin.Id;
    
        // Создаем временный скин для мгновенного отображения
        if (_currentSkin != null) {
            Destroy(_currentSkin);
        }
    
        
        var tempSkin = Instantiate(playerSkin.SkinPrefab, _playerWearSkinParent);
        _currentSkin = tempSkin;

        var tempController = tempSkin.GetComponent<SkinElementsController>();
        _playerAnimator.SetSkinElementsController(tempController);
    
        // Запускаем корутину для финальной замены с аватаром
        if (_changeSkinRoutine != null)
        {
            StopCoroutine(_changeSkinRoutine);
        }

        _changeSkinRoutine = StartCoroutine(ChangeSkinRoutine(playerSkin));
    }

    private IEnumerator ChangeSkinRoutine(SkinItemConfig skin) {
        _inputActivity.Disable();
    
        // Не уничтожаем tempSkin здесь, так как это и есть текущий скин
        _animator.avatar = null;
    
        yield return null;
    
        // Обновляем аватар
        _animator.avatar = skin.Avatar;
        _inputActivity.Enable();
        NewSkinWear?.Invoke();
    }
}

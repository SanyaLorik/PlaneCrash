using System;
using Architecture_M;
using UnityEngine;
using Zenject;

public class PlayerSkinWear : MonoBehaviour { 
    [field: SerializeField] public SkinItemConfig DefaultSkinItemConfig;
    [SerializeField] private Transform _playerWearSkinParent;
    [SerializeField] private Animator _animator;
    [SerializeField] private SkinItemView[] _skinItemViews;
    [SerializeField] private GameObject _currentSkin;
   
    

    [Inject] private IGameSave<GameSavePC> _saver; 
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

       _animator.avatar = playerSkin.Avatar;
    
       NewSkinWear?.Invoke();
   }
}

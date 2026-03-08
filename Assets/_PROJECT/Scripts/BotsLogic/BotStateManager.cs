using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public enum BotState {
    Wandering,
    Flight
}


public class BotStateManager : MonoBehaviour {
    [SerializeField] private List<Transform> _petsPoints;
    [SerializeField] private Transform _skinParent;
    [SerializeField] private BotAnimator _botAnimator;
    
    private BotFlight _botFlight;
    private BotWander _botWander;
    private BotMonolog _botMonolog;
    
    private IBotBehaviour _currentBotBehaviour;

    public BotState State { get; private set; }
    public Rigidbody Rb { get; private set; }

    
    [Inject] private PetsManager _petsManager;
    
    private void Awake() {
        _botFlight = GetComponent<BotFlight>();
        _botWander = GetComponent<BotWander>();
        _botMonolog = GetComponent<BotMonolog>();

        _botFlight.EndFlightAfterPlayerFall += BotFlightOnEndFlight;
        _currentBotBehaviour = _botWander;
        State = BotState.Wandering;
        Rb = GetComponent<Rigidbody>();
    }


    private void Start() {
        _petsManager.BotSetRandomPets(_petsPoints);
    }

    public void ChangeBotState(BotState newState) {
        _currentBotBehaviour?.Exit();
        
        State = newState;
        _currentBotBehaviour = State switch {
            BotState.Flight => _botFlight,
            BotState.Wandering => _botWander,
            _ => _currentBotBehaviour
        };

        // Debug.Log(_currentBotBehaviour);
        _currentBotBehaviour?.Enter();
    }
    
    
    public void ChangeNickname() {
        _botMonolog.ChangeNickname();
    }
    
    public void PlayerInSpawn() {
        _botFlight.GoToFall();
    }

    private GameObject _skinInstance;

    public void InitAnimator() {
        _botAnimator.InitAnimator(_botFlight, _botWander);
    }
    public void SetBotSkin(SkinItemConfig skinItemConfig) {
        StartCoroutine(ChangeSkinRoutine(skinItemConfig));
    }
    
    private IEnumerator ChangeSkinRoutine(SkinItemConfig skin) {
        Debug.Log("Смена скина у бота");
        if (_skinInstance != null) {
            Destroy(_skinInstance);
            _botAnimator.SetModelData(null, null);
        }
        yield return null; // дождаться конца кадра

        _skinInstance = Instantiate(skin.SkinPrefab, _skinParent);
        var skinItem = _skinInstance.GetComponent<SkinElementsController>();
        _botAnimator.SetModelData(skin.Avatar, skinItem);
    }
    

    public void SetBotSpeak() {
        _botMonolog.SaySomething();
    }

    public void SetBotStfu() {
        _botMonolog.Stfu();
    }

    
    private void BotFlightOnEndFlight() {
        ChangeBotState(BotState.Wandering);
    }


    private void OnDisable() {
        _botFlight.EndFlightAfterPlayerFall -= BotFlightOnEndFlight;
    }
}

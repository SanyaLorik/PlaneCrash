using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using _PROJECT.Scripts.Helpers;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class PlayerPrediction : MonoBehaviour {
    // Прокачивается Z координата до бустов
    private PlayerStateManager _playerStateManager;
    private CancellationTokenSource _tokenSource;
    private CancellationToken _token;
    
    
    private List<float> _boostsZ;
    private List<Boost> _trueBoosts;
    private BoostSpawner _boostSpawner;
    private IPlayerStatsReadOnly _playerStats;
    
    
    [Inject]
    private void Init(PlayerStateManager playerStateManager, LevelBounds levelBounds, BoostSpawner boostSpawner, IPlayerStatsReadOnly playerStats) {
        _playerStateManager = playerStateManager;
        _playerStateManager.ChangeState += PlayerStateManagerOnChangeState;
        _boostSpawner =  boostSpawner;
        _playerStats = playerStats;
    }

    private void PlayerStateManagerOnChangeState(PlayerState state) {
        if (state == PlayerState.Flight) {
            _token = UniTaskHelper.CreateNewToken(ref _tokenSource);
            GetZBoostCoord();
            PlayerPredictAsync(_token).Forget();
        }
        else {
            UniTaskHelper.StopTask(ref _tokenSource);
        }
    }
    
    private void GetZBoostCoord() {
        _trueBoosts = _boostSpawner.GetRightBoosts();
        _boostsZ = _trueBoosts
            .Select(boost => boost.transform.position.z)
            .ToList();
        
        Debug.Log("Z boosts is " + _boostsZ);
        foreach (var boost in _boostsZ) {
            Debug.Log(boost);
        }
    }



    [SerializeField] private float _predictDistance;
    private async UniTask PlayerPredictAsync(CancellationToken token) {
        int index = 0;
        while(!token.IsCancellationRequested && index < _boostsZ.Count) {
            if (transform.position.z + _playerStats.PredictDistance >= _boostsZ[index] ) {
                _trueBoosts[index]
                    .SetBoostPersonalityVisibleAndRevealTheHiddenInnerEnergeticMetaphysicalGameplayEssenceOfThisSpecificAccelerationEntityWhileSynchronizingItsVisualAuraWithPlayerPerceptionSystemsTheLivingBreathingDigitalUniverse();
                index++;
            }

            await UniTask.WaitForFixedUpdate(token);
        }
    }

    
    
    
    private void OnDestroy() {
        UniTaskHelper.DisposeTask(ref _tokenSource);
    }
}

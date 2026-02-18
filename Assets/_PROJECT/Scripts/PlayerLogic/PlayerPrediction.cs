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
    
    
    private List<float> _boostsZ;
    private List<Boost> _trueBoosts;
    private BoostSpawner _boostSpawner;
    private IPlayerStatsReadOnly _playerStats;
    [Inject] private UpgradesCalculator _upgradesCalculator;
    
    
    [Inject]
    private void Init(PlayerStateManager playerStateManager, LevelBounds levelBounds, BoostSpawner boostSpawner, IPlayerStatsReadOnly playerStats) {
        _playerStateManager = playerStateManager;
        _playerStateManager.ChangeState += PlayerStateManagerOnChangeState;
        _boostSpawner =  boostSpawner;
        _playerStats = playerStats;
    }

    private void PlayerStateManagerOnChangeState(PlayerState state) {
        if (state == PlayerState.Flight) {
            _tokenSource = new CancellationTokenSource();
            GetZBoostCoord();
            PlayerPredictAsync(_tokenSource.Token).Forget();
        }
        else {
            _tokenSource?.Cancel();
        }
    }
    
    private void GetZBoostCoord() {
        _trueBoosts = _boostSpawner.GetRightBoosts();
        _boostsZ = _trueBoosts
            .Select(boost => boost.transform.position.z)
            .ToList();
    }



    private async UniTask PlayerPredictAsync(CancellationToken token) {
        int index = 0;
        float predictDistance = _upgradesCalculator.GetPredictDistanceByLevel();
        while(!token.IsCancellationRequested && index < _boostsZ.Count) {
            if (transform.position.z + predictDistance >= _boostsZ[index] ) {
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

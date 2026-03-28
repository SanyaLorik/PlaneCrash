using System;
using DG.Tweening;
using SanyaBeerExtension;
using TMPro;
using UnityEngine;
using Zenject;

public class BetVisual : MonoBehaviour {
    // [Header("Animation")]
    // [SerializeField] private Ease _hideEase = Ease.InBack;
    // [SerializeField] private Ease _showEase = Ease.OutBack;
    // [SerializeField] private float _showDuration = 1f;
    // [SerializeField] private RectTransform _outScreenPointer;
    //
    //
    // private float _inScreenPoseY;
    // private float _outScreenPoseY;
    //
    //
    // [SerializeField] private TMP_Text _playerBankText;
    // [SerializeField] private TMP_Text _playerBetText;
    // [SerializeField] private TMP_Text _rewardText;
    // [SerializeField] private RectTransform _betAndRewardContainer;
    //
    // [Header("Animation")]
    // [SerializeField] private TMP_Text _betNamingText;
    // [SerializeField] private TMP_Text _rewardNamingText;
    //
    //
    //
    // [Inject] private PlayerStateManager _playerStateManager;
    // [Inject] private ZoneManager _zoneManager;
    // [Inject] private IPlayerStatsReadOnly _playerStats;
    // [Inject] private PetsManager _petsManager; 
    // [Inject] private PlayerBank _bank;
    // [Inject] private UpgradesCalculator _upgradesCalculator;
    // [Inject] private NumberFormatter _formatter; 
    // [Inject] private LocalizationDataPC _localization;
    //
    //
    // private void OnEnable() {
    //     _bank.BankChanged += OnChangeBank;
    //     _zoneManager.ChooseBet += ShowBet;
    //     _zoneManager.ChooseMultiplier += ShowMultiplier;
    //     _playerStateManager.ChangeState += OnChangeState;
    // }
    //
    //     
    // private void OnDisable() {
    //     _bank.BankChanged -= OnChangeBank;
    //     _zoneManager.ChooseBet -= ShowBet;
    //     _zoneManager.ChooseMultiplier -= ShowMultiplier;
    //     _playerStateManager.ChangeState -= OnChangeState;
    // }
    //
    // private void Start() {
    //     _betAndRewardContainer.ActiveSelf();
    //     _inScreenPoseY = _betAndRewardContainer.anchoredPosition.y;
    //     _outScreenPoseY = RectTransformHelper.GetYUnderScreen(_betAndRewardContainer, _outScreenPointer);
    //     _playerBankText.text = _formatter.ValuteFormatter(_bank.PlayerCapital);
    //     _betAndRewardContainer.anchoredPosition = new Vector2(_betAndRewardContainer.anchoredPosition.x, _outScreenPoseY);
    //     // Перевод текста
    //     _betNamingText.text = _localization.Bet;
    //     _rewardNamingText.text = _localization.Reward;
    //     
    //     _betAndRewardContainer.DisactiveSelf();
    // }
    //
    //
    // private void OnChangeState(PlayerState state) {
    //     // ANIMATE 
    //     HideBetRewardCanvasAnimation();
    //     if (state == PlayerState.Cruisered || state == PlayerState.Grounded) {
    //         _multiplier = 1f;
    //     }
    // }
    //
    // private void HideBetRewardCanvasAnimation() {
    //     Debug.Log("HideBetRewardCanvasAnimation");
    //     _betAndRewardContainer.DOKill();
    //     _betAndRewardContainer.
    //         DOAnchorPosY(_outScreenPoseY, _showDuration)
    //         .SetEase(_hideEase)
    //         .OnComplete(_betAndRewardContainer.DisactiveSelf);
    // }
    //
    // private void ShowBetRewardCanvasAnimation() {
    //     Debug.Log("ShowBetRewardCanvasAnimation");
    //     
    //     _betAndRewardContainer.ActiveSelf();
    //     _betAndRewardContainer.DOKill();
    //     _betAndRewardContainer.DOAnchorPosY(_inScreenPoseY, _showDuration)
    //         .SetEase(_showEase);
    // }
    //
    //
    // private void OnChangeBank(long capital) {
    //     _playerBankText.text = _formatter.ValuteFormatter(capital);
    //     
    // }
    //
    // private void ShowBet(float bet) {
    //     _multiplier = 1f;
    //     _playerBetText.text = _formatter.ValuteFormatter(bet);
    //     float playerRewardWithoutMultiplier = bet * _upgradesCalculator.GetUpgradeMultiplierByLevel();
    //     
    //     long win;
    //     try {
    //         checked {
    //             win = (long)playerRewardWithoutMultiplier;
    //         }
    //     }
    //     catch (OverflowException) {
    //         win = long.MaxValue;
    //     }
    //     
    //     
    //     _rewardText.text = _formatter.ValuteFormatter(win);
    //     
    //     if (bet == 0 && _betAndRewardContainer.gameObject.activeSelf) {
    //         HideBetRewardCanvasAnimation();
    //     }
    //     else if (bet > 0 && !_betAndRewardContainer.gameObject.activeSelf) {
    //         ShowBetRewardCanvasAnimation();
    //     }
    // }
    //
    // private float _multiplier = 1f;
    //
    // private void ShowMultiplier(float multiplier) {
    //     _multiplier = multiplier;
    //     float playerReward = _zoneManager.BetAmount * _multiplier * _upgradesCalculator.GetUpgradeMultiplierByLevel();
    //     long win;
    //     try {
    //         checked {
    //             win = (long)playerReward;
    //         }
    //     }
    //     catch (OverflowException) {
    //         win = long.MaxValue;
    //     }
    //     
    //     _rewardText.text = _formatter.ValuteFormatter(win);
    // }

    

    
}

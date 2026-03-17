using System;
using Architecture_M;
using Zenject;

[Serializable]
public abstract class PurshaseItem
{
    [Inject] protected PlayerBank _bank;
    [Inject] protected IGameSave<GameSavePC> _saver;
    [Inject] private IInterstitialActivity _interstitialActivity;
    
    protected void SavePurchasedStatus() 
    {
        _interstitialActivity.DisableInterstitial();
        _saver.GetSave.Purchased = true;
    }
    public abstract void Receive(); 
}

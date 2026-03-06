using Architecture_M;
using SanyaBeerExtension;
using UnityEngine;
using Zenject;

public class PurchaseSlotPC : PurchaseSlot
{
    [SerializeReference] [SubclassSelector] private PurshaseItem[] _purshaseItems;

    [Inject] private DiContainer _diContainer;

    private void Start()
    {
        BindPurshaseItem();
    }

    public override void Buy()
    {
        _purshaseItems.ForEach(i => i.Receive());
    }

    private void BindPurshaseItem()
    {
        _purshaseItems.ForEach(i => _diContainer.Inject(i));
    }
}

using SanyaBeerExtension;
using UnityEngine;

public class TEST_PURCHASE : MonoBehaviour
{
    public PurchaseSlotPC[] PurchaseSlotPCs;

    private void Start()
    {
        PurchaseSlotPCs.ForEach(i => i.Buy());
    }
}
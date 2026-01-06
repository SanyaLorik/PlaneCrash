using System;
using TMPro;
using UnityEngine;

public class BetVisual : MonoBehaviour {
    public TMP_Text CapitalVisual;
    public TMP_Text PlayerBetVisual;
    public TMP_Text XMultiplyVisual;
    public TMP_Text RewardVisual;
    
    
    public static BetVisual Instantiate { get; private set; }

    private void Awake() {
        if (Instantiate != null) {
            Debug.Log("There is already an instance of BetVisual at " + gameObject.name);
            return;
        }
        Instantiate = this;
    }
    
    
    
    
}

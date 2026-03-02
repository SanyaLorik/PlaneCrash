using System;
using UnityEngine.UI;

public static class ButtonExtension {
    public static Action Click;
    
    public static void AddListenerWithSound(this Button button, Action action) {
        button.onClick.AddListener(() => {
            Click?.Invoke();
            action?.Invoke();
        });
    }
}

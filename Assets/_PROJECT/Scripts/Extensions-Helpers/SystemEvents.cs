using System;

public static class SystemEvents {
    public static event Action<bool> OpenCanvasWindow;
    
    public static void InvokeCanvasWindow(bool state) {
        OpenCanvasWindow?.Invoke(state);
    }
}
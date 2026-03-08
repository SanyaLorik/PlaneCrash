using System;

public static class SystemEvents {
    public static event Action<bool> WindowOpened;
    public static event Action<bool> ForbidZoomChanged;
    
    public static void WindowOpen(bool state) {
        WindowOpened?.Invoke(state);
    }
    
    public static void ForbidZoomChange(bool state) {
        ForbidZoomChanged?.Invoke(state);
    }
}
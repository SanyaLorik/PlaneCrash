using UnityEngine;


public enum MagneticType {
    Money,
    Boost
}

public interface IMagnetic {
    bool CanBeMagnetic { get; set; }
    Vector3 Position { get; }
    MagneticType Type { get; }
    void Attract(Vector3 target, float speed);
}

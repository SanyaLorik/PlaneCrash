using UnityEngine;

public interface IOrbitalRotationInput
{
    Vector2 OrbitalDirection { get; }
}

public interface IActivityButtonPC
{
    void ShowJumpButton();

    void HideJumpButton();

    void ShowOrbitalJoystick();

    void HidOrbitalJoystick();
}

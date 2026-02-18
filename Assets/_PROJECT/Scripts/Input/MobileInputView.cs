using Architecture_M;
using SanyaBeerExtension;
using UnityEngine;

public class MobileInputView : MobileInputViewBase
{
    [SerializeField] private Joystick _orbitalMovement;

    public Vector2 OrbitalDirection => _orbitalMovement.Direction;

    public override void Disable()
    {
        base.Disable();

        _orbitalMovement.DisactiveSelf();
    }

    public override void Enable()
    {
        base.Enable();

        _orbitalMovement.ActiveSelf();
    }
}
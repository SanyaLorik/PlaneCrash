using Architecture_M;
using UnityEngine;

public class MobileInput : MobileInputBase<MobileInputView>, IOrbitalRotationInput
{
    public MobileInput(MobileInputView inputView) : base(inputView)
    {

    }

    public Vector2 OrbitalDirection => inputView.OrbitalDirection;

    public override void Enable()
    {
        inputView.Enable();
    }

    public override void Disable()
    {
        inputView.Disable();
    }

}
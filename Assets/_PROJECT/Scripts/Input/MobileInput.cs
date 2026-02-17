using Architecture_M;

public class MobileInput : MobileInputBase
{
    public MobileInput(MobileInputView inputView) : base(inputView)
    {

    }

    public override void Enable()
    {
        inputView.Enable();
    }

    public override void Disable()
    {
        inputView.Disable();
    }
}

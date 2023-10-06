public class DigimochiStateFeed : State
{

    protected override void OnEnter()
    {
        base.OnEnter();

        digimochi.SetAnimation(Digimochi.DigimochiAnimations.Feed);
    }

}

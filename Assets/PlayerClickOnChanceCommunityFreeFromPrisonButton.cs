public class PlayerClickOnChanceCommunityFreeFromPrisonButton<T>:IClickableButtonHandler where T:SpecialCard
{
    private MonopolyPlayer _monopolyPlayer;
    public PlayerClickOnChanceCommunityFreeFromPrisonButton(MonopolyPlayer monopolyPlayer)
    {
        _monopolyPlayer = monopolyPlayer;
    }
    public void OnClick()
    {
        _monopolyPlayer.ClickOnChanceCommunityFreeFromPrisonButton<T>();
    }
}
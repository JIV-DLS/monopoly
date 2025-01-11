using Monopoly;

public abstract class CommunityCard : SpecialCard
{
    public CommunityCard(MonopolyGameManager monopolyGameManager, string description)
        : base(monopolyGameManager, "Community", description)
    {
    }

}
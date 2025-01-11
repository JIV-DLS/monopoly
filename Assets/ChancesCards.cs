using System.Collections.Generic;
using Monopoly;

public class ChancesCards : ShuffableCollection<ChanceCard>
{
    
    public ChancesCards(MonopolyGameManager monopolyGameManager)
    {
        
        AddRange(new List<ChanceCard>{
            new AdvanceToUtilityCard(monopolyGameManager),
            new BankDividendCard(monopolyGameManager),
            new SpeedingFineCard(monopolyGameManager),
            new RepairCostCard(monopolyGameManager),
            new AdvanceToStartCard(monopolyGameManager),
            new AdvanceToStartCard(monopolyGameManager),
            new GetOutOfJailCard(monopolyGameManager),
            new GoToJailCard(monopolyGameManager),
            new AdvanceToAvenueHenriMartinCard(monopolyGameManager),
            new RealEstateLoanCard(monopolyGameManager),
            new MoveBackThreeSpacesCard(monopolyGameManager),
            new AdvanceToBoulevardDeLaVilletteCard(monopolyGameManager),
            new ElectedChairmanCard(monopolyGameManager),
            new AdvanceToRueDeLaPaixCard(monopolyGameManager),
            new AdvanceToGareMontparnasseCard(monopolyGameManager),
            new AdvanceToStationCardChance(monopolyGameManager),
        });
        Shuffle();
    }

}
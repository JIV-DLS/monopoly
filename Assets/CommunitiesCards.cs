using System.Collections.Generic;
using Monopoly;

public class CommunitiesCards : ShuffableCollection<CommunityCard>
{
    
    

    public CommunitiesCards(MonopolyGameManager monopolyGameManager)
    {
        
        AddRange(new List<CommunityCard>{
            new PlaygroundDonationCard(monopolyGameManager),
            new NeighborhoodPartyCard(monopolyGameManager),
            new BakeSaleCard(monopolyGameManager),
            new BakeSalePurchaseCard(monopolyGameManager),
            new GardenCleanupCard(monopolyGameManager),
            new HospitalPlayCard(monopolyGameManager),
            new PedestrianPathCleanupCard(monopolyGameManager),
            new ChattingWithElderNeighborCard(monopolyGameManager),
            new AnimalShelterDonationCard(monopolyGameManager),
            new BloodDonationCard(monopolyGameManager),
            new MarathonForHospitalCard(monopolyGameManager),
            new HospitalPlayCard(monopolyGameManager),
            new MarathonForHospitalCard(monopolyGameManager),
            new AdoptPuppyCard(monopolyGameManager),
            new LoudMusicCard(monopolyGameManager),
            new HelpNeighborCard(monopolyGameManager),
            new CharityCarWashCard(monopolyGameManager),
            new HousingImprovementCard(monopolyGameManager),
            // Ajoutez ici d'autres cartes Community...
        });
        Shuffle();
        
    }
}
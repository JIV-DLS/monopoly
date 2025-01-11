using System.Collections;
using System.Collections.Generic;

namespace Monopoly
{
    public interface IMonopolyPlayer
    {
        string ToString();
        MonopolyPlayerDeck deck { get; }
        List<GetOutOfJailCard> getOutOfJailChanceCards { get; }
        List<AdoptPuppyCard> adoptPuppyCards { get; }
        string name { get; }
        PlayerElementOnMap playerElementOnMap { get; }
        int money { get; }
        bool hasPerformedAction { get; }
        BoardTile currentTile { get; }
        int prison { get; }
        void IncrementMoneyWith(int amount);
        void DecrementMoneyWith(int amount);
        void MoveTo(BoardTile tileToLandOn);
        IEnumerator TriggerPlay(float rollDiceTimeout, float userBuyTileTimeout);
        IEnumerator HandlePlayerBuyAction(float actionTimeout);
        IEnumerator HandlePlayerRollDice(float actionTimeout);
        void AskPlayFromButton();
        IEnumerator Play();
        void DisableAllActionButtons();
        void EnableRollDiceAction();
        bool CanBeChargedOf(int dueAmount);
        int ChargedOf(int dueAmount);
        void HaveWon(int won);
        IEnumerator GatherMoneyToReach(int chargedOf);
        bool CanContinuePlaying();
        bool HavePurchasedTiles();
        int GetAllHousesNumber();
        int GetAllHotelsNumber();
        void GoInPrison();
        void DisableAllFreeFromPrisonButtons();
        bool IsInPrison();
        void FreeFromPrison();
        bool CanPlay();
        void HaveWonAGetOutOfJailChanceCard(GetOutOfJailCard getOutOfJailCard);
        void HaveWonAnAdoptAPuppyCommunityCard(AdoptPuppyCard adoptPuppyCard);
        void ClickOnChanceCommunityFreeFromPrisonButton<T>() where T : SpecialCard;
        void HandleCardClick<TCard>(List<TCard> cardList, FreeFromPrisonButton fromPrisonButton) where TCard : SpecialCard;
        void Buy(PurchasableTile purchasableTile);
        void BuildOnPropertyTile(PropertyTile propertyTile);
    }
}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Realtime;

namespace Monopoly
{
    public class MonopolyPlayer : IMonopolyPlayer
    {
        public override string ToString()
        {
            return name;
        }

        public MonopolyPlayer(Player player, PlayerSummaryButton playerSummaryButton,
            PlayerElementOnMap playerElementOnMap, 
            ThrowDices throwDices,
            FreeFromPrisonButton communityFreeFromPrisonButton,
            FreeFromPrisonButton chanceFreeFromPrisonButton,
            MonopolyGameManager monopolyGameManager)
        {
            deck = new MonopolyPlayerDeck();
            name = player.NickName;
            this.player = player;
            _playerSummaryButton = playerSummaryButton;
            _playerSummaryButton.setPlayer(this);
            this.playerElementOnMap = playerElementOnMap;
            playerElementOnMap.SetMonopolyPlayer(this);
            _throwDices = throwDices;
            _communityFreeFromPrisonButton = communityFreeFromPrisonButton;
            _communityFreeFromPrisonButton.Init();
            _communityFreeFromPrisonButton.Handler = new PlayerClickOnChanceCommunityFreeFromPrisonButton<CommunityCard>(this);
            _chanceFreeFromPrisonButton = chanceFreeFromPrisonButton;
            _chanceFreeFromPrisonButton.Init();
            _chanceFreeFromPrisonButton.Handler = new PlayerClickOnChanceCommunityFreeFromPrisonButton<ChanceCard>(this);
            _throwDices.SetSelfMadePlayer(this);
            _monopolyGameManager = monopolyGameManager;
            getOutOfJailChanceCards = new List<GetOutOfJailCard>();
            adoptPuppyCards = new List<AdoptPuppyCard>();
        }

        public Player player { get; set; }

        public MonopolyPlayerDeck deck{get; }
        private readonly ThrowDices _throwDices;
        private readonly FreeFromPrisonButton _communityFreeFromPrisonButton;
        private readonly FreeFromPrisonButton _chanceFreeFromPrisonButton;
        public List<GetOutOfJailCard> getOutOfJailChanceCards{get; }
        public List<AdoptPuppyCard> adoptPuppyCards{get; }
        public string name{get; }
        private readonly PlayerSummaryButton _playerSummaryButton;
        public PlayerElementOnMap playerElementOnMap{get; }
        public int money { get; private set; } = 0;
        private readonly MonopolyGameManager _monopolyGameManager;
        private bool _askedPlayFromButton = false;
        public bool hasPerformedAction { get; private set; } = false;

        private float _timer = 0f;
        private int _lastRolledResult;

        public BoardTile currentTile { get; private set; }
        // Start is called once before the first execution of Update after the MonoBehaviour is created


        public void IncrementMoneyWith(int amount)
        {
            money += amount;
            _playerSummaryButton.Refresh();
        }
        public void DecrementMoneyWith(int amount)
        {
            money -= amount;
            _playerSummaryButton.Refresh();
        }
        public void MoveTo(BoardTile tileToLandOn)
        {
            currentTile = tileToLandOn;
            // Debug.Log($"assigning tile {tile.tileGameObject}");
            playerElementOnMap.UpdateTransform(new Vector3(currentTile.getTransform().position.x,
                playerElementOnMap.transform.position.y, currentTile.getTransform().position.z));

        }

        public IEnumerator TriggerPlay(float rollDiceTimeout, float userBuyTileTimeout)
        {

            if (player.IsLocal)
            {
                if (CanPlay())
                {
                    yield return HandlePlayerRollDice(rollDiceTimeout);
                    yield return HandlePlayerBuyAction(userBuyTileTimeout);
                }else if (IsInPrison())
                {
                    prison--;
                    if (prison > 0)
                    {
                        _monopolyGameManager.SetGameTextEventsText($"{name} est en prison. Encore {prison} tours restant.");
                    }
                    else
                    {
                        _monopolyGameManager.SetGameTextEventsText($"{name} est libéré de prison.");
                        DisableAllFreeFromPrisonButtons();
                    }
                    yield return new WaitForSeconds(.5f);
                }
            }
            else
            {
                
                yield return _monopolyGameManager.AskAPlayerToPlay(player);
            }
            
        
        }
        public IEnumerator PerformRemoteAction()
        {
            if (true)
            {
                Debug.LogError("Cannot call remote actions on your own object!");
                yield break;
            }

            Debug.Log("Requesting remote player to perform action...");

            // Send an RPC to the owner to perform the action
            // playerElementOnMap.photonView.RPC("StartRemoteAction", playerElementOnMap.photonView.Owner);

            // Wait for the action to complete
            while (!playerElementOnMap.actionCompleted)
            {
                yield return null; // Wait for the next frame
            }

            Debug.Log("Remote action completed!");
            playerElementOnMap.SetActionCompleted(false); // Reset for the next request
        }
        public IEnumerator HandlePlayerBuyAction(float actionTimeout)
        {
            _timer = 0f;
            if (currentTile is PurchasableTile purchasableTile)
            {
                /*foreach (PurchasableTile p in _monopolyGameManager.GetAllGroupOfThisPropertyTile(purchasableTile.GetTargetType()))
            {
                if (p.CanBeBought())
                {
                    _monopolyGameManager.BuyPurchasableTileBy(this, p);
                }
            }*/

                if (currentTile.CanBeBought() && currentTile.getPrice()<=money)
                {
                    _monopolyGameManager.gameCardBuy.ShowPurchasableCard(purchasableTile, this);
                    // Wait for the player to perform an action or timeout
                    while (_timer < actionTimeout && _monopolyGameManager.gameCardBuy.gameObject.activeSelf)
                    {

                        _timer += Time.deltaTime;
                        _monopolyGameManager.SetGameTextEventsText($"{name}, Veuillez decidez {actionTimeout-_timer:0.00} seconde(s)");
                        yield return null; // Wait until the next frame
                    }

                    if (purchasableTile.IsOwnedBy(this))
                    {
                        _monopolyGameManager.SetGameTextEventsText($"{name} a acquéris {currentTile.TileName}");
                    }
                    else
                    {
                        _monopolyGameManager.SetGameTextEventsText($"{name} a décliné l'offre {currentTile.TileName}");
                    }
                    _monopolyGameManager.gameCardBuy.Hide();
                    yield return new WaitForSeconds(1.5f);
                }else if (purchasableTile.IsOwnedBy(this))
                {
                    if(purchasableTile is PropertyTile propertyTile){
                        /*while (propertyTile.CanBeUpgraded())
                    {
                        BuildOnPropertyTile(propertyTile);
                    }*/

                        if (propertyTile.CanBeUpgraded() && CanBeChargedOf(propertyTile.GetUpgradePrice()))
                        {
                            PropertyTileState oldPropertyState = propertyTile.propertyTileState;
                            int oldCost = propertyTile.GetLevelCost();
                            _monopolyGameManager.gameCardBuild.ShowPurchasableCard(purchasableTile, this);
                            while (_timer < actionTimeout && _monopolyGameManager.gameCardBuild.gameObject.activeSelf)
                            {

                                _timer += Time.deltaTime;
                                _monopolyGameManager.SetGameTextEventsText(
                                    $"{name}, Veuillez decider {actionTimeout - _timer:0.00} seconde(s)");
                                yield return null; // Wait until the next frame
                            }

                            if (oldPropertyState.CompareTo(propertyTile.propertyTileState) < 0)
                            {
                                _monopolyGameManager.SetGameTextEventsText(
                                    $"{name} a construit un {propertyTile.propertyTileState.GetName()} sur {currentTile.TileName}. Le prix de passage passe de {oldCost}M à {propertyTile.GetLevelCost()}M");
                            }
                            else
                            {
                                _monopolyGameManager.SetGameTextEventsText(
                                    $"{name} n'a pas fait des travaux sur {currentTile.TileName}");
                            }

                            _monopolyGameManager.gameCardBuild.Hide();
                            yield return new WaitForSeconds(1.5f);
                        }
                        else
                        {
                            if (!propertyTile.CheckIfOwnerDeckHasThisGroup())
                            {
                                _monopolyGameManager.SetGameTextEventsText(
                                    $"{name} ne peut pas faire de travaux sur {currentTile.TileName}. {propertyTile.GetLevelText()}");
                            }
                            else
                            {
                                _monopolyGameManager.SetGameTextEventsText(
                                    $"{name} ne peut plus faire de travaux sur {currentTile.TileName}.");
                            }
                            yield return new WaitForSeconds(3f);
                        }
                    }

                    _monopolyGameManager.SetGameTextEventsText(
                        $"{name}, chaque joueur passant sur {currentTile.TileName} devra vous payer {purchasableTile.GetCostText()}, {purchasableTile.GetLevelText()}.");
                    yield return new WaitForSeconds(4f);
                }
                else
                {
                    if (purchasableTile.IsOwned())
                    {
                        string priceText = currentTile is PublicServiceTile ? (purchasableTile.GetLevelCost() * _lastRolledResult).ToString(): purchasableTile.GetLevelCost().ToString();
                        _monopolyGameManager.SetGameTextEventsText(
                            $"{name} doit payer {priceText}M à {purchasableTile.GetOwner().name}, ({purchasableTile.GetOwner().name} {purchasableTile.GetLevelText()}).");
                    }
                    else if(purchasableTile.IsMortgaged)
                    {
                        _monopolyGameManager.SetGameTextEventsText(
                            $"{purchasableTile.TileName} appartient à {purchasableTile.GetOwner().name}. Mais il a été hypothéqué.");
                    }
                    else
                    {
                        _monopolyGameManager.SetGameTextEventsText(
                            $"{purchasableTile.TileName} n'appartient à aucun joueur.");
                    }
                    yield return new WaitForSeconds(4f);
                }
            }
            else
            {
                _monopolyGameManager.SetGameTextEventsText($"{name} ne peut effectuer aucune action.");
                yield return new WaitForSeconds(4f);
            }

        }

        public IEnumerator HandlePlayerRollDice(float actionTimeout)
        {
            hasPerformedAction = false;
            _timer = 0f;

            EnableRollDiceAction();
            float messageTimer = 0f; // Timer for the message

            // Wait for the player to perform an action or timeout
            while (_timer < actionTimeout)
            {
                if (hasPerformedAction || _askedPlayFromButton)
                {
                    break;
                }

                // Increment timers
                _timer += Time.deltaTime;
                _monopolyGameManager.SetGameTextEventsText($"{name}, Veuillez lancer les dés. Lancement automatique dans {actionTimeout - _timer:0.00} seconde(s)");


                yield return null; // Wait until the next frame
            }

            // If the player didn't perform the action, notify them
            if (!hasPerformedAction)
            {
                yield return Play();
            }
        }


        public void AskPlayFromButton()
        {
            _askedPlayFromButton = true;
        }

        public IEnumerator Play()
        {
            DisableAllActionButtons();
            if (_askedPlayFromButton)
            {
                _monopolyGameManager.SetGameTextEventsText($"{name} a jeté les dés.");
            }
            else
            {
                _monopolyGameManager.SetGameTextEventsText("les dés ont été jetés automatiquement.");
            }
        
            int rolledResult = 0;
            foreach (int gottenResult in _monopolyGameManager.AskAPlayerToRollDices(this))
            {
                yield return null;
                rolledResult = gottenResult;
                if (rolledResult>0)
                {
                    break;
                }
            }
            _lastRolledResult = rolledResult;
            if (_askedPlayFromButton)
            {
                _monopolyGameManager.SetGameTextEventsText($"{name} a joué {rolledResult}.");
            }else
            {

                _monopolyGameManager.SetGameTextEventsText($"le résultat du lancé est {rolledResult}.");
            }
            yield return new WaitForSeconds(2f);
        
            _askedPlayFromButton = false;
            yield return _monopolyGameManager.APlayerRolledDice(this, rolledResult);

        }

        public void DisableAllActionButtons()
        {
            _throwDices.SetButtonInteractable(false);
        }

        public void EnableRollDiceAction()
        {
            _throwDices.SetButtonInteractable(true);
        }

        public bool CanBeChargedOf(int dueAmount)
        {
            return money >= dueAmount;
        }

        public int ChargedOf(int dueAmount)
        {
            money -= dueAmount;
            _playerSummaryButton.Refresh();
            return money;
        }

        public void HaveWon(int won)
        {
            money += won;
            _playerSummaryButton.Refresh();
        }

        public IEnumerator GatherMoneyToReach(int chargedOf)
        {
            while (!CanBeChargedOf(chargedOf) && HavePurchasedTiles() && CanContinuePlaying())
            {
                IGood good = deck.GetSmallestGoodToSell();
                if (good != null)
                {
                    PurchasableTile goodPurchasableTile = good.GetPurchasableTile();
                    int oldCost = goodPurchasableTile.GetLevelCost();
                    good.Sell();
                    int sellAmount = good.Sell();;
                    HaveWon(sellAmount);
                
                    _monopolyGameManager.SetGameTextEventsText($"{name} a {good.GetSellText()}. Il a gagné {sellAmount}M.");
                    yield return new WaitForSeconds(1f);
                    string newCost = goodPurchasableTile.IsFullyOwned()? goodPurchasableTile.GetCostText():"0M";
                    _monopolyGameManager.SetGameTextEventsText($"{name}, votre nouveau solde est de {money}M. Son prix de passage passe de {oldCost}M à {newCost}");
                    yield return new WaitForSeconds(1f);
                }
            }

            if (!CanContinuePlaying())
            {
                _monopolyGameManager.SetGameTextEventsText($"{name} ne peut pas payer {chargedOf}.");
                yield return new WaitForSeconds(1f);
            }
            yield return null;
        }

        public bool CanContinuePlaying()
        {
            return money > -1 || HavePurchasedTiles();
        }

        public bool HavePurchasedTiles()
        {
            return deck.HaveAtLeastOneNotMortgagedCard();
        }

        public int GetAllHousesNumber()
        {
            return deck.GetAllOfType<PropertyTile>().Select(propertyTile => propertyTile.GetHousesNumber()).Sum();
        }

        public int GetAllHotelsNumber()
        {
            return deck.GetAllOfType<PropertyTile>().Select(propertyTile => propertyTile.GetHotelNumber()).Sum();
        }

        public void GoInPrison()
        {
            Debug.Assert(currentTile is PrisonOrVisitTile, "Sorry make sure prison move to prison first.");
            prison = 3;
            if (adoptPuppyCards.Count > 0)
            {
                _communityFreeFromPrisonButton.SetButtonInteractable(true);
            }

            if (getOutOfJailChanceCards.Count > 0)
            {
                _chanceFreeFromPrisonButton.SetButtonInteractable(true);
            }
        }

        public void DisableAllFreeFromPrisonButtons()
        {
            _communityFreeFromPrisonButton.SetButtonInteractable(false);
            _chanceFreeFromPrisonButton.SetButtonInteractable(false);

        }

        public int prison { get; private set; }

        public bool IsInPrison()
        {
            return prison > 0;
        }
        public void FreeFromPrison()
        {
            prison = 0;
            DisableAllFreeFromPrisonButtons();
        }
        public bool CanPlay()
        {
            return CanContinuePlaying() && !IsInPrison();
        }

        public void HaveWonAGetOutOfJailChanceCard(GetOutOfJailCard getOutOfJailCard)
        {
            getOutOfJailChanceCards.Add(getOutOfJailCard);
            _chanceFreeFromPrisonButton.SetCardCanBeUsedInFuture(getOutOfJailChanceCards.Count);
        }
        public void HaveWonAnAdoptAPuppyCommunityCard(AdoptPuppyCard adoptPuppyCard)
        {
            adoptPuppyCards.Add(adoptPuppyCard);
            _communityFreeFromPrisonButton.SetCardCanBeUsedInFuture(adoptPuppyCards.Count);
        }

        public void ClickOnChanceCommunityFreeFromPrisonButton<T>() where T : SpecialCard
        {
            if (typeof(T) == typeof(ChanceCard))
            {
                HandleCardClick(getOutOfJailChanceCards, _chanceFreeFromPrisonButton);
            }
            else if (typeof(T) == typeof(CommunityCard))
            {
                HandleCardClick(adoptPuppyCards, _communityFreeFromPrisonButton);
            }
            else
            {
                Debug.LogError($"The card type {typeof(T).Name} is not supported.");
            }
        }

        public void HandleCardClick<TCard>(List<TCard> cardList, FreeFromPrisonButton fromPrisonButton) where TCard : SpecialCard
        {
            if (cardList.Count == 0)
            {
                // Debug.LogError($"No cards available in the list for {typeof(TCard).Name}.");
                return;
            }
            TCard card = cardList[0];
            FreeFromPrison();
            _monopolyGameManager.SetGameTextEventsText($"{name} a utilisé la carte {card.description} pour se libérer de prison.");
            cardList.RemoveAt(0);
            fromPrisonButton.SetCardCanBeUsedInFuture(cardList.Count);
            _monopolyGameManager.TakeACardFromPlayer(card);
        }

        public void Buy(PurchasableTile purchasableTile)
        {
            _monopolyGameManager.BuyPurchasableTileBy(this, purchasableTile);
        }

        public void BuildOnPropertyTile(PropertyTile propertyTile)
        {
            _monopolyGameManager.BuildOnPropertyTile(propertyTile, this);
        }

        public void BroadCastDiceState(Vector3 positions1, Quaternion rotations1, Vector3 positions2, Quaternion rotations2)
        {
            _monopolyGameManager.BroadCastDiceState(positions1, rotations1, positions2, rotations2);
        }
    }
}

// Ensure this is included for LINQ methods
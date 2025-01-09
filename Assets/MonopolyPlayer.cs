
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Tilemaps;
using Object = System.Object; // Ensure this is included for LINQ methods

public class MonopolyPlayerDeck
{
    private Dictionary<Type, Dictionary<int, PurchasableTile>> _purchasableTiles = new Dictionary<Type, Dictionary<int, PurchasableTile>>();

    // Add any object of type T where T inherits from PurchasableTile
    public void Add<T>(T value) where T : PurchasableTile
    {
        var targetType = value.GetTargetType();
        var groupIndex = value.groupIndex;

        if (!_purchasableTiles.ContainsKey(targetType))
        {
            _purchasableTiles[targetType] = new Dictionary<int, PurchasableTile>();
        }

        if (_purchasableTiles[targetType].ContainsKey(groupIndex))
        {
            Console.WriteLine($"A {typeof(T).Name} with group index {groupIndex} already exists in the {targetType.Name} group.");
            return;
        }

        _purchasableTiles[targetType][groupIndex] = value;
        Console.WriteLine($"Added {typeof(T).Name} with group index {groupIndex} to target type {targetType.Name}.");
    }

    public bool RemoveByGroupAtIndex(Type targetType, int groupIndex)
    {
        if (!typeof(PurchasableTile).IsAssignableFrom(targetType))
        {
            throw new ArgumentException($"Type {targetType} must inherit from PurchasableTile", nameof(targetType));
        }

        // Use reflection to invoke the method dynamically
        var method = typeof(MonopolyPlayerDeck).GetMethod("Remove")?.MakeGenericMethod(targetType);
        if (method != null)
        {
            return (bool)method.Invoke(this, new Object[] { groupIndex });
        }
        else
        {
            throw new ArgumentException("Remove not found");
        }
        
    }
    // Remove an object by group index
    public bool Remove<T>(int groupIndex) where T : PurchasableTile
    {
        var targetType = typeof(T);

        if (_purchasableTiles.ContainsKey(targetType) && _purchasableTiles[targetType].ContainsKey(groupIndex))
        {
            _purchasableTiles[targetType].Remove(groupIndex);
            Console.WriteLine($"Removed {typeof(T).Name} with group index {groupIndex} from {targetType.Name} group.");
            return true;
        }

        return false;
    }

    // Get an object by group index
    public T Get<T>(int groupIndex) where T : PurchasableTile
    {
        var targetType = typeof(T);

        if (_purchasableTiles.ContainsKey(targetType) && _purchasableTiles[targetType].ContainsKey(groupIndex))
        {
            return (T)_purchasableTiles[targetType][groupIndex];
        }

        return null;
    }

    // Display all entries grouped by type
    public void Display()
    {
        foreach (var group in _purchasableTiles)
        {
            Console.WriteLine($"Type: {group.Key.Name}");
            foreach (var item in group.Value)
            {
                Console.WriteLine($"    Group Index: {item.Key}, Tile: {item.Value}");
            }
        }
    }

    // Get all objects of a specific type
    public IEnumerable<T> GetAllOfType<T>() where T : PurchasableTile
    {
        var targetType = typeof(T);

        if (_purchasableTiles.ContainsKey(targetType))
        {
            return _purchasableTiles[targetType].Values.OfType<T>();
        }

        return Enumerable.Empty<T>();
    }

    // Get the count of objects of a specific type
    public int GetCountOfType<T>() where T : PurchasableTile
    {
        var targetType = typeof(T);

        return _purchasableTiles.ContainsKey(targetType) ? _purchasableTiles[targetType].Count : 0;
    }

    // Check if a specific type has at least one non-mortgaged card
    public bool HaveAtLeastOneNotMortgagedCard()
    {
        return GetAllTiles().Any(tile => !tile.IsMortgaged);
    }

    // Get the smallest good to sell across all groups
    public IGood GetSmallestGoodToSell()
    {
        return _purchasableTiles.Values
            .SelectMany(group => group.Values)
            .Select(tile => tile.GetMinimumGoodToSell())
            .Where(good => good != null)
            .OrderBy(good => good.GetSellPrice())
            .FirstOrDefault();
    }

    // Access the entire collection (read-only)
    public IReadOnlyDictionary<Type, Dictionary<int, PurchasableTile>> GetCollection()
    {
        return _purchasableTiles;
    }
    public IEnumerable<PurchasableTile> GetAllGroupOfThisPropertyTile(Type targetType)
    {
        if (!typeof(PurchasableTile).IsAssignableFrom(targetType))
        {
            throw new ArgumentException($"Type {targetType} must inherit from PurchasableTile", nameof(targetType));
        }

        // Use reflection to invoke the method dynamically
        var method = typeof(MonopolyPlayerDeck).GetMethod("GetAllOfType")?.MakeGenericMethod(targetType);
        if (method != null)
        {
            return (IEnumerable<PurchasableTile>)method.Invoke(this, null);
        }
        else
        {
            throw new ArgumentException("GetAllOfType not found");
        }
        
    }
    // Get an array of all PurchasableTile objects sorted by their TileIndex
    public PurchasableTile[] GetAllTilesSortedByTileIndex()
    {
        return GetAllTiles() // Flatten all tiles into a single collection
            .OrderBy(tile => tile.GetTileIndex()) // Sort by TileIndex
            .ToArray(); // Convert to an array
    }

    public IEnumerable<PurchasableTile> GetAllTiles()
    {
        return _purchasableTiles.Values
            .SelectMany(group => group.Values);
    }
}
public class MonopolyPlayer
{
    public MonopolyPlayer(string playerName, PlayerSummaryButton playerSummaryButton,
        PlayerElementOnMap playerElementOnMap, 
        ThrowDices throwDices,
        FreeFromPrisonButton communityFreeFromPrisonButton,
        FreeFromPrisonButton chanceFreeFromPrisonButton,
        MonopolyGameManager monopolyGameManager)
    {
        deck = new MonopolyPlayerDeck();
        name = playerName;
        _playerSummaryButton = playerSummaryButton;
        _playerSummaryButton.setPlayer(this);
        _playerElementOnMap = playerElementOnMap;
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

    public MonopolyPlayerDeck deck{get; private set;}
    private ThrowDices _throwDices;
    private FreeFromPrisonButton _communityFreeFromPrisonButton;
    private FreeFromPrisonButton _chanceFreeFromPrisonButton;
    public List<GetOutOfJailCard> getOutOfJailChanceCards{get; private set;}
    public List<AdoptPuppyCard> adoptPuppyCards{get; private set;}
    public string name{get; private set;}
    private PlayerSummaryButton _playerSummaryButton;
    public PlayerElementOnMap _playerElementOnMap{get; private set;}
    public int money { get; private set; } = 0;
    public PlayerContent PlayerContent;
    private MonopolyGameManager _monopolyGameManager;
    private bool _askedPlayFromButton = false;
    public bool hasPerformedAction { get; private set; } = false;

    private float _timer = 0f;

    public BoardTile tile { get; private set; }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayerContent.SetSelfMadePlayer(this);
    }

    public void IncrementMoneyWith(int amount)
    {
        money += amount;
        _playerSummaryButton.Refresh();
        // PlayerContent.updateMoney(money);
    }
    public void DecrementMoneyWith(int amount)
    {
        money -= amount;
        _playerSummaryButton.Refresh();
    }
    public void MoveTo(BoardTile tileToLandOn)
    {
        tile = tileToLandOn;
        // Debug.Log($"assigning tile {tile.tileGameObject}");
        _playerElementOnMap.transform.position = new Vector3(tile.getTransform().position.x,
            _playerElementOnMap.transform.position.y, tile.getTransform().position.z);
        // PlayerContent.UpdateTile(tile);

    }
    public void DicesRoll(int rollResult, bool allEqual)
    {
        hasPerformedAction = true;
        // PlayerContent.SetDicesRolled(rollResult, allEqual);
    }
    // Update is called once per frame
    void Update()
    {
         
    }

    // The method that each player will call during their turn
    public IEnumerator TriggerPlay(float actionTimeout)
    {
        yield return TriggerPlay(actionTimeout,actionTimeout);
    }
    public IEnumerator TriggerPlay(float rollDiceTimeout, float userBuyTileTimeout)
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

    private IEnumerator HandlePlayerBuyAction(float actionTimeout)
    {
        _timer = 0f;
        if (tile is PurchasableTile purchasableTile)
        {
            foreach (PurchasableTile p in _monopolyGameManager.GetAllGroupOfThisPropertyTile(purchasableTile.GetTargetType()))
            {
                if (p.CanBeBought())
                {
                    _monopolyGameManager.BuyPurchasableTileBy(this, p);
                }
            }
            
            if (tile.CanBeBought() && tile.getPrice()<=money)
            {
                _monopolyGameManager.gameCardBuy.ShowPurchasableCard(purchasableTile, this);
                // Wait for the player to perform an action or timeout
                while (_timer < actionTimeout && _monopolyGameManager.gameCardBuy.gameObject.activeSelf)
                {

                    _timer += Time.deltaTime;
                    _monopolyGameManager.GameTextEvents.SetText($"{name}, Veuillez decidez {actionTimeout-_timer:0.00} seconde(s)");
                    yield return null; // Wait until the next frame
                }

                if (purchasableTile.IsOwnedBy(this))
                {
                    _monopolyGameManager.SetGameTextEventsText($"{name} a acquéris {tile.TileName}");
                }
                else
                {
                    _monopolyGameManager.SetGameTextEventsText($"{name} a décliné l'offre {tile.TileName}");
                }
                _monopolyGameManager.gameCardBuy.Hide();
                yield return new WaitForSeconds(1.5f);
                //PlayerContent.EnableBuyAction(tile.getPrice());
            }else if (purchasableTile.IsOwnedBy(this))
            {
                if(purchasableTile is PropertyTile propertyTile){
                    if (propertyTile.CanBeUpgraded())
                    {
                        PropertyTileState oldPropertyState = propertyTile.propertyTileState;
                        int oldCost = propertyTile.GetLevelCost();
                        _monopolyGameManager.gameCardBuild.ShowPurchasableCard(purchasableTile, this);
                        while (_timer < actionTimeout && _monopolyGameManager.gameCardBuild.gameObject.activeSelf)
                        {

                            _timer += Time.deltaTime;
                            _monopolyGameManager.GameTextEvents.SetText(
                                $"{name}, Veuillez decider {actionTimeout - _timer:0.00} seconde(s)");
                            yield return null; // Wait until the next frame
                        }

                        if (oldPropertyState.CompareTo(propertyTile.propertyTileState) > 0)
                        {
                            _monopolyGameManager.SetGameTextEventsText(
                                $"{name} a construit un {propertyTile.propertyTileState.GetName()} sur {tile.TileName}. Le prix de passage passe de {oldCost}M à {propertyTile.GetLevelCost()}");
                        }
                        else
                        {
                            _monopolyGameManager.SetGameTextEventsText(
                                $"{name} n'a pas fait des travaux sur {tile.TileName}");
                        }

                        _monopolyGameManager.gameCardBuild.Hide();
                        yield return new WaitForSeconds(1.5f);
                    }
                    else
                    {
                        
                        _monopolyGameManager.SetGameTextEventsText(
                            $"{name} ne peut plus faire de travaux sur {tile.TileName}.");
                        yield return new WaitForSeconds(3f);
                    }
                }
                
                _monopolyGameManager.SetGameTextEventsText(
                    $"{name}, chaque joueur passant sur {tile.TileName} devra vous payer {purchasableTile.GetLevelCost()}, vous {purchasableTile.GetLevelText()}.");
                yield return new WaitForSeconds(4f);
            }
            else
            {
                _monopolyGameManager.SetGameTextEventsText(
                    $"{name} doit payer {purchasableTile.GetLevelCost()} à {purchasableTile.GetOwner().name}, ({purchasableTile.GetOwner().name} {purchasableTile.GetLevelText()}).");
                yield return new WaitForSeconds(4f);
            }
        }
        else
        {
            _monopolyGameManager.GameTextEvents.SetText($"{name} ne peut effectuer aucune action");
            yield return new WaitForSeconds(4f);
        }

    }

    private IEnumerator HandlePlayerRollDice(float actionTimeout)
    {
        hasPerformedAction = false;
        _timer = 0f;

        EnableRollDiceAction();
        // Wait for the player to perform an action or timeout
        while (_timer < actionTimeout)
        {
            if (hasPerformedAction || _askedPlayFromButton)
            {
                break;
            }
            _timer += Time.deltaTime;
            
            _monopolyGameManager.GameTextEvents.SetText($"{name}, Veuillez lancer les dés. Lancement automatique dans {actionTimeout-_timer:0.00} seconde(s)");
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
    private IEnumerator Play()
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
        
        int rolledResult = 1;
        /*foreach (int gottenResult in _monopolyGameManager.AskAPlayerToRollDices(this))
        {
            yield return null;
            rolledResult = gottenResult;
            if (rolledResult>0)
            {
                break;
            }
        }*/

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

    private void DisableAllActionButtons()
    {
        _throwDices.SetButtonInteractable(false);
    }
    private void EnableRollDiceAction()
    {
        _throwDices.SetButtonInteractable(true);
    }

    public bool canBeChargedOf(int dueAmount)
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
        money = 99999;
        _playerSummaryButton.Refresh();
    }

    public IEnumerator GatherMoneyToReach(int chargedOf)
    {
        while (!canBeChargedOf(chargedOf) && HavePurchasedTiles() && CanContinuePlaying())
        {
            IGood good = deck.GetSmallestGoodToSell();
            if (good != null)
            {
                int sellAmount = good.Sell();;
                HaveWon(sellAmount);
                _monopolyGameManager.SetGameTextEventsText($"{name} a vendu {good}. Nouveau solde {sellAmount}.");
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

    private bool HavePurchasedTiles()
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
        Debug.Assert(tile is PrisonOrVisitTile, "Sorry make sure prison move to prison first.");
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

    private void DisableAllFreeFromPrisonButtons()
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

    private void HandleCardClick<TCard>(List<TCard> cardList, FreeFromPrisonButton fromPrisonButton) where TCard : SpecialCard
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
        _monopolyGameManager.BuildOnPropertyTile(propertyTile);
    }
}
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

public interface IGood
{
    public int GetSellPrice();
    public int Sell();
    public bool CanBeSelled();
}
public abstract class Good: IGood
{
    // You can leave this method abstract or provide a default implementation.
    public abstract int GetSellPrice();
    public abstract int Sell();
    public abstract bool CanBeSelled();

    public abstract string GetGoodName();
}

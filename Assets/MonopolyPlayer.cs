
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Tilemaps; // Ensure this is included for LINQ methods


public class MonopolyPlayerDeck
{
    private Dictionary<int, PurchasableTile> _purchasableTiles = new Dictionary<int, PurchasableTile>();

    // Add any object of type T where T inherits from Mother
    public void Add<T>(int key, T value) where T : PurchasableTile
    {
        if (!_purchasableTiles.ContainsKey(key))
        {
            _purchasableTiles.Add(key, value);
            Console.WriteLine($"Added {typeof(T).Name} with key {key}");
        }
        else
        {
            Console.WriteLine($"{typeof(T).Name} with key {key} already exists.");
        }
    }

    // Remove an object by key
    public bool RemoveByKey(int key)
    {
        if (_purchasableTiles.ContainsKey(key))
        {
            _purchasableTiles.Remove(key);
            Console.WriteLine($"Removed object with key {key}");
            return true;
        }
        return false;
    }

    // Remove an object by reference
    public bool RemoveObject<T>(T value) where T : PurchasableTile
    {
        var key = FindKey<T>(value);
        if (key.HasValue)
        {
            _purchasableTiles.Remove(key.Value);
            Console.WriteLine($"Removed {typeof(T).Name} object: {value}");
            return true;
        }
        return false;
    }

    // Get an object by key
    public T Get<T>(int key) where T : PurchasableTile
    {
        if (_purchasableTiles.ContainsKey(key) && _purchasableTiles[key] is T)
        {
            return (T)_purchasableTiles[key];
        }
        return null;
    }

    // Display all entries
    public void Display()
    {
        foreach (var item in _purchasableTiles)
        {
            Console.WriteLine($"Key: {item.Key}, Type: {item.Value.GetType().Name}");
        }
    }

    // Get the count of objects of a specific type T
    public int GetCountOfType<T>() where T : PurchasableTile
    {
        return _purchasableTiles.Values.Count(value => value is T);
    }

    // Helper to find the key of an object
    private int? FindKey<T>(T value) where T : PurchasableTile
    {
        foreach (var item in _purchasableTiles)
        {
            if (item.Value is T && EqualityComparer<T>.Default.Equals((T)item.Value, value))
            {
                return item.Key;
            }
        }
        return null;
    }

    public bool HaveAtLeastOneCard()
    {
        return _purchasableTiles.Count > 0;
    }

    public IGood GetSmallestGoodToSell()
    {
        // Transform the dictionary into a collection of prices
        return _purchasableTiles
            .Select(tile => tile.Value.GetMinimumGoodToSell())  // Get the price from each tile
            .Where(good=>good!=null).OrderBy<IGood, object>(good => good.GetSellPrice())                      // Sort by price
            .FirstOrDefault();  
        /*
         return _purchasableTiles
            .OrderBy(tile => tile.Value.GetSmallestGood())  // Sort by the price
            .FirstOrDefault().Value.GetSmallestGood();  // Get the first (smallest) one
            */
    }
}
public class MonopolyPlayer
{
    public MonopolyPlayer(string playerName, PlayerSummaryButton playerSummaryButton,
        PlayerElementOnMap playerElementOnMap, ThrowDices throwDices,
        MonopolyGameManager monopolyGameManager)
    {
        deck = new MonopolyPlayerDeck();
        name = playerName;
        _playerSummaryButton = playerSummaryButton;
        _playerSummaryButton.setPlayer(this);
        _playerElementOnMap = playerElementOnMap;
        _throwDices = throwDices;
        _throwDices.SetSelfMadePlayer(this);
        _monopolyGameManager = monopolyGameManager;
    }

    public MonopolyPlayerDeck deck{get; private set;}
    private ThrowDices _throwDices;
    public string name{get; private set;}
    private PlayerSummaryButton _playerSummaryButton;
    private PlayerElementOnMap _playerElementOnMap;
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
        yield return HandlePlayerRollDice(rollDiceTimeout);
        yield return HandleUserDoAction(userBuyTileTimeout);
    }

    private IEnumerator HandleUserDoAction(float actionTimeout)
    {
        _timer = 0f;
        if (tile is PurchasableTile)
        {
            if (tile.CanBeBought() && tile.getPrice()<=money)
            {
                _monopolyGameManager.gameCardBuy.ShowPurchasableTile((PurchasableTile)tile);
                // Wait for the player to perform an action or timeout
                while (_timer < actionTimeout && _monopolyGameManager.gameCardBuy.gameObject.activeSelf)
                {

                    _timer += Time.deltaTime;
                    _monopolyGameManager.GameTextEvents.SetText($"{this}, Veiullez decidez {actionTimeout-_timer:0.00} seconde(s)");
                    yield return null; // Wait until the next frame
                }
                //PlayerContent.EnableBuyAction(tile.getPrice());
            }
        }else if (tile is TaxTile taxTile)
        {
            _monopolyGameManager.GameTextEvents.SetText($"{this} a paye une taxe de {taxTile.taxAmount}M");
            yield return new WaitForSeconds(1.5f);
        }else if (tile is SpecialTile specialTile)
        {
            _monopolyGameManager.GameTextEvents.SetText($"{this} est sur une case speciale");
            yield return new WaitForSeconds(1.5f);
        }
        else
        {
            _monopolyGameManager.GameTextEvents.SetText($"{this} ne peut effectuer aucune action");
            yield return new WaitForSeconds(1.5f);
        }

        _monopolyGameManager.gameCardBuy.Hide();
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
                if (_askedPlayFromButton)
                {
                    _askedPlayFromButton = false;
                }
                break;
            }
            _timer += Time.deltaTime;
            
            _monopolyGameManager.GameTextEvents.SetText($"{this}, Veiullez lancer les des. Lancement automatique dans {actionTimeout-_timer:0.00} seconde(s)");
            yield return null; // Wait until the next frame
        }

        // If the player didn't perform the action, notify them
        if (!hasPerformedAction)
        {
            yield return _playerElementOnMap.StartCoroutine(Play());
        }

    }


    public void AskPlayFromButton()
    {
        _askedPlayFromButton = true;
    }
    private IEnumerator Play()
    {
        DisableAllActionButtons();
        hasPerformedAction = false;
        _monopolyGameManager.PlayerRollDice(this);
        while (!hasPerformedAction)
        {
            yield return null; 
        }
        hasPerformedAction = false;
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
        return money > dueAmount;
    }

    public int ChargedOf(int dueAmount)
    {
        money -= dueAmount;
        return money;
    }

    public void HaveWon(int chargedOf)
    {
        money += chargedOf;
    }

    public IEnumerator GatherMoneyToReach(int chargedOf)
    {
        while (!canBeChargedOf(chargedOf) && HavePurchasedTiles() && PlayerCanContinuePlaying())
        {
            IGood good = deck.GetSmallestGoodToSell();
            if (good != null)
            {
                int sellAmount = good.Sell();;
                HaveWon(sellAmount);
                _monopolyGameManager.SetGameTextEventsText($"{this} a vendu {good}. Nouveau solde {sellAmount}.");
                yield return new WaitForSeconds(1f);
            }
        }

        if (!PlayerCanContinuePlaying())
        {
            _monopolyGameManager.SetGameTextEventsText($"{this} a perdu le jeu. Il n'a plus d'argent.");
            yield return new WaitForSeconds(1f);
        }
        yield return null;
    }

    private bool PlayerCanContinuePlaying()
    {
        return money > 0;
    }

    private bool HavePurchasedTiles()
    {
        return deck.HaveAtLeastOneCard();
    }
}

public interface IGood
{
    public int GetSellPrice();
    public int Sell();
}
public abstract class Good: IGood
{
    // You can leave this method abstract or provide a default implementation.
    public abstract int GetSellPrice();
    public abstract int Sell();
    public abstract string GetGoodName();
}

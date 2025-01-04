
using UnityEngine;
using System.Collections;

public class MonopolyPlayer
{
    public MonopolyPlayer(string playerName, PlayerSummaryButton playerSummaryButton,
        PlayerElementOnMap playerElementOnMap, ThrowDices throwDices,
        MonopolyGameManager monopolyGameManager)
    {
        
        name = playerName;
        _playerSummaryButton = playerSummaryButton;
        _playerSummaryButton.setPlayer(this);
        _playerElementOnMap = playerElementOnMap;
        _throwDices = throwDices;
        _throwDices.SetSelfMadePlayer(this);
        _monopolyGameManager = monopolyGameManager;
    }
    
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
}


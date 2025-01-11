using UnityEngine;
using System.Collections;
using Monopoly;

public class SelfMadePlayer:MonoBehaviour
{
    public SelfMadePlayer(PlayerSummaryButton playerSummaryButton, PlayerElementOnMap playerElementOnMap)
    {
        _playerSummaryButton = playerSummaryButton;
        _playerElementOnMap = playerElementOnMap;
    }
    public string name{get; private set;}
    private PlayerSummaryButton _playerSummaryButton;
    private readonly PlayerElementOnMap _playerElementOnMap;
    public int money { get; private set; } = 0;
    public PlayerContent PlayerContent;
    public MonopolyGameManager MonopolyGameManager;
    private bool _askedPlayFromButton = false;
    public bool hasPerformedAction { get; private set; } = false;

    private float _timer = 0f;

    public BoardTile tile { get; private set; }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // PlayerContent.SetSelfMadePlayer(this);
    }

    public void IncrementMoneyWith(int amount)
    {
        money += amount;
        PlayerContent.updateMoney(money);
    }
    public void DecrementMoneyWith(int amount)
    {
        money -= amount;
        PlayerContent.updateMoney(money);
    }
    public void MoveTo(BoardTile tileToLandOn)
    {
        tile = tileToLandOn;
        // Debug.Log($"assigning tile {tile.tileGameObject}");
        _playerElementOnMap.transform.position = new Vector3(tile.getTransform().position.x,
            _playerElementOnMap.transform.position.y, tile.getTransform().position.z);
        PlayerContent.UpdateTile(tile);

    }
    public void DicesRoll(int rollResult, bool allEqual)
    {
        hasPerformedAction = true;
        PlayerContent.SetDicesRolled(rollResult, allEqual);
    }
    // Update is called once per frame
    void Update()
    {
         
    }

    // The method that each player will call during their turn
    public IEnumerator TriggerPlay(float actionTimeout)
    {
        yield return HandlePlayerRollDice(actionTimeout);
        yield return HandleUserDoAction(actionTimeout);
    }

    private IEnumerator HandleUserDoAction(float actionTimeout)
    {
        _timer = 0f;

        if (tile.CanBeBought() && tile.getPrice()<=money)
        {
            PlayerContent.EnableBuyAction(tile.getPrice());
        }else
        {
            _timer = actionTimeout;
        }
        // Wait for the player to perform an action or timeout
        while (_timer < actionTimeout)
        {

            _timer += Time.deltaTime;
            
            yield return null; // Wait until the next frame
        }

        PlayerContent.DisableBuyAction();
    }

    private IEnumerator HandlePlayerRollDice(float actionTimeout)
    {
        hasPerformedAction = false;
        _timer = 0f;

        PlayerContent.EnableRollDiceAction();
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
        PlayerContent.DisableAllActionButtons();
        hasPerformedAction = false;
        // MonopolyGameManager.PlayerRollDice(this);
        while (!hasPerformedAction)
        {
            yield return null; 
        }
        hasPerformedAction = false;
    }
}
public class Player : MonoBehaviour
{
    [SerializeField] private string playerName; // Exposed to Unity Editor
    public int Position { get; private set; }

    public void ResetPosition()
    {
        Position = 0;
        transform.position = Vector3.zero; // Reset Unity world position
    }

    public void MoveToPosition(int newPosition, System.Action onMoveComplete)
    {
        Position = newPosition;
        // Optional: Update Unity position here if needed
        // For example: transform.position = new Vector3(newPosition, transform.position.y, transform.position.z);

        // Call the callback after completing the movement
        onMoveComplete?.Invoke();
    }
}

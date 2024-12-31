using UnityEngine;
using System.Collections;
public class SelfmadePlayer : MonoBehaviour
{
    private bool askedPlayFromButton = false;
    private int Money=0;
    public PlayerContent playerContent;
    public MonopolyGameManager monopolyGameManager;
    public bool HasPerformedAction { get; private set; } = false;
    
    float timer = 0f;
    private bool HasPlayed = false;

    public BoardTile tile { get; private set; }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerContent.SetSelfMadePlayer(this);
    }

    public void incrementMoneyWith(int amount)
    {
        Money += amount;
        playerContent.updateMoney(Money);
    }
    public void decrementMoneyWith(int amount)
    {
        Money -= amount;
        playerContent.updateMoney(Money);
    }
    public void MoveTo(BoardTile tile)
    {
        this.tile = tile;
        // Debug.Log($"assigning tile {tile.tileGameObject}");
        transform.position = new Vector3(tile.getTransform().position.x,
            transform.position.y, tile.getTransform().position.z);
        playerContent.UpdateTile(tile);

    }
    public void DicesRoll(int rollResult, bool allEqual)
    {
        HasPerformedAction = true;
        playerContent.SetDicesRolled(rollResult, allEqual);
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
        timer = 0f;

        if (tile.CanBeBought() && tile.getPrice()<=Money)
        {
            playerContent.EnableBuyAction(tile.getPrice());
        }else
        {
            timer = actionTimeout;
        }
        // Wait for the player to perform an action or timeout
        while (timer < actionTimeout)
        {

            timer += Time.deltaTime;
            
            yield return null; // Wait until the next frame
        }

        playerContent.DisableBuyAction();
    }

    private IEnumerator HandlePlayerRollDice(float actionTimeout)
    {
        HasPerformedAction = false;
        timer = 0f;

        playerContent.EnableRollDiceAction();
        // Wait for the player to perform an action or timeout
        while (timer < actionTimeout)
        {
            if (HasPerformedAction || askedPlayFromButton)
            {
                if (askedPlayFromButton)
                {
                    askedPlayFromButton = false;
                }
                break;
            }
            timer += Time.deltaTime;
            
            yield return null; // Wait until the next frame
        }

        // If the player didn't perform the action, notify them
        if (!HasPerformedAction)
        {
            yield return StartCoroutine(Play());
        }

    }

    public void AskPlayFromButton()
    {
        askedPlayFromButton = true;
    }
    private IEnumerator Play()
    {
        playerContent.DisableAllActionButtons();
        HasPerformedAction = false;
        monopolyGameManager.PlayerRollDice(this);
        while (!HasPerformedAction)
        {
            yield return null; 
        }
        HasPerformedAction = false;
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

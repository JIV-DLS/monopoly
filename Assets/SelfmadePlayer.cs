using UnityEngine;
using System.Collections;
public class SelfmadePlayer : MonoBehaviour
{
    public MonopolyGameManager monopolyGameManager;
    public bool HasPerformedAction { get; private set; } = false;
    
    float timer = 0f;
    private bool HasPlayed = false;
    private bool isTriggerPlayed = false;

    public BoardTile tile { get; private set; }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void MoveTo(BoardTile tile)
    {
        this.tile = tile;
        Debug.Log($"assigning tile {tile.tileGameObject}");
        transform.position = new Vector3(tile.getTransform().position.x,
            transform.position.y, tile.getTransform().position.z);
    }
    public void DicesRoll(int rollResult)
    {
        HasPerformedAction = true;
    }
    // Update is called once per frame
    void Update()
    {
         
    }

    // The method that each player will call during their turn
    public IEnumerator TriggerPlay(float actionTimeout)
    {
        if (isTriggerPlayed)
        {
            yield break;
        }
        HasPerformedAction = false;
        
        isTriggerPlayed = true;
        
        timer = 0f;

        Debug.Log("TriggerPlay");
        // Wait for the player to perform an action or timeout
        while (timer < actionTimeout)
        {
            // Simulate action input, e.g., pressing a key (space bar for example)
            if (Input.GetKeyDown(KeyCode.Space)) // You can replace this with actual gameplay input
            {
                yield return StartCoroutine(Play());
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

    private IEnumerator Play()
    {
        HasPerformedAction = false;
        Debug.Log("asked for roll dice");
        monopolyGameManager.PlayerRollDice(this);
        Debug.Log("after asked for roll dice");
        while (!HasPerformedAction)
        {
            yield return null; 
        }
        HasPerformedAction = false;
        isTriggerPlayed = false;
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

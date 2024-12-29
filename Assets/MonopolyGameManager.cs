using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class MonopolyGameManager : MonoBehaviour
{
    public List<Player> players;
    public Board board;
    public Dice dice;
    private int currentPlayerIndex;
    private GameState gameState;

    private void Start()
    {
        // InitializeGame();
    }

    private void InitializeGame()
    {
        currentPlayerIndex = 0;
        gameState = GameState.WaitingForRoll;
        foreach (var player in players)
        {
            player.ResetPosition();
        }
    }

    private void Update()
    {
        /*switch (gameState)
        {
            case GameState.WaitingForRoll:
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    RollDice();
                }
                break;

            case GameState.MovingPiece:
                break;

            case GameState.TurnEnd:
                AdvanceToNextPlayer();
                break;
        }*/
    }

    public void DicesRoll(int rollResult, bool allEqual)
    {
        showPlayerDiceResultToPanel(rollResult);
        showPlayerEqualDicesToPanel(allEqual);
        Debug.Log($"This is the dices roll result: {rollResult}");
    }

    private void showPlayerEqualDicesToPanel(bool allEqual)
    {
        GameObject playerDicesThrownEqualValues = GameObject.Find("ThrownEqualDicesValue");

        if (allEqual && playerDicesThrownEqualValues != null)
        {
            // Get the TextMeshProUGUI component
            TextMeshProUGUI textComponent = playerDicesThrownEqualValues.GetComponent<TextMeshProUGUI>();

            if (textComponent != null)
            {
                // Set the text of the TextMeshProUGUI component
                int oldValue = -1;
                if (int.TryParse(textComponent.text, out oldValue))
                {
                    if (oldValue < 3)
                    {
                        oldValue++;
                        textComponent.text = oldValue.ToString();
                    }
                    else
                    {
                        CurrentPlayerThrownEqualDicesThreeTimes();
                        textComponent.text = "0";
                    }
                }else
                {
                    Debug.LogError("Fail to parse text component.");
                }
                
            }
            else
            {
                Debug.LogError("TextMeshProUGUI component not found on the GameObject.");
            }
        }
        else
        {
            Debug.LogError("GameObject not found with the specified name.");
        }
    }

    private void CurrentPlayerThrownEqualDicesThreeTimes()
    {
        GoToPrison();
    }

    private void IsYourTour()
    {
        CountPrison();
    }
    private void GoToPrison()
    {
        GameObject prisonValues = GameObject.Find("PrisonValue");

        if (prisonValues != null)
        {
            // Get the TextMeshProUGUI component
            TextMeshProUGUI textComponent = prisonValues.GetComponent<TextMeshProUGUI>();

            if (textComponent != null)
            {
                // Set the text of the TextMeshProUGUI component
                textComponent.text = "3";

            }
            else
            {
                Debug.LogError("TextMeshProUGUI component not found on the GameObject.");
            }
        }
        else
        {
            Debug.LogError("GameObject not found with the specified name.");
        }
    }
    private void CountPrison()
    {
        GameObject prisonValues = GameObject.Find("PrisonValue");

        if (prisonValues != null)
        {
            // Get the TextMeshProUGUI component
            TextMeshProUGUI textComponent = prisonValues.GetComponent<TextMeshProUGUI>();

            if (textComponent != null)
            {
                // Set the text of the TextMeshProUGUI component
                int oldValue = -1;
                if (int.TryParse(textComponent.text, out oldValue))
                {
                    if (oldValue > 0)
                    {
                        oldValue--;
                        textComponent.text = oldValue.ToString();
                    }
                }else
                {
                    Debug.LogError("Fail to parse text component.");
                }
                
            }
            else
            {
                Debug.LogError("TextMeshProUGUI component not found on the GameObject.");
            }
        }
        else
        {
            Debug.LogError("GameObject not found with the specified name.");
        }
    }
    private static void showPlayerDiceResultToPanel(int rollResult)
    {
        // Get the GameObject by its name
        GameObject playerDicesThrownResultValue = GameObject.Find("ThrownResultValue");

        if (playerDicesThrownResultValue != null)
        {
            // Get the TextMeshProUGUI component
            TextMeshProUGUI textComponent = playerDicesThrownResultValue.GetComponent<TextMeshProUGUI>();

            if (textComponent != null)
            {
                // Set the text of the TextMeshProUGUI component
                textComponent.text = $"{rollResult}";
            }
            else
            {
                Debug.LogError("TextMeshProUGUI component not found on the GameObject.");
            }
        }
        else
        {
            Debug.LogError("GameObject not found with the specified name.");
        }
    }

    private void RollDice()
    {
        int roll = dice.Roll();
        DiceThrownResult(roll);
    }

    public void DiceThrownResult(int roll)
    {
        Debug.Log($"Player {currentPlayerIndex + 1} rolled {roll}");
        MovePlayer(players[currentPlayerIndex], roll);
    }

    private void MovePlayer(Player player, int roll)
    {
        gameState = GameState.MovingPiece;
        int newPosition = (player.Position + roll) % board.spaces.Count;
        player.MoveToPosition(newPosition, () =>
        {
            HandleLanding(player, board.spaces[newPosition]);
            gameState = GameState.TurnEnd;
        });
    }

    private void HandleLanding(Player player, BoardSpace space)
    {
        Debug.Log($"Player {player.name} landed on {space.name}");
        // Implement property buying, paying rent, etc.
    }

    private void AdvanceToNextPlayer()
    {
        currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
        Debug.Log($"Player {currentPlayerIndex + 1}'s turn!");
        gameState = GameState.WaitingForRoll;
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

public class Dice
{
    public int Roll()
    {
        return Random.Range(1, 7) + Random.Range(1, 7);
    }
}

public class Board
{
    public List<BoardSpace> spaces;
}

public class BoardSpace
{
    public string name;
    // Add fields like property details, card effects, etc.
}

public enum GameState
{
    WaitingForRoll,
    MovingPiece,
    TurnEnd
}
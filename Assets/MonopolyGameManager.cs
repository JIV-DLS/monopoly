using System.Collections.Generic;
using UnityEngine;

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

    public void DicesRoll(int rollResult)
    {
        
        Debug.Log($"This is the dices roll result: {rollResult}");
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

public class Player
{
    public string name;
    public int Position { get; private set; }

    public void ResetPosition()
    {
        Position = 0;
    }

    public void MoveToPosition(int newPosition, System.Action onMoveComplete)
    {
        Position = newPosition;
        // Optionally animate the movement
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
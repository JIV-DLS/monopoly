using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
public class MonopolyGameManager : MonoBehaviour
{
    public List<SelfmadePlayer> players;
    public SelfmadePlayer localPlayer;
    private SelfmadePlayer currentPlayer; // The player whose turn it is

    private Board board;
    public Dice dice;
    public DicesManager dicesManager;
    private int currentPlayerIndex;
    private GameState gameState;
    
    public enum BoardSide { Bottom, Right, Top, Left }

    public Material tileMaterial; // Single material for all tiles
    public Color[] borderColors; // Array of colors for each border (assign 4 colors)
    public float tileHeight = 1.0f; // Height for both corner and regular tiles

    private const int regularTileCount = 9; // Number of regular tiles per side
    private float actionTimeout = 5f; // Wait time of 5 seconds
    private bool isGameRunning = true;


    private void Start()
    {
        // InitializeGame();
        CreateBoard();
        currentPlayer = localPlayer;
        // Start the waiting process
        StartCoroutine(GameLoop());
    }
    

    private IEnumerator<Coroutine> GameLoop()
    {
        while (isGameRunning)
        {
            // Call the player's Play method to start their turn
            yield return StartCoroutine(currentPlayer.TriggerPlay(actionTimeout));

            // Switch to the next player
            // currentPlayer = (currentPlayer == player1) ? player2 : player1;

            // Optionally add a delay between turns
            yield return null; // 1 second delay before next turn
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

    void CreateBoard()
    {
        // Get the plane's Renderer to determine its size
        Renderer planeRenderer = GetComponent<Renderer>();
        if (planeRenderer == null)
        {
            Debug.LogError("The object does not have a Renderer component. Make sure this script is attached to a plane.");
            return;
        }

        Vector3 planeSize = planeRenderer.bounds.size;
        float planeWidth = planeSize.x;
        float planeHeight = planeSize.z;

        // Create the 4 sides of the board
        board = new Board(CreateSide(transform.position, planeWidth, planeHeight, BoardSide.Bottom),
        CreateSide(transform.position, planeWidth, planeHeight, BoardSide.Left),
        CreateSide(transform.position, planeWidth, planeHeight, BoardSide.Top),
        CreateSide(transform.position, planeWidth, planeHeight, BoardSide.Right));
        
    }

    GameObject[] CreateSide(Vector3 center, float planeWidth, float planeHeight, BoardSide side)
    {
        // Determine the side index for the color
        int sideIndex = (int)side;
        GameObject[] sideTiles = new GameObject[regularTileCount+1];
        // Ensure we have enough colors for all sides
        Color sideColor = borderColors != null && borderColors.Length > sideIndex 
            ? borderColors[sideIndex] 
            : Color.white;

        // Determine the length of the current side
        float sideLength = side == BoardSide.Bottom || side == BoardSide.Top ? planeWidth : planeHeight;

        // Calculate the width of the regular tiles
        float regularTileWidth = (sideLength - 2 * tileHeight) / regularTileCount;

        // Offset to move the tiles along the border
        Vector3 offset = GetSideOffset(planeWidth, planeHeight, side);

        // Corner Tiles
        /*Vector3 cornerPosition1 = center + offset + GetCornerTileOffset(sideLength, side, true);
        CreateTile(cornerPosition1, tileHeight, tileHeight, $"{side} Corner 1", sideColor);*/

        Vector3 cornerPosition2 = center + offset + GetCornerTileOffset(sideLength, side, false);
        sideTiles[0] = CreateTile(cornerPosition2, tileHeight, tileHeight, $"{side} Corner 2", sideColor);

        // Regular Tiles
        for (int i = 0; i < regularTileCount; i++)
        {
            Vector3 regularPosition = center + offset + GetRegularTileOffset(i, sideLength, regularTileWidth, side);
            GameObject createdTile = CreateTile(regularPosition, regularTileWidth, tileHeight, $"{side} Tile {i + 1}", sideColor);
            if (side == BoardSide.Left || side == BoardSide.Right)
            {
                createdTile.transform.rotation = Quaternion.Euler(0, 90, 0);
                sideTiles[i+1] = createdTile;
            }else{
                sideTiles[regularTileCount-i] = createdTile;
            }
        }
        return sideTiles;
    }

    Vector3 GetSideOffset(float width, float height, BoardSide side)
    {
        switch (side)
        {
            case BoardSide.Bottom:
                return new Vector3(0, 0, -height *7/ 16);
            case BoardSide.Right:
                return new Vector3(width *7/ 16, 0, 0);
            case BoardSide.Top:
                return new Vector3(0, 0, height *7/ 16);
            case BoardSide.Left:
                return new Vector3(-width *7/ 16, 0, 0);
            default:
                return Vector3.zero;
        }
    }

    Vector3 GetCornerTileOffset(float sideLength, BoardSide side, bool isFirstCorner)
    {
        float offset = (sideLength / 2 - tileHeight / 2) * (isFirstCorner ? -1 : 1);

        switch (side)
        {
            case BoardSide.Bottom:
                return new Vector3(offset, 0, 0);
            case BoardSide.Right:
                return new Vector3(0, 0, offset);
            case BoardSide.Top:
                return new Vector3(-offset, 0, 0);
            case BoardSide.Left:
                return new Vector3(0, 0, -offset);
            default:
                return Vector3.zero;
        }
    }

    Vector3 GetRegularTileOffset(int index, float sideLength, float tileWidth, BoardSide side)
    {
        float positionOffset = (index + 1) * tileWidth - tileWidth / 2 + tileHeight;

        switch (side)
        {
            case BoardSide.Bottom:
                return new Vector3(-sideLength / 2 + positionOffset, 0, 0);
            case BoardSide.Right:
                return new Vector3(0, 0, sideLength / 2 - positionOffset);
            case BoardSide.Top:
                return new Vector3(sideLength / 2 - positionOffset, 0, 0);
            case BoardSide.Left:
                return new Vector3(0, 0, -sideLength / 2 + positionOffset);
            default:
                return Vector3.zero;
        }
    }

    GameObject CreateTile(Vector3 position, float width, float height, string name, Color color)
    {
        // Create a transparent cube for the tile
        GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Cube);
        // Get the BoxCollider component
        BoxCollider boxCollider = tile.GetComponent<BoxCollider>();

        // Check if it exists (it will always exist for primitives unless removed)
        if (boxCollider != null)
        {
            // Disable the BoxCollider
            boxCollider.enabled = false;
        }
        tile.transform.position = position;
        tile.transform.localScale = new Vector3(width, 2f, height);
        tile.name = name;

        // Apply the material and set the color
        Renderer renderer = tile.GetComponent<Renderer>();
        if (tileMaterial != null)
        {
            renderer.material = tileMaterial;
            renderer.material.color = color; // Dynamically set the color for this tile
        }
        else
        {
            Debug.LogWarning($"No material assigned for {name}. Using default material.");
        }

        // Parent the tile to the plane for better organization
        tile.transform.parent = transform;
        return tile;
    }
    private void InitializeGame()
    {
        currentPlayerIndex = 0;
        gameState = GameState.WaitingForRoll;
        /*foreach (var player in players)
        {
            player.ResetPosition();
        }*/
    }

    public void PlayerRollDice(SelfmadePlayer player)
    {
        dicesManager.ThrowDice(player);
    }
    public void DicesRoll(SelfmadePlayer player, int rollResult, bool allEqual)
    {
        Debug.Log($"Player {player.name} Roll Result: {rollResult} {board} {board.GetTile(0)}");
        player.DicesRoll(rollResult);
        BoardTile playerTile = player.tile;
        if (playerTile == null)
        {
            playerTile = board.GetTile(0);
        }

        bool passHome = false;
        player.MoveTo(board.GetTile(board.MoveFromTile(playerTile, rollResult, out passHome)));
        //showPlayerDiceResultToPanel(rollResult);
        //showPlayerEqualDicesToPanel(allEqual);
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
        //MovePlayer(players[currentPlayerIndex], roll);
    }

    private void MovePlayer(Player player, int roll)
    {
        /*gameState = GameState.MovingPiece;
        int newPosition = (player.Position + roll) % board.tiles.Count;
        player.MoveToPosition(newPosition, () =>
        {
            HandleLanding(player, board.tiles[newPosition]);
            gameState = GameState.TurnEnd;
        });*/
    }

    private void HandleLanding(Player player, BoardTile tile)
    {
        Debug.Log($"Player {player.name} landed on {tile}");
        // Implement property buying, paying rent, etc.
    }

    private void AdvanceToNextPlayer()
    {
        currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
        Debug.Log($"Player {currentPlayerIndex + 1}'s turn!");
        gameState = GameState.WaitingForRoll;
    }
}




public class Dice
{
    public int Roll()
    {
        return UnityEngine.Random.Range(1, 7) + UnityEngine.Random.Range(1, 7);
    }
}

public class Board
{
    private BoardTile[] tiles;
    private Dictionary<int, BoardTile> tileLookup;
    private Dictionary<BoardTile, int> indexLookup;

    public Board(GameObject[] bottom, GameObject[] left, GameObject[] top, GameObject[] right)
    {
        tiles = new BoardTile[40];
        tileLookup = new Dictionary<int, BoardTile>(40);
        indexLookup = new Dictionary<BoardTile, int>(40);

        PopulateTiles(bottom, 0);
        PopulateTiles(left, bottom.Length);
        PopulateTiles(top, bottom.Length + left.Length);
        PopulateTiles(right, bottom.Length + left.Length + top.Length);
    }

    private void PopulateTiles(GameObject[] side, int startIndex)
    {
        for (int i = 0; i < side.Length; i++)
        {
            BoardTile boardTile;
            switch (i)
            {
                case 0:
                    boardTile = new StartTile(side[i]);
                    break;
                case 1:
                    boardTile = new PropertyTile(side[i], "Boulevard de Belleville",
                        new int[] { 2, 4, 10, 30, 90, 160, 250 },
                        50, 50, 60);
                    break;
                case 2:
                    boardTile = new CommunityTile(side[i]);
                    break;
                case 3:
                    boardTile = new PropertyTile(side[i], "Rue Lecourbe",
                        new int[] { 4, 8, 20, 60, 180, 320, 450 },
                        50, 50, 60);
                    break;
                case 4:
                    boardTile = new TaxTile(side[i], "Impôts sur le revenu", 200);
                    break;
                case 5:
                    boardTile = new RailroadTile(side[i], "Gare Mont-Parnasse");
                    break;
                case 6:
                    boardTile = new PropertyTile(side[i], "Rue De Vaugirard",
                        new int[] { 6, 12, 30, 90, 270, 400, 550 },
                        50, 50, 100);
                    break;
                case 7:
                    boardTile = new ChanceTile(side[i]);
                    break;
                case 8:
                    boardTile = new PropertyTile(side[i], "Rue De Courcelles",
                        new int[] { 6, 12, 30, 90, 270, 400, 550 },
                        50, 50, 100);
                    break;
                case 9:
                    boardTile = new PropertyTile(side[i], "Avenue de la Republique",
                        new int[] { 8, 16, 40, 100, 300, 450, 600 },
                        50, 50, 120);
                    break;
                case 10:
                    boardTile = new PrisonOrVisitTile(side[i]);
                    break;
                case 11:
                    boardTile = new PropertyTile(side[i], "Boulevard de Villette",
                        new int[] { 10, 20, 50, 150, 450, 625, 750 },
                        100, 100, 140);
                    break;
                case 12:
                    boardTile = new ElectricityTile(side[i]);
                    break;
                case 13:
                    boardTile = new PropertyTile(side[i], "Avenue de Neuilly",
                        new int[] { 10, 20, 50, 150, 450, 625, 750 },
                        100, 100, 140);
                    break;
                case 14:
                    boardTile = new PropertyTile(side[i], "Rue de Paradis",
                        new int[] { 12, 24, 60, 180, 500, 700, 900 },
                        100, 100, 160);
                    break;
                case 15:
                    boardTile = new RailroadTile(side[i], "Gare de Lyon");
                    break;
                case 16:
                    boardTile = new PropertyTile(side[i], "Avenue Mozart",
                        new int[] { 14, 28, 70, 200, 550, 750, 950 },
                        100, 100, 180);
                    break;
                case 17:
                    boardTile = new CommunityTile(side[i]);
                    break;
                case 18:
                    boardTile = new PropertyTile(side[i], "Boulevard Saint-Michel",
                        new int[] { 14, 28, 70, 200, 550, 750, 950 },
                        100, 100, 180);
                    break;
                case 19:
                    boardTile = new PropertyTile(side[i], "Place Pigalle",
                        new int[] { 16, 32, 80, 220, 600, 800, 1000 },
                        100, 100, 200);
                    break;
                case 20:
                    boardTile = new FreeParcTile(side[i]);
                    break;
                case 21:
                    boardTile = new PropertyTile(side[i], "Avenue Matignon",
                        new int[] { 18, 36, 90, 250, 700, 875, 1050 },
                        150, 150, 220);
                    break;
                case 22:
                    boardTile = new ChanceTile(side[i]);
                    break;
                case 23:
                    boardTile = new PropertyTile(side[i], "Boulevard Malesherbes",
                        new int[] { 18, 36, 90, 250, 700, 875, 1050 },
                        150, 150, 220);
                    break;
                case 24:
                    boardTile = new PropertyTile(side[i], "Avenue Henri-Martin",
                        new int[] { 20, 40, 100, 300, 750, 925, 1100 },
                        150, 150, 240);
                    break;
                case 25:
                    boardTile = new RailroadTile(side[i], "Gare du Nord");
                    break;
                case 26:
                    boardTile = new PropertyTile(side[i], "Faubourg Saint-Honoré",
                        new int[] { 22, 44, 110, 330, 800, 975, 1150 },
                        150, 150, 260);
                    break;
                case 27:
                    boardTile = new PropertyTile(side[i], "Place de la Bourse",
                        new int[] { 22, 44, 110, 330, 800, 975, 1150 },
                        150, 150, 260);
                    break;
                case 28:
                    boardTile = new WaterPumpTile(side[i]);
                    break;
                case 29:
                    boardTile = new PropertyTile(side[i], "Rue de la Fayette",
                        new int[] { 24, 48, 120, 360, 850, 1025, 1200 },
                        150, 150, 280);
                    break;
                case 30:
                    boardTile = new GoInPrisonTile(side[i]);
                    break;
                case 31:
                    boardTile = new PropertyTile(side[i], "Avenue de Breteuil",
                        new int[] { 26, 52, 130, 390, 900, 1100, 1275 },
                        200, 200, 300);
                    break;
                case 32:
                    boardTile = new PropertyTile(side[i], "Avenue Foch",
                        new int[] { 26, 52, 130, 390, 900, 1100, 1275 },
                        200, 200, 300);
                    break;
                case 33:
                    boardTile = new CommunityTile(side[i]);
                    break;
                case 34:
                    boardTile = new PropertyTile(side[i], "Boulvard des Capucines",
                        new int[] { 28, 56, 150, 450, 1000, 1200, 1400 },
                        200, 200, 320);
                    break;
                case 35:
                    boardTile = new RailroadTile(side[i], "Gare Saint Lazarre");
                    break;
                case 36:
                    boardTile = new ChanceTile(side[i]);
                    break;
                case 37:
                    
                    boardTile = new PropertyTile(side[i], "Avenue des Champs-Elysées",
                        new int[] { 35, 70, 175, 500, 1100, 1300, 1500 },
                        200, 200, 350);
                    break;
                case 38:

                    boardTile = new TaxTile(side[i], "Taxe de Luxe", 100);
                    break;
                case 39:
                    
                    boardTile = new PropertyTile(side[i], "Rue de la Paix",
                        new int[] { 50, 100, 200, 600, 1400, 1700, 2000 },
                        200, 200, 400);
                    break;
                default:
                    throw new InvalidOperationException($"Unknown case: {i}"); 
            }

            int index = startIndex + i;
            Debug.Log($"adding {boardTile.tileGameObject} at index {index}");
            tiles[index] = boardTile;
            tileLookup[index] = boardTile;
            indexLookup[boardTile] = index;
        }
    }

    public BoardTile GetTile(int index)
    {
        return tileLookup.TryGetValue(index, out var tile) ? tile : null;
    }

    public int GetTileIndex(BoardTile tile)
    {
        int x = indexLookup.TryGetValue(tile, out var index) ? index : -1;
        Debug.Log($"Tile {tile} is at index {x}");
        return x;
    }

    // Move the tile by a certain number of spaces, returning the new index and if modulo was applied
    public int MoveFromTile(BoardTile tile, int moveCount, out bool moduloApplied)
    {
        int currentIndex = GetTileIndex(tile);
        if (currentIndex == -1)
        {
            throw new System.ArgumentException("The provided tile is not part of the board.");
        }

        // Calculate the new index after the move
        int newIndex = currentIndex + moveCount;
        Debug.Log($"Moving from {currentIndex} to {newIndex} with {moveCount} moves");
        // Check if modulo is needed to wrap around the board
        moduloApplied = false;

        if (newIndex >= tiles.Length)
        {
            newIndex %= tiles.Length;
            moduloApplied = true;
        }
        else if (newIndex < 0)
        {
            newIndex = (newIndex + tiles.Length) % tiles.Length;
            moduloApplied = true;
        }
        
        Debug.Log($"Final new index {newIndex} {GetTile(newIndex).tileGameObject}");
        return newIndex;
    }
}

public abstract class BoardTile
{
    public string TileName { get; }
    public GameObject tileGameObject { get; private set; }
    public BoardTile(GameObject tileGameObject, string name)
    {
        this.tileGameObject = tileGameObject;
        TileName = name;
    }

    public Transform getTransform()
    {
        return tileGameObject.transform;
    }
    
    public void OnPlayerLanded(SelfmadePlayer player)
    {
        Debug.Log($"Player {player} landed on {TileName}.");
    }
}

public class CornerTile : BoardTile
{
    
    public CornerTile(GameObject tileGameObject, string tileName)
        : base(tileGameObject, tileName)
    {
    }
    
    public override void OnPlayerLanded(Player player)
    {
    }
}
public class PurchasableTile : BoardTile
{
    private int[] Costs { get; }
    private int Price { get; }
    private int MortgageCost { get; }
    private int MortgageFinishedCost { get;  }

    protected PurchasableTile(GameObject tileGameObject, string name, int[] costs, int price,
        int mortgageCost, int mortgageFinishedCost)
        : base(tileGameObject, name)
    {
        Costs = costs;
        Price = price;
        MortgageCost = mortgageCost;
        MortgageFinishedCost = mortgageFinishedCost;
    }

    protected PurchasableTile(GameObject tileGameObject, string name, int[] costs, int price)
        : this(tileGameObject, name, costs, price, price / 2, price / 2 + (int)Math.Round(price / 20.0))
    {
    }
    public override void OnPlayerLanded(Player player)
    {
    }
}
public abstract class PublicServiceTile : PurchasableTile
{
    public int BaseCost { get; }
    public string Owner { get; private set; }

    public PublicServiceTile(GameObject tileGameObject, string name, int[] costs, int price)
        : base(tileGameObject, name, costs, price)
    {
    }
    public PublicServiceTile(GameObject tileGameObject, string name)
        : base(tileGameObject, name, new int[] { 4, 10 }, 150)
    {
    }

    public override void OnPlayerLanded(Player player)
    {
        if (Owner == null)
        {
            //Console.WriteLine($"{player.Name} can buy {TileName}.");
        }
        else
        {
            // Implement rent logic for public services
            //Console.WriteLine($"{player.Name} pays rent to {Owner} for using {TileName}.");
            // For example, rent is based on dice rolls or number of houses owned
        }
    }
}

public class ElectricityTile : PublicServiceTile
{
    public ElectricityTile(GameObject tileGameObject)
        : base(tileGameObject, "Compagnie de distribution d'électricité")
    {
    }

    // Override rent calculation if necessary (e.g., based on dice roll)
    public override void OnPlayerLanded(Player player)
    {
        if (Owner == null)
        {
            ////Console.WriteLine($"{player.Name} can buy {TileName}.");
        }
        else
        {
            ////Console.WriteLine($"{player.Name} pays rent to {Owner} for using {TileName}.");
            // Rent could be based on the dice roll or another player’s ownership of other utilities
        }
    }
}

public class WaterPumpTile : PublicServiceTile
{
    public WaterPumpTile(GameObject tileGameObject)
        : base(tileGameObject, "Compagnie de distribution des Eaux")
    {
    }

    // Override rent calculation if necessary
    public override void OnPlayerLanded(Player player)
    {
        if (Owner == null)
        {
            //Console.WriteLine($"{player.Name} can buy {TileName}.");
        }
        else
        {
            //Console.WriteLine($"{player.Name} pays rent to {Owner} for using {TileName}.");
            // Rent calculation logic similar to electricity
        }
    }
}
public class StartTile : CornerTile
{
    public StartTile(GameObject tileGameObject)
        : base(tileGameObject, "Depart")
    {
    }

    public override void OnPlayerLanded(Player player)
    {
        //Console.WriteLine($"{player.Name} passed the start tile and collects $200!");
    }
}
public class PrisonOrVisitTile : CornerTile
{
    public PrisonOrVisitTile(GameObject tileGameObject)
        : base(tileGameObject, "Simple Visite/En Prison")
    {
    }

}
public class FreeParcTile : CornerTile
{
    public FreeParcTile(GameObject tileGameObject)
        : base(tileGameObject, "Parc Gratuit")
    {
    }

}
public class GoInPrisonTile : CornerTile
{
    public GoInPrisonTile(GameObject tileGameObject)
        : base(tileGameObject, "Allez en prison")
    {
    }

}
public class PropertyTile : PurchasableTile
{
    private int HouseCost { get; }
    private int HotelCost { get; }
    public string Owner { get; private set; }

    public PropertyTile(GameObject tileGameObject, string name, int[] costs, int houseCost, int hotelCost,
        int price,
        int mortgageCost, int mortgageFinishedCost)
        : base(tileGameObject, name, costs, price, mortgageCost, mortgageFinishedCost)
    {
        HouseCost = houseCost;
        HotelCost = hotelCost;
        Owner = null; // No owner initially
    }
    public PropertyTile(GameObject tileGameObject, string name, int[] costs, int houseCost, int hotelCost,
        int price)
        : base(tileGameObject, name, costs, price)
    {
        HouseCost = houseCost;
        HotelCost = hotelCost;
        Owner = null; // No owner initially
    }

    public override void OnPlayerLanded(Player player)
    {
        if (Owner == null)
        {
            //Console.WriteLine($"{player.Name} can buy {Name}.");
        }
        else
        {
            //Console.WriteLine($"{player.Name} pays rent to {Owner}.");
            // Implement rent logic
        }
    }
}


public class RailroadTile : PurchasableTile
{
    public RailroadTile(GameObject tileGameObject, string name)
        : base(tileGameObject, name, new int[] { 25, 50, 100, 200 }, 200)
    {
    }
    public RailroadTile(GameObject tileGameObject, string name, int[] costs,int price, 
    int mortgageCost, int mortgageFinishedCost)
        : base(tileGameObject, name, costs, price, mortgageCost, mortgageFinishedCost)
    {
    }

    public override void OnPlayerLanded(Player player)
    {
        /*if (Owner == null)
        {
            //Console.WriteLine($"{player.Name} can buy {Name}.");
        }
        else
        {
            //Console.WriteLine($"{player.Name} pays rent to {Owner}.");
            // Implement rent logic based on railroads owned
        }*/
    }
}

public class TaxTile : BoardTile
{
    public int TaxAmount { get; }

    public TaxTile(GameObject tileGameObject, string name, int taxAmount)
        : base(tileGameObject, name)
    {
        TaxAmount = taxAmount;
    }

    public override void OnPlayerLanded(Player player)
    {
        //Console.WriteLine($"{player.Name} pays a tax of {TaxAmount}.");
        // Deduct tax from player's balance
    }
}

public class ChanceTile : BoardTile
{
    public ChanceTile(GameObject tileGameObject)
        : base(tileGameObject, "Chance")
    {
    }

    public override void OnPlayerLanded(Player player)
    {
        //Console.WriteLine($"{player.Name} lands on Chance. Draw a card!");
        // Implement Chance card logic (move player, collect money, etc.)
    }

}

public class CommunityTile : BoardTile
{
    public CommunityTile(GameObject tileGameObject)
        : base(tileGameObject, "Community")
    {
    }

    public override void OnPlayerLanded(Player player)
    {
        //Console.WriteLine($"{player.Name} lands on Community. Draw a card!");
        // Implement Community card logic (move player, collect money, etc.)
    }

}

public enum GameState
{
    WaitingForRoll,
    MovingPiece,
    TurnEnd
}
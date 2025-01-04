using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Collections;

public class MonopolyGameManager : MonoBehaviour
{
    public GameCardBuy gameCardBuy;
    private PlayersHorizontalView _playersHorizontalView;
    private PlayerPieceOnBoardBuilder _playerPieceOnBoardBuilder;
    // public List<SelfmadePlayer> players;
    private MonopolyPlayer localPlayer;
    private MonopolyPlayer currentPlayer; // The player whose turn it is
    public BaseTextHandler GameTextEvents;
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
    private float rollDiceTimeout = 3f; // Wait time of 5 seconds
    private float buyTileTimeout = 7f; // Wait time of 5 seconds
    private bool isGameRunning = true;

    private void Awake()
    {
        
        // InitializeGame();
        CreateBoard();
        
        _playersHorizontalView  = GetComponentInChildren<PlayersHorizontalView>();
        
        if (_playersHorizontalView == null)
        {
            Debug.LogError($"No PlayersHorizontalView {_playersHorizontalView}");
        }
        
        _playerPieceOnBoardBuilder  = GetComponentInChildren<PlayerPieceOnBoardBuilder>();
        if (_playerPieceOnBoardBuilder == null)
        {
            Debug.LogError("No PlayerPieceOnBoardBuilder");
        }
        

        localPlayer = new MonopolyPlayer("Toi",_playersHorizontalView.CreateNewChildAtEnd(),
            _playerPieceOnBoardBuilder.Create(PlayerPieceEnum.TopHat, transform),
            GetComponentInChildren<ThrowDices>(),
            this
            );

        MoveAPlayerToATile(localPlayer, board.GetTile(0), false);
        currentPlayer = localPlayer;
        // Start the waiting process
        StartCoroutine(GameLoop());
    }

    private void Start()
    {
    }

    public void MoveAPlayerToATile(MonopolyPlayer player, BoardTile tile, bool passHome)
    {
        if (tile is StartTile || passHome)
        {
            player.IncrementMoneyWith(StartTile.GetStartReward());
        }

        if (tile is TaxTile)
        {
            player.DecrementMoneyWith(((TaxTile)tile).taxAmount);
        }
        player.MoveTo(tile);
        
        
        GameTextEvents.SetText($"Le joueur {currentPlayer} s'est déplacé à {tile.TileName}");

    }

    private IEnumerator GameLoop()
    {
        while (isGameRunning)
        {
            Debug.Log($"Game Loop {gameState}");
            switch (gameState)
            {
                case GameState.WaitingForRoll:
                    GameTextEvents.SetText($"En attente du joueur {currentPlayer}");
                    // Call the player's Play method to start their turn
                    yield return StartCoroutine(currentPlayer.TriggerPlay(rollDiceTimeout, buyTileTimeout));
                    gameState = GameState.TurnEnd;

                    // Switch to the next player
                    // currentPlayer = (currentPlayer == player1) ? player2 : player1;
                    break;

                case GameState.MovingPiece:
                    break;

                case GameState.TurnEnd:

                    // Optionally wait 2 seconds before the next player's turn starts
                    
                    GameTextEvents.SetText($"{currentPlayer} a finit son tour");
                    yield return new WaitForSeconds(1.5f);
                    AdvanceToNextPlayer();

                    break;
            }

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
        CreateSide(transform.position, planeWidth, planeHeight, BoardSide.Right),
        GetComponentInChildren<TitleDeedCard>(true), GetComponentInChildren<RailRoadCard>(true),
        GetComponentInChildren<PublicServiceCard>(true), 
        GetComponentInChildren<CardBehind>(true));
        
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

    public void PlayerRollDice(MonopolyPlayer player)
    {
        GameTextEvents.SetText($"{player} a jete les des...");
        dicesManager.ThrowDice(player);
        
    }
    public void DicesRoll(MonopolyPlayer player, int rollResult, bool allEqual)
    {
        player.DicesRoll(rollResult, allEqual);
        BoardTile playerTile = player.tile;
        if (playerTile == null)
        {
            playerTile = board.GetTile(0);
        }

        bool passHome = false;
        MoveAPlayerToATile(player, board.GetTile(board.MoveFromTile(playerTile, rollResult, out passHome)), passHome);
        player.tile.OnPlayerLanded(player);
        
        GameTextEvents.SetText($"{player} played {rollResult}");
        
        //showPlayerDiceResultToPanel(rollResult);
        //showPlayerEqualDicesToPanel(allEqual);
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
        // Implement property buying, paying rent, etc.
    }

    private void AdvanceToNextPlayer()
    {
        // currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
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
    public TitleDeedCard titleDeedCardPrefab { get; private set; }
    public RailRoadCard railRoadCardPrefab { get; private set; }
    public PublicServiceCard publicServiceCardPrefab { get; private set; }
    public CardBehind cardBehindPrefab { get; private set; }
    public Board(GameObject[] bottom, GameObject[] left, GameObject[] top, GameObject[] right, 
        TitleDeedCard titleDeedCardPrefab,
        RailRoadCard railRoadCardPrefab,
        PublicServiceCard publicServiceCardPrefab,
        CardBehind cardBehindPrefab
        )
    {
        tiles = new BoardTile[40];
        tileLookup = new Dictionary<int, BoardTile>(40);
        indexLookup = new Dictionary<BoardTile, int>(40);
        if (titleDeedCardPrefab == null)
        {
            Debug.LogError("title deed card prefab is null.");
        }
        if (railRoadCardPrefab == null)
        {
            Debug.LogError("rail road card prefab is null.");
        }

        if (publicServiceCardPrefab == null)
        {
            Debug.LogError("public service card prefab is null.");
        }

        if (cardBehindPrefab == null)
        {
            Debug.LogError("card behind prefab is null.");
        }

        this.titleDeedCardPrefab = titleDeedCardPrefab;
        this.railRoadCardPrefab = railRoadCardPrefab;
        this.publicServiceCardPrefab = publicServiceCardPrefab;
        this.cardBehindPrefab = cardBehindPrefab;
        PopulateTiles(bottom, 0);
        PopulateTiles(left, bottom.Length);
        PopulateTiles(top, bottom.Length + left.Length);
        PopulateTiles(right, bottom.Length + left.Length + top.Length);
    }

    private void PopulateTiles(GameObject[] side, int startIndex)
    {
        for (int i = 0; i < side.Length; i++)
        {
            int index = startIndex + i;
            BoardTile boardTile;
            switch (index)
            {
                case 0:
                    boardTile = new StartTile(side[i]);
                    break;
                case 1:
                    boardTile = new PropertyTile(side[i], "Boulevard de Belleville",
                        MonopolyColors.GetColor(MonopolyColors.PropertyColor.Brown),
                        new int[] { 2, 4, 10, 30, 90, 160, 250 },
                        50, 50, 60, titleDeedCardPrefab, cardBehindPrefab);
                    break;
                case 2:
                    boardTile = new CommunityTile(side[i]);
                    break;
                case 3:
                    boardTile = new PropertyTile(side[i], "Rue Lecourbe",
                        MonopolyColors.GetColor(MonopolyColors.PropertyColor.Brown),
                        new int[] { 4, 8, 20, 60, 180, 320, 450 },
                        50, 50, 60, titleDeedCardPrefab, cardBehindPrefab);
                    break;
                case 4:
                    boardTile = new TaxTile(side[i], "Impôts sur le revenu", 200);
                    break;
                case 5:
                    boardTile = new RailroadTile(side[i], "Gare Mont-Parnasse", railRoadCardPrefab, cardBehindPrefab);
                    break;
                case 6:
                    boardTile = new PropertyTile(side[i], "Rue De Vaugirard",
                        MonopolyColors.GetColor(MonopolyColors.PropertyColor.LightBlue),
                        new int[] { 6, 12, 30, 90, 270, 400, 550 },
                        50, 50, 100, titleDeedCardPrefab, cardBehindPrefab);
                    break;
                case 7:
                    boardTile = new ChanceTile(side[i]);
                    break;
                case 8:
                    boardTile = new PropertyTile(side[i], "Rue De Courcelles",
                        MonopolyColors.GetColor(MonopolyColors.PropertyColor.LightBlue),
                        new int[] { 6, 12, 30, 90, 270, 400, 550 },
                        50, 50, 100, titleDeedCardPrefab, cardBehindPrefab);
                    break;
                case 9:
                    boardTile = new PropertyTile(side[i], "Avenue de la Republique",
                        MonopolyColors.GetColor(MonopolyColors.PropertyColor.LightBlue),
                        new int[] { 8, 16, 40, 100, 300, 450, 600 },
                        50, 50, 120, titleDeedCardPrefab, cardBehindPrefab);
                    break;
                case 10:
                    boardTile = new PrisonOrVisitTile(side[i]);
                    break;
                case 11:
                    boardTile = new PropertyTile(side[i], "Boulevard de Villette",
                        MonopolyColors.GetColor(MonopolyColors.PropertyColor.Pink),
                        new int[] { 10, 20, 50, 150, 450, 625, 750 },
                        100, 100, 140, titleDeedCardPrefab, cardBehindPrefab);
                    break;
                case 12:
                    boardTile = new ElectricityTile(side[i], publicServiceCardPrefab, cardBehindPrefab);
                    break;
                case 13:
                    boardTile = new PropertyTile(side[i], "Avenue de Neuilly",
                        MonopolyColors.GetColor(MonopolyColors.PropertyColor.Pink),
                        new int[] { 10, 20, 50, 150, 450, 625, 750 },
                        100, 100, 140, titleDeedCardPrefab, cardBehindPrefab);
                    break;
                case 14:
                    boardTile = new PropertyTile(side[i], "Rue de Paradis",
                        MonopolyColors.GetColor(MonopolyColors.PropertyColor.Pink),
                        new int[] { 12, 24, 60, 180, 500, 700, 900 },
                        100, 100, 160, titleDeedCardPrefab, cardBehindPrefab);
                    break;
                case 15:
                    boardTile = new RailroadTile(side[i], "Gare de Lyon", railRoadCardPrefab, cardBehindPrefab);
                    break;
                case 16:
                    boardTile = new PropertyTile(side[i], "Avenue Mozart",
                        MonopolyColors.GetColor(MonopolyColors.PropertyColor.Orange),
                        new int[] { 14, 28, 70, 200, 550, 750, 950 },
                        100, 100, 180, titleDeedCardPrefab, cardBehindPrefab);
                    break;
                case 17:
                    boardTile = new CommunityTile(side[i]);
                    break;
                case 18:
                    boardTile = new PropertyTile(side[i], "Boulevard Saint-Michel",
                        MonopolyColors.GetColor(MonopolyColors.PropertyColor.Orange),
                        new int[] { 14, 28, 70, 200, 550, 750, 950 },
                        100, 100, 180, titleDeedCardPrefab, cardBehindPrefab);
                    break;
                case 19:
                    boardTile = new PropertyTile(side[i], "Place Pigalle",
                        MonopolyColors.GetColor(MonopolyColors.PropertyColor.Orange),
                        new int[] { 16, 32, 80, 220, 600, 800, 1000 },
                        100, 100, 200, titleDeedCardPrefab, cardBehindPrefab);
                    break;
                case 20:
                    boardTile = new FreeParcTile(side[i]);
                    break;
                case 21:
                    boardTile = new PropertyTile(side[i], "Avenue Matignon",
                        MonopolyColors.GetColor(MonopolyColors.PropertyColor.Red),
                        new int[] { 18, 36, 90, 250, 700, 875, 1050 },
                        150, 150, 220, titleDeedCardPrefab, cardBehindPrefab);
                    break;
                case 22:
                    boardTile = new ChanceTile(side[i]);
                    break;
                case 23:
                    boardTile = new PropertyTile(side[i], "Boulevard Malesherbes",
                        MonopolyColors.GetColor(MonopolyColors.PropertyColor.Red),
                        new int[] { 18, 36, 90, 250, 700, 875, 1050 },
                        150, 150, 220, titleDeedCardPrefab, cardBehindPrefab);
                    break;
                case 24:
                    boardTile = new PropertyTile(side[i], "Avenue Henri-Martin",
                        MonopolyColors.GetColor(MonopolyColors.PropertyColor.Red),
                        new int[] { 20, 40, 100, 300, 750, 925, 1100 },
                        150, 150, 240, titleDeedCardPrefab, cardBehindPrefab);
                    break;
                case 25:
                    boardTile = new RailroadTile(side[i], "Gare du Nord", railRoadCardPrefab, cardBehindPrefab);
                    break;
                case 26:
                    boardTile = new PropertyTile(side[i], "Faubourg Saint-Honoré",
                        MonopolyColors.GetColor(MonopolyColors.PropertyColor.Yellow),
                        new int[] { 22, 44, 110, 330, 800, 975, 1150 },
                        150, 150, 260, titleDeedCardPrefab, cardBehindPrefab);
                    break;
                case 27:
                    boardTile = new PropertyTile(side[i], "Place de la Bourse",
                        MonopolyColors.GetColor(MonopolyColors.PropertyColor.Yellow),
                        new int[] { 22, 44, 110, 330, 800, 975, 1150 },
                        150, 150, 260, titleDeedCardPrefab, cardBehindPrefab);
                    break;
                case 28:
                    boardTile = new WaterPumpTile(side[i], publicServiceCardPrefab, cardBehindPrefab);
                    break;
                case 29:
                    boardTile = new PropertyTile(side[i], "Rue de la Fayette",
                        MonopolyColors.GetColor(MonopolyColors.PropertyColor.Yellow),
                        new int[] { 24, 48, 120, 360, 850, 1025, 1200 },
                        150, 150, 280, titleDeedCardPrefab, cardBehindPrefab);
                    break;
                case 30:
                    boardTile = new GoInPrisonTile(side[i]);
                    break;
                case 31:
                    boardTile = new PropertyTile(side[i], "Avenue de Breteuil",
                        MonopolyColors.GetColor(MonopolyColors.PropertyColor.Green),
                        new int[] { 26, 52, 130, 390, 900, 1100, 1275 },
                        200, 200, 300, titleDeedCardPrefab, cardBehindPrefab);
                    break;
                case 32:
                    boardTile = new PropertyTile(side[i], "Avenue Foch",
                        MonopolyColors.GetColor(MonopolyColors.PropertyColor.Green),
                        new int[] { 26, 52, 130, 390, 900, 1100, 1275 },
                        200, 200, 300, titleDeedCardPrefab, cardBehindPrefab);
                    break;
                case 33:
                    boardTile = new CommunityTile(side[i]);
                    break;
                case 34:
                    boardTile = new PropertyTile(side[i], "Boulvard des Capucines",
                        MonopolyColors.GetColor(MonopolyColors.PropertyColor.Green),
                        new int[] { 28, 56, 150, 450, 1000, 1200, 1400 },
                        200, 200, 320, titleDeedCardPrefab, cardBehindPrefab);
                    break;
                case 35:
                    boardTile = new RailroadTile(side[i], "Gare Saint Lazare", railRoadCardPrefab, cardBehindPrefab);
                    break;
                case 36:
                    boardTile = new ChanceTile(side[i]);
                    break;
                case 37:
                    
                    boardTile = new PropertyTile(side[i], "Avenue des Champs-Elysées",
                        MonopolyColors.GetColor(MonopolyColors.PropertyColor.DarkBlue),
                        new int[] { 35, 70, 175, 500, 1100, 1300, 1500 },
                        200, 200, 350, titleDeedCardPrefab, cardBehindPrefab);
                    break;
                case 38:

                    boardTile = new TaxTile(side[i], "Taxe de Luxe", 100);
                    break;
                case 39:
                    
                    boardTile = new PropertyTile(side[i], "Rue de la Paix",
                        MonopolyColors.GetColor(MonopolyColors.PropertyColor.DarkBlue),
                        new int[] { 50, 100, 200, 600, 1400, 1700, 2000 },
                        200, 200, 400, titleDeedCardPrefab, cardBehindPrefab);
                    break;
                default:
                    throw new InvalidOperationException($"Unknown case: {i}"); 
            }

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
        
        return newIndex;
    }
}

public class BoardTile
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

    public virtual bool CanBeBought()
    {
        return false;
    }

    public virtual int getPrice()
    {
        return 0;
    }
    public void OnPlayerLanded(MonopolyPlayer player)
    {
        // Debug.Log($"Player {player} landed on {TileName}.");
    }
}

public class CornerTile : BoardTile
{
    
    public CornerTile(GameObject tileGameObject, string tileName)
        : base(tileGameObject, tileName)
    {
    }
    
}
public abstract class PurchasableTile : BoardTile
{
    public int[] costs { get; private set; }
    private int price { get; set; }
    public int mortgageCost { get; private set; }
    public int mortgageFinishedCost { get; private set;  }
    public bool isMortgaged { get; private set;  }

    private MonopolyPlayer _monopolyPlayer;
    public override bool CanBeBought()
    {
        return _monopolyPlayer == null;
    }
    
    public override int getPrice()
    {
        return price;
    }
    protected PurchasableTile(GameObject tileGameObject, string name, int[] costs, int price,
        int mortgageCost, int mortgageFinishedCost)
        : base(tileGameObject, name)
    {
        this.costs = costs;
        this.price = price;
        this.mortgageCost = mortgageCost;
        this.mortgageFinishedCost = mortgageFinishedCost;
    }

    public abstract PurchasableFaceCard GetFaceCard();
    public abstract PurchasableBehindCard GetBehindCard();
    protected PurchasableTile(GameObject tileGameObject, string name, int[] costs, int price)
        : this(tileGameObject, name, costs, price, price / 2, price / 2 + (int)Math.Round(price / 20.0))
    {
    }
}
public class PublicServiceTile : PurchasableTile
{
    public PublicServiceCard publicService { get; private set; }
    public CardBehind titleDeedBehindCard { get; private set; }

    public override PurchasableFaceCard GetFaceCard()
    {
        return publicService;
    }
    public  override PurchasableBehindCard GetBehindCard()
    {
        return titleDeedBehindCard;
    }

    public PublicServiceTile(GameObject tileGameObject, string name, PublicServiceCard publicService,
        CardBehind titleDeedBehindCard)
        : base(tileGameObject, name, new int[] { 4, 10 }, 150)
    {
        this.publicService = (PublicServiceCard)publicService.Clone(this);
        this.titleDeedBehindCard = (CardBehind)titleDeedBehindCard.Clone(this);
    }

}

public class ElectricityTile : PublicServiceTile
{
    
    public ElectricityTile(GameObject tileGameObject, PublicServiceCard publicService,
        CardBehind titleDeedBehindCard)
        : base(tileGameObject, "Compagnie de distribution d'électricité", publicService, titleDeedBehindCard)
    {
    }

}

public class WaterPumpTile : PublicServiceTile
{
    public WaterPumpTile(GameObject tileGameObject, PublicServiceCard publicService,
        CardBehind titleDeedBehindCard)
        : base(tileGameObject, "Compagnie de distribution des Eaux", publicService, titleDeedBehindCard)
    {
    }

}
public class StartTile : CornerTile
{
    public static int GetStartReward(){
        return 200;
    }
    public StartTile(GameObject tileGameObject)
        : base(tileGameObject, "Depart")
    {
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
    public int houseCost { get; private set; }
    public int hotelCost { get; private set; }
    public string owner { get; private set; }
    public Color color { get; set; }
    public TitleDeedCard titleDeedFaceCard { get; private set; }
    public CardBehind titleDeedBehindCard { get; private set; }

    public override PurchasableFaceCard GetFaceCard()
    {
        return titleDeedFaceCard;
    }
    public  override PurchasableBehindCard GetBehindCard()
    {
        return titleDeedBehindCard;
    }

    public PropertyTile(GameObject tileGameObject, string name, Color color, int[] costs, int houseCost, int hotelCost,
        int price, TitleDeedCard titleDeedFaceCard, CardBehind titleDeedBehindCard)
        : base(tileGameObject, name, costs, price)
    {
        this.houseCost = houseCost;
        this.hotelCost = hotelCost;
        this.color = color;
        this.titleDeedFaceCard = (TitleDeedCard)titleDeedFaceCard.Clone(this);
        this.titleDeedBehindCard = (CardBehind)titleDeedBehindCard.Clone(this);
        owner = null; // No owner initially
    }

}


public class RailroadTile : PurchasableTile
{
    
    public RailRoadCard railRoadCard { get; private set; }
    public CardBehind titleDeedBehindCard { get; private set; }

    public override PurchasableFaceCard GetFaceCard()
    {
        return railRoadCard;
    }
    public  override PurchasableBehindCard GetBehindCard()
    {
        return titleDeedBehindCard;
    }
    public RailroadTile(GameObject tileGameObject, string name,
        RailRoadCard railRoadCard, CardBehind titleDeedBehindCard)
        : base(tileGameObject, name, new int[] { 25, 50, 100, 200 }, 200)
    {
        this.railRoadCard = (RailRoadCard)railRoadCard.Clone(this);
        this.titleDeedBehindCard = (CardBehind)titleDeedBehindCard.Clone(this);
    }

}

public class TaxTile : BoardTile
{
    public int taxAmount { get; }

    public TaxTile(GameObject tileGameObject, string name, int taxAmount)
        : base(tileGameObject, name)
    {
        this.taxAmount = taxAmount;
    }

}

public class ChanceTile : BoardTile
{
    public ChanceTile(GameObject tileGameObject)
        : base(tileGameObject, "Chance")
    {
    }


}

public class CommunityTile : BoardTile
{
    public CommunityTile(GameObject tileGameObject)
        : base(tileGameObject, "Community")
    {
    }

}

public enum GameState
{
    WaitingForRoll,
    MovingPiece,
    TurnEnd
}
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Collections;
using System.Linq;

public class MonopolyGameManager : MonoBehaviour
{
    public CommunitiesCards communitiesCards{get;private set;}
    public ChancesCards chancesCards{get;private set;}
    public GameCardBuy gameCardBuy;
    private PlayersHorizontalView _playersHorizontalView;
    private PlayerPieceOnBoardBuilder _playerPieceOnBoardBuilder;
    public List<MonopolyPlayer> monopolyPlayers;
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
        

        localPlayer = new MonopolyPlayer("Jojo",_playersHorizontalView.CreateNewChildAtEnd(),
            _playerPieceOnBoardBuilder.Create(PlayerPieceEnum.TopHat, transform),
            GetComponentInChildren<ThrowDices>(),
            ChildUtility.GetChildComponentByName<FreeFromPrisonButton>(transform, "CommunityFreeFromPrisonButton"),
            ChildUtility.GetChildComponentByName<FreeFromPrisonButton>(transform, "ChanceFreeFromPrisonButton"),
            this
            );

        currentPlayer = localPlayer;
        monopolyPlayers = new List<MonopolyPlayer>();
        monopolyPlayers.Add(localPlayer);
        monopolyPlayers.Add(new MonopolyPlayer("Ami",_playersHorizontalView.CreateNewChildAtEnd(),
            _playerPieceOnBoardBuilder.Create(PlayerPieceEnum.BattleShip, transform),
            GetComponentInChildren<ThrowDices>(),
            ChildUtility.GetChildComponentByName<FreeFromPrisonButton>(transform, "CommunityFreeFromPrisonButton"),
            ChildUtility.GetChildComponentByName<FreeFromPrisonButton>(transform, "ChanceFreeFromPrisonButton"),
            this
        ));
        chancesCards = new ChancesCards(this);
        communitiesCards = new CommunitiesCards(this);
        // Start the waiting process
        StartCoroutine(GameLoop());
    }

    private void Start()
    {
    }

    public IEnumerator MoveAPlayerToATile(MonopolyPlayer player, int tileIndex)
    {
        yield return MoveAPlayerToATile(player, board.GetTileAtIndex(tileIndex), false, true);
    }
    public IEnumerator MoveAPlayerToATile(MonopolyPlayer player, BoardTile tile)
    {
        yield return MoveAPlayerToATile(player, tile, false, true);
    }
    public IEnumerator MoveAPlayerToATile(MonopolyPlayer player, BoardTile tile, bool doLandAction)
    {
        yield return MoveAPlayerToATile(player, tile, false, doLandAction);
    }
    public IEnumerator MoveAPlayerToATile(MonopolyPlayer player, BoardTile tile, bool passHome, bool doLandAction)
    {
        player.MoveTo(tile);
        GameTextEvents.SetText($"Le joueur {currentPlayer.name} s'est déplacé à {tile.TileName}");
        
        if (doLandAction)
        {
            yield return new WaitForSeconds(.5f);
            if (tile is StartTile || passHome)
            {
                yield return PassedHome(player);
            }

            if (tile is TaxTile)
            {
                player.DecrementMoneyWith(((TaxTile)tile).taxAmount);
            }
            else if (tile is GoInPrisonTile)
            {
                yield return PutPlayerIntoPrison(player);
            } else if (tile is SpecialBoardTile specialBoardTile)
            {
                GameTextEvents.SetText($"{this} est sur la case speciale {specialBoardTile}");
            
                yield return new WaitForSeconds(.5f);
                if (specialBoardTile is CommunitySpecialTile)
                {
                    CommunityCard communityCard = communitiesCards.GetFromStart();
                    yield return communityCard.TriggerEffectMain(player);
                    if (communityCard is not AdoptPuppyCard)
                    {
                        communitiesCards.Rotate();
                    }
                }
                else if (specialBoardTile is ChanceSpecialTile)
                {
                    
                    ChanceCard chanceCard = chancesCards.GetFromStart();
                    yield return chanceCard.TriggerEffectMain(player);
                    if (chanceCard is not GetOutOfJailCard)
                    {
                        chancesCards.Rotate();
                    }
                }
                yield return new WaitForSeconds(1.5f);
            }
        }
        else
        {
            if (tile is StartTile)
            {
                yield return PassedHome(player);
            }
        }

    }

    private object PassedHome(MonopolyPlayer player)
    {
        player.IncrementMoneyWith(StartTile.GetStartReward());
        GameTextEvents.SetText($"Le joueur {currentPlayer.name} est passé par la case départ. Il reçoit 200M.");
        return new WaitForSeconds(.5f);
    }

    private IEnumerator GameLoop()
    {
        foreach (MonopolyPlayer player in monopolyPlayers)
        {
            yield return MoveAPlayerToStartTile(player);
        }
        while (isGameRunning)
        {
            Debug.Log($"Game Loop {gameState}");
            switch (gameState)
            {
                case GameState.WaitingForRoll:
                    GameTextEvents.SetText($"En attente du joueur {currentPlayer.name}");
                    // Call the player's Play method to start their turn
                    yield return currentPlayer.TriggerPlay(rollDiceTimeout, buyTileTimeout);
                    gameState = GameState.TurnEnd;

                    // Switch to the next player
                    // currentPlayer = (currentPlayer == player1) ? player2 : player1;
                    break;

                case GameState.MovingPiece:
                    break;
                case GameState.GameOver:
                    foreach (var player in monopolyPlayers.Where(player => player.CanContinuePlaying()))
                    {
                        GameTextEvents.SetText($"Le joueur {player.name} a remporté la partie.");
                        break;
                    }
                    break;

                case GameState.TurnEnd:

                    // Optionally wait 2 seconds before the next player's turn starts
                    
                    GameTextEvents.SetText($"{currentPlayer.name} a finit son tour");
                    yield return new WaitForSeconds(1.5f);
                    AdvanceToNextPlayer();

                    break;
            }
            
            var remainingPlayers = monopolyPlayers.Where(player => player.CanContinuePlaying()).ToList();

            if (remainingPlayers.Count == 1)
            {
                var winner = remainingPlayers.First();
                GameTextEvents.SetText($"Le joueur {winner.name} a remporté la partie.");
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

    public IEnumerator APlayerRolledDice(MonopolyPlayer player, int rollResult)
    {
        /*yield return MoveAPlayerToATile(player, board.GetTileAtIndex(board.MoveFromTile(player.tile, rollResult, out var passHome)),
            passHome, true);*/
        yield return MoveAPlayerFromTileByJumping(player, rollResult);
        yield return MoveAPlayerToATile(player, player.tile);
        yield return null;
    }
    public void DicesRoll(MonopolyPlayer player, int rollResult, bool allEqual)
    {
        player.DicesRoll(rollResult, allEqual);
        BoardTile playerTile = player.tile;
        if (playerTile == null)
        {
            playerTile = board.GetTileAtIndex(0);
        }

        bool passHome = false;
        //MoveAPlayerToATile(player, board.GetTileAtIndex(board.MoveFromTile(playerTile, rollResult, out passHome)), passHome);
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
        currentPlayerIndex = (currentPlayerIndex + 1) % monopolyPlayers.Count;
        currentPlayer = monopolyPlayers[currentPlayerIndex];
        gameState = GameState.WaitingForRoll;
    }

    public IEnumerator MoveAPlayerToNextType<T>(MonopolyPlayer monopolyPlayer)
    {
        IEnumerator<(int tileIndex, bool passHome)> moveEnumerator = board.MoveFromTileToNextType<T>(monopolyPlayer.tile);

        // Variable to keep track of the last result
        (int lastTileIndex, bool lastPassHome) = (-1, false);
        
        while (moveEnumerator.MoveNext())
        {
            // Deconstruct the current value
            // Update the last result with the current one
            lastTileIndex = moveEnumerator.Current.tileIndex;
            lastPassHome = moveEnumerator.Current.passHome;

            // Perform actions with the current tile index and passHome flag
            Debug.Log($"Moved to Tile: {lastTileIndex}, Passed Home: {lastPassHome}");

            // Add logic to update your player position or animations here

            yield return MoveAPlayerToATile(monopolyPlayer, board.GetTileAtIndex(lastTileIndex), false, false);
            // Simulate a delay for each step
            yield return new WaitForSeconds(0.09f);
        }
        yield return MoveAPlayerToATile(monopolyPlayer, board.GetTileAtIndex(lastTileIndex), false, false);
    }
    public IEnumerator MoveAPlayerFromTileByJumping(MonopolyPlayer monopolyPlayer, int tileIndex)
    {
        IEnumerator<(int tileIndex, bool passHome)> moveEnumerator = board.MoveAPlayerFromTileByJumping(monopolyPlayer.tile, tileIndex);

        // Variable to keep track of the last result
        (int lastTileIndex, bool lastPassHome) = (-1, false);
        
        while (moveEnumerator.MoveNext())
        {
            // Deconstruct the current value
            // Update the last result with the current one
            lastTileIndex = moveEnumerator.Current.tileIndex;
            lastPassHome = moveEnumerator.Current.passHome;

            // Perform actions with the current tile index and passHome flag
            Debug.Log($"Moved to Tile: {lastTileIndex}, Passed Home: {lastPassHome}");

            // Add logic to update your player position or animations here

            yield return MoveAPlayerToATile(monopolyPlayer, board.GetTileAtIndex(lastTileIndex), false, false);
            // Simulate a delay for each step
            yield return new WaitForSeconds(0.09f);
        }
        yield return MoveAPlayerToATile(monopolyPlayer, board.GetTileAtIndex(lastTileIndex), false, false);
    }public IEnumerator MoveAPlayerToTileIndex(MonopolyPlayer monopolyPlayer, int tileIndex)
    {
        IEnumerator<(int tileIndex, bool passHome)> moveEnumerator = board.MoveAPlayerToTileIndex(monopolyPlayer.tile, tileIndex);

        // Variable to keep track of the last result
        (int lastTileIndex, bool lastPassHome) = (-1, false);
        
        while (moveEnumerator.MoveNext())
        {
            // Deconstruct the current value
            // Update the last result with the current one
            lastTileIndex = moveEnumerator.Current.tileIndex;
            lastPassHome = moveEnumerator.Current.passHome;

            // Perform actions with the current tile index and passHome flag
            Debug.Log($"Moved to Tile: {lastTileIndex}, Passed Home: {lastPassHome}");

            // Add logic to update your player position or animations here

            yield return MoveAPlayerToATile(monopolyPlayer, board.GetTileAtIndex(lastTileIndex), false, false);
            // Simulate a delay for each step
            yield return new WaitForSeconds(0.09f);
        }
        yield return MoveAPlayerToATile(monopolyPlayer, board.GetTileAtIndex(lastTileIndex), false, false);
    }

    public void SetGameTextEventsText(string text)
    {
        GameTextEvents.SetText(text);
    }

    public IEnumerable<int> AskAPlayerToRollDices(MonopolyPlayer monopolyPlayer)
    {
        foreach (int gottenValue in dicesManager.RollDiceAndGetResult(monopolyPlayer))
        {
            yield return gottenValue;
        }
    }
public IEnumerator HandlePayment(MonopolyPlayer payer, MonopolyPlayer receiver, int dueAmount, bool isBankPayment)
{
    if (payer.CanContinuePlaying() && payer.canBeChargedOf(dueAmount))
    {
        ProcessPayment(payer, receiver, dueAmount, isBankPayment);
    }
    else if (payer.CanContinuePlaying())
    {
        yield return WaitForPlayerToBeAbleToBeChargeOf(payer, dueAmount);
        ProcessPayment(payer, receiver, dueAmount, isBankPayment);
    }
    else
    {
        int allPlayerLastMoney = payer.money;
        ProcessPartialPayment(payer, receiver, allPlayerLastMoney, dueAmount, isBankPayment);
        yield return new WaitForSeconds(.5f);
        int amountThatBankMustPay = dueAmount - allPlayerLastMoney;
        ProcessPartialPayment(payer, receiver, amountThatBankMustPay, dueAmount, isBankPayment);
        yield return new WaitForSeconds(1f);
    }
    yield return null;
}

private void ProcessPayment(MonopolyPlayer payer, MonopolyPlayer receiver, int amount, bool isBankPayment)
{
    if (isBankPayment)
    {
        payer.ChargedOf(amount);
        SetGameTextEventsText($"{payer} a payé {amount}M à la banque.");
    }
    else
    {
        receiver.HaveWon(payer.ChargedOf(amount));
        SetGameTextEventsText($"{payer} a payé {amount}M à {receiver}.");
    }
}

private void ProcessPartialPayment(MonopolyPlayer payer, MonopolyPlayer receiver, int amountPaid, int dueAmount, bool isBankPayment)
{
    if (isBankPayment)
    {
        payer.ChargedOf(amountPaid);
        SetGameTextEventsText($"{payer} n'a payé que {dueAmount}M à la banque.");
    }
    else
    {
        receiver.HaveWon(payer.ChargedOf(amountPaid));
        SetGameTextEventsText($"{payer} n'a payé que {dueAmount}M à {receiver}.");
    }
}

public IEnumerator PlayerMustPayToBank(MonopolyPlayer monopolyPlayer, int dueAmount)
{
    return HandlePayment(monopolyPlayer, null, dueAmount, true);
}

public IEnumerator PlayerAPayPlayerB(MonopolyPlayer monopolyPlayer, MonopolyPlayer tileOwner, int dueAmount)
{
    return HandlePayment(monopolyPlayer, tileOwner, dueAmount, false);
}

public IEnumerator PlayerMustPayToEachPlayer(MonopolyPlayer payer, int dueAmount)
{
    foreach (MonopolyPlayer player in monopolyPlayers)
    {
        if (player != payer)
        {
            yield return HandlePayment(payer, player, dueAmount, false);
        }
    }
    yield return null;
}
public IEnumerator AllPlayersPayToPlayer(MonopolyPlayer receiver, int dueAmount)
{
    foreach (MonopolyPlayer player in monopolyPlayers)
    {
        if (player != receiver)
        {
            yield return HandlePayment(player, receiver, dueAmount, false);
        }
    }
    yield return null;
}
    private IEnumerator WaitForPlayerToBeAbleToBeChargeOf(MonopolyPlayer monopolyPlayer, int dueAmount)
    {
        yield return monopolyPlayer.GatherMoneyToReach(dueAmount);
    }

    public T[] GetAllGroupOfThisPropertyTile<T>() where T : PurchasableTile
    {
        return board.GetTilesOfType<T>();
    }

    public IEnumerator MoveAPlayerToStartTile(MonopolyPlayer monopolyPlayer)
    {
        yield return MoveAPlayerToATile(monopolyPlayer, board.GetTileAtIndex(0));
    }

    public IEnumerator MoveAPlayerToLastTile(MonopolyPlayer monopolyPlayer)
    {
        yield return MoveAPlayerToATile(monopolyPlayer, board.GetTileAtLastIndex());
    }

    public IEnumerator PutPlayerIntoPrison(MonopolyPlayer monopolyPlayer)
    {
        yield return MoveAPlayerTo<PrisonOrVisitTile>(monopolyPlayer);
        monopolyPlayer.GoInPrison();
        
        SetGameTextEventsText($"{monopolyPlayer} est partit en prison.");
        yield return new WaitForSeconds(.5f);
    }

    private IEnumerator MoveAPlayerTo<T>(MonopolyPlayer monopolyPlayer) where T:BoardTile
    {
        
        yield return MoveAPlayerToATile(monopolyPlayer, board.OfType<T>().First());
    }

    public IEnumerator MoveBackTo(MonopolyPlayer monopolyPlayer, int i)
    {
        yield return MoveAPlayerToATile(monopolyPlayer, board.GetTileBackFromATileTo(monopolyPlayer, i));
    }
    public IEnumerator GiveCardToPlayer<TCard, TSpecificCard>(
        MonopolyPlayer monopolyPlayer, 
        Func<TCard> takeCardFunc, 
        Action<MonopolyPlayer, TSpecificCard> handleCardAction, 
        string successMessage) 
        where TCard : SpecialCard 
        where TSpecificCard : TCard
    {
        TCard firstCard = takeCardFunc();
        if (firstCard is TSpecificCard specificCard)
        {
            handleCardAction(monopolyPlayer, specificCard);
            SetGameTextEventsText(successMessage);
            yield return new WaitForSeconds(0.5f);
        }
        else
        {
            Debug.LogError($"Sorry, this is not a {typeof(TSpecificCard).Name}");
        }

        yield return null;
    }
    public IEnumerator GiveChanceCardToPlayerGetOutOfJailCard(MonopolyPlayer monopolyPlayer)
    {
        return GiveCardToPlayer<ChanceCard, GetOutOfJailCard>(
            monopolyPlayer,
            () => chancesCards.TakeFromStart(),
            (player, card) => player.HaveWonAGetOutOfJailChanceCard(card),
            $"{monopolyPlayer} a gagné une carte chance pour sortir de prison s'il y va."
        );
        
    }

    public IEnumerator GiveCommunityCardToPlayerToAdoptAPuppyCard(MonopolyPlayer monopolyPlayer)
    {
        return GiveCardToPlayer<CommunityCard, AdoptPuppyCard>(
            monopolyPlayer,
            () => communitiesCards.TakeFromStart(),
            (player, card) => player.HaveWonAnAdoptAPuppyCommunityCard(card),
            $"{monopolyPlayer} a gagné une carte community pour sortir de prison s'il y va."
        );
    }
    public void TakeACardFromPlayer<T>(T card) where T : SpecialCard
    {
        if (card is ChanceCard)
        {
            chancesCards.Add((ChanceCard)(object)card);
        }
        else if (card is CommunityCard)
        {
            communitiesCards.Add((CommunityCard)(object)card);
        }
        else
        {
            Debug.LogError($"The card type {typeof(T).Name} is not supported.");
        }
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
    
    public T[] GetTilesOfType<T>() where T : PurchasableTile
    {
        return tiles.OfType<T>().ToArray();
    }
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
                    boardTile = new BrownPropertyGroupTile(side[i], "Boulevard de Belleville",
                        new int[] { 2, 4, 10, 30, 90, 160, 250 },
                        50, 50, 60, titleDeedCardPrefab, cardBehindPrefab);
                    break;
                case 2:
                    boardTile = new CommunitySpecialTile(side[i]);
                    break;
                case 3:
                    boardTile = new BrownPropertyGroupTile(side[i], "Rue Lecourbe",
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
                    boardTile = new LightBluePropertyGroupTile(side[i], "Rue De Vaugirard",
                        new int[] { 6, 12, 30, 90, 270, 400, 550 },
                        50, 50, 100, titleDeedCardPrefab, cardBehindPrefab);
                    break;
                case 7:
                    boardTile = new ChanceSpecialTile(side[i]);
                    break;
                case 8:
                    boardTile = new LightBluePropertyGroupTile(side[i], "Rue De Courcelles",
                        new int[] { 6, 12, 30, 90, 270, 400, 550 },
                        50, 50, 100, titleDeedCardPrefab, cardBehindPrefab);
                    break;
                case 9:
                    boardTile = new LightBluePropertyGroupTile(side[i], "Avenue de la Republique",
                        new int[] { 8, 16, 40, 100, 300, 450, 600 },
                        50, 50, 120, titleDeedCardPrefab, cardBehindPrefab);
                    break;
                case 10:
                    boardTile = new PrisonOrVisitTile(side[i]);
                    break;
                case 11:
                    boardTile = new PinkPropertyGroupTile(side[i], "Boulevard de Villette",
                        new int[] { 10, 20, 50, 150, 450, 625, 750 },
                        100, 100, 140, titleDeedCardPrefab, cardBehindPrefab);
                    break;
                case 12:
                    boardTile = new ElectricityTile(side[i], publicServiceCardPrefab, cardBehindPrefab);
                    break;
                case 13:
                    boardTile = new PinkPropertyGroupTile(side[i], "Avenue de Neuilly",
                        new int[] { 10, 20, 50, 150, 450, 625, 750 },
                        100, 100, 140, titleDeedCardPrefab, cardBehindPrefab);
                    break;
                case 14:
                    boardTile = new PinkPropertyGroupTile(side[i], "Rue de Paradis",
                        new int[] { 12, 24, 60, 180, 500, 700, 900 },
                        100, 100, 160, titleDeedCardPrefab, cardBehindPrefab);
                    break;
                case 15:
                    boardTile = new RailroadTile(side[i], "Gare de Lyon", railRoadCardPrefab, cardBehindPrefab);
                    break;
                case 16:
                    boardTile = new OrangePropertyGroupTile(side[i], "Avenue Mozart",
                        new int[] { 14, 28, 70, 200, 550, 750, 950 },
                        100, 100, 180, titleDeedCardPrefab, cardBehindPrefab);
                    break;
                case 17:
                    boardTile = new CommunitySpecialTile(side[i]);
                    break;
                case 18:
                    boardTile = new OrangePropertyGroupTile(side[i], "Boulevard Saint-Michel",
                        new int[] { 14, 28, 70, 200, 550, 750, 950 },
                        100, 100, 180, titleDeedCardPrefab, cardBehindPrefab);
                    break;
                case 19:
                    boardTile = new OrangePropertyGroupTile(side[i], "Place Pigalle",
                        new int[] { 16, 32, 80, 220, 600, 800, 1000 },
                        100, 100, 200, titleDeedCardPrefab, cardBehindPrefab);
                    break;
                case 20:
                    boardTile = new FreeParcTile(side[i]);
                    break;
                case 21:
                    boardTile = new RedPropertyGroupTile(side[i], "Avenue Matignon",
                        new int[] { 18, 36, 90, 250, 700, 875, 1050 },
                        150, 150, 220, titleDeedCardPrefab, cardBehindPrefab);
                    break;
                case 22:
                    boardTile = new ChanceSpecialTile(side[i]);
                    break;
                case 23:
                    boardTile = new RedPropertyGroupTile(side[i], "Boulevard Malesherbes",
                        new int[] { 18, 36, 90, 250, 700, 875, 1050 },
                        150, 150, 220, titleDeedCardPrefab, cardBehindPrefab);
                    break;
                case 24:
                    boardTile = new RedPropertyGroupTile(side[i], "Avenue Henri-Martin",
                        new int[] { 20, 40, 100, 300, 750, 925, 1100 },
                        150, 150, 240, titleDeedCardPrefab, cardBehindPrefab);
                    break;
                case 25:
                    boardTile = new RailroadTile(side[i], "Gare du Nord", railRoadCardPrefab, cardBehindPrefab);
                    break;
                case 26:
                    boardTile = new YellowPropertyGroupTile(side[i], "Faubourg Saint-Honoré",
                        new int[] { 22, 44, 110, 330, 800, 975, 1150 },
                        150, 150, 260, titleDeedCardPrefab, cardBehindPrefab);
                    break;
                case 27:
                    boardTile = new YellowPropertyGroupTile(side[i], "Place de la Bourse",
                        new int[] { 22, 44, 110, 330, 800, 975, 1150 },
                        150, 150, 260, titleDeedCardPrefab, cardBehindPrefab);
                    break;
                case 28:
                    boardTile = new WaterPumpTile(side[i], publicServiceCardPrefab, cardBehindPrefab);
                    break;
                case 29:
                    boardTile = new YellowPropertyGroupTile(side[i], "Rue de la Fayette",
                        new int[] { 24, 48, 120, 360, 850, 1025, 1200 },
                        150, 150, 280, titleDeedCardPrefab, cardBehindPrefab);
                    break;
                case 30:
                    boardTile = new GoInPrisonTile(side[i]);
                    break;
                case 31:
                    boardTile = new GreenPropertyGroupTile(side[i], "Avenue de Breteuil",
                        new int[] { 26, 52, 130, 390, 900, 1100, 1275 },
                        200, 200, 300, titleDeedCardPrefab, cardBehindPrefab);
                    break;
                case 32:
                    boardTile = new GreenPropertyGroupTile(side[i], "Avenue Foch",
                        new int[] { 26, 52, 130, 390, 900, 1100, 1275 },
                        200, 200, 300, titleDeedCardPrefab, cardBehindPrefab);
                    break;
                case 33:
                    boardTile = new CommunitySpecialTile(side[i]);
                    break;
                case 34:
                    boardTile = new GreenPropertyGroupTile(side[i], "Boulvard des Capucines",
                        new int[] { 28, 56, 150, 450, 1000, 1200, 1400 },
                        200, 200, 320, titleDeedCardPrefab, cardBehindPrefab);
                    break;
                case 35:
                    boardTile = new RailroadTile(side[i], "Gare Saint Lazare", railRoadCardPrefab, cardBehindPrefab);
                    break;
                case 36:
                    boardTile = new ChanceSpecialTile(side[i]);
                    break;
                case 37:
                    
                    boardTile = new DarkBluePropertyGroupTile(side[i], "Avenue des Champs-Elysées",
                        new int[] { 35, 70, 175, 500, 1100, 1300, 1500 },
                        200, 200, 350, titleDeedCardPrefab, cardBehindPrefab);
                    break;
                case 38:

                    boardTile = new TaxTile(side[i], "Taxe de Luxe", 100);
                    break;
                case 39:
                    
                    boardTile = new DarkBluePropertyGroupTile(side[i], "Rue de la Paix",
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

    public BoardTile GetTileAtIndex(int index)
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

    public IEnumerator<(int tileIndex, bool passHome)> MoveFromTileToNextType<T>(BoardTile monopolyPlayerTile)
    {
        
        int currentIndex = GetTileIndex(monopolyPlayerTile);
        if (currentIndex == -1)
        {
            throw new ArgumentException("The provided tile is not part of the board.");
        }
        bool passHome = false;

        do
        {
            currentIndex = MoveFromTile(GetTileAtIndex(currentIndex), 1, out bool passedHomeOnce);
            passHome = passHome || passedHomeOnce; // Logical OR ensures passHome stays true.
            // Yield the current index and the passHome flag.
            yield return (currentIndex, passHome);
        } while (GetTileAtIndex(currentIndex) is not T);

        // Yield the current index and the passHome flag.
        yield return (currentIndex, passHome);
    }

    public BoardTile GetTileAtLastIndex()
    {
        return tiles.Last();

    }

    public IEnumerable<T> OfType<T>() => tiles.OfType<T>();

    public int GetTileBackFromATileTo(MonopolyPlayer monopolyPlayer, int i)
    {
        int index = GetTileIndex(monopolyPlayer.tile)-i;
        if (index < 0)
        {
            index = (index + tiles.Length) % tiles.Length;
        }

        return index;
    }
    public IEnumerator<(int tileIndex, bool passHome)> MoveAPlayerFromTileByJumping(BoardTile monopolyPlayerTile, int jumpings)
    {
        int currentIndex = GetTileIndex(monopolyPlayerTile);
        if (currentIndex == -1)
        {
            throw new ArgumentException("The provided tile is not part of the board.");
        }

        bool passHome = false;

        for (int i = 0; i < jumpings; i++)
        {
            currentIndex = MoveFromTile(GetTileAtIndex(currentIndex), 1, out bool passedHomeOnce);
            passHome = passHome || passedHomeOnce; // Accumulate the passHome state.

            // Yield the current index and the passHome flag for each jump.
            yield return (currentIndex, passHome);
        }

        // Final yield after completing all jumps.
        yield return (currentIndex, passHome);
    }
    public IEnumerator<(int tileIndex, bool passHome)> MoveAPlayerToTileIndex(BoardTile monopolyPlayerTile, int tileIndex)
    {
        int currentIndex = GetTileIndex(monopolyPlayerTile);
        if (currentIndex == -1)
        {
            throw new ArgumentException("The provided tile is not part of the board.");
        }
        bool passHome = false;

        while (currentIndex != tileIndex)
        {
            currentIndex = MoveFromTile(GetTileAtIndex(currentIndex), 1, out bool passedHomeOnce);
            passHome = passHome || passedHomeOnce; // Accumulate the passHome state.

            // Yield the current index and the passHome flag.
            yield return (currentIndex, passHome);
        }

        // Once at the target tile, yield the final index and the passHome flag.
        yield return (currentIndex, passHome);
    }
}

public class BoardTile
{
    public string TileName { get; }
    public GameObject tileGameObject { get; private set; }
    public MonopolyGameManager monopolyGameManager { get; private set; }
    public BoardTile(GameObject tileGameObject, string name)
    {
        this.tileGameObject = tileGameObject;
        monopolyGameManager = tileGameObject.transform.parent.GetComponent<MonopolyGameManager>();
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

public class PropertyHouseTileGood : PropertyItemTileGood
{
    public PropertyHouseTileGood(PurchasableTile purchasableTile) : base(purchasableTile, $"Maison sur {purchasableTile}")
    {
        
    }
    
    public override int GetSellPrice()
    {
        return GetPropertyTile().houseCost;
    }
}
public class PropertyHotelTileGood : PropertyItemTileGood
{
    public PropertyHotelTileGood(PurchasableTile purchasableTile) : base(purchasableTile, $"Hotel sur {purchasableTile}")
    {
        
    }

    public override int GetSellPrice()
    {
        return GetPropertyTile().hotelCost;
    }
}
public abstract class PropertyItemTileGood : TileGood
{
    protected PropertyItemTileGood(PurchasableTile purchasableTile, string goodName) : base(purchasableTile, goodName)
    {
        
    }

    protected PropertyTile GetPropertyTile()
    {
        return (PropertyTile)purchasableTile;
    }
    public override int Sell()
    {
        GetPropertyTile().DowngradeGood();
        return GetSellPrice();
    }
}
public abstract class TileGood : IGood
{
    protected PurchasableTile purchasableTile{ get; private set; }
    public string goodName{ get; private set; }

    protected TileGood(PurchasableTile purchasableTile, string goodName)
    {
        this.purchasableTile = purchasableTile;
        this.goodName = goodName;
    }

    public virtual int GetSellPrice()
    {
        return purchasableTile.GetSellPrice();
    }

    public virtual int Sell()
    {
        return purchasableTile.Sell();
    }

    public string GetGoodName()
    {
        return goodName;
    }
}
public abstract class PurchasableTile : BoardTile, IGood
{
    public int[] costs { get; private set; }
    private int price { get; set; }
    public int mortgageCost { get; private set; }
    public int mortgageFinishedCost { get; private set;  }
    public bool isMortgaged { get; private set;  }
    public List<TileGood> TileGoods { get; private set; }

    public MonopolyPlayer monopolyPlayer { get; private set; }
    public override bool CanBeBought()
    {
        return monopolyPlayer == null;
    }
    
    public override int getPrice()
    {
        return price;
    }
    private PurchasableTile(GameObject tileGameObject, string name, int[] costs, int price,
        int mortgageCost, int mortgageFinishedCost)
        : base(tileGameObject, name)
    {
        this.costs = costs;
        this.price = price;
        this.mortgageCost = mortgageCost;
        this.mortgageFinishedCost = mortgageFinishedCost;
        TileGoods = new List<TileGood>();
        isMortgaged = false;
    }

    public abstract PurchasableFaceCard GetFaceCard();
    public abstract PurchasableBehindCard GetBehindCard();
    protected PurchasableTile(GameObject tileGameObject, string name, int[] costs, int price)
        : this(tileGameObject, name, costs, price, price / 2, price / 2 + (int)Math.Round(price / 20.0))
    {
    }
    
    public bool IsOwned()
    {
        return monopolyPlayer != null;
    }

    public bool IsOwnedBy(MonopolyPlayer monopolyPlayer)
    {
        return this.monopolyPlayer == monopolyPlayer;
    }
    public MonopolyPlayer GetOwner()
    {
        return monopolyPlayer;
    }

    public virtual IGood GetMinimumGoodToSell()
    {
        return this;
    }

    public int GetSellPrice()
    {
        return getPrice();
    }

    public int Sell()
    {
        isMortgaged = true;
        return GetSellPrice();
    }

    public string GetGoodName()
    {
        return $"Hypothèque sur {this}";
    }
}
public abstract class PublicServiceTile : PurchasableTile
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

    public abstract Sprite GetImageSprite();

}

public class ElectricityTile : PublicServiceTile
{
    
    public ElectricityTile(GameObject tileGameObject, PublicServiceCard publicService,
        CardBehind titleDeedBehindCard)
        : base(tileGameObject, "Compagnie de distribution d'électricité", publicService, titleDeedBehindCard)
    {
    }

    public override Sprite GetImageSprite()
    {
        return ChildUtility.GetChildComponentByName<BaseImageHandler>(tileGameObject.transform.parent, "ElectricityPrefab").GetSprite();
    }
}

public class WaterPumpTile : PublicServiceTile
{
    public WaterPumpTile(GameObject tileGameObject, PublicServiceCard publicService,
        CardBehind titleDeedBehindCard)
        : base(tileGameObject, "Compagnie de distribution des Eaux", publicService, titleDeedBehindCard)
    {
    }

    public override Sprite GetImageSprite()
    {
        return ChildUtility.GetChildComponentByName<BaseImageHandler>(tileGameObject.transform.parent, "WaterPrefab").GetSprite();
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

public interface IPropertyTileStateHolder
{
    
    void SetPropertyTileState(IPropertyTileState propertyTileState);
}
public interface IPropertyTileActionsPossibilityState
{
    public bool CanBuildBeUpgraded();
    public bool CanBuildBeDowngraded();
}
public interface IPropertyTileState:IPropertyTileActionsPossibilityState
{
    void Upgrade(IPropertyTileStateHolder holder);
    void Downgrade(IPropertyTileStateHolder holder);
    int GetHousesNumber();
    int GetHotelNumber();
}
public abstract class PropertyTileStateCanBuildHouse : IPropertyTileState
{
    public bool CanBuildBeUpgraded()
    {
        return true;
    }
    public virtual bool CanBuildBeDowngraded()
    {
        return true;
    }

    public abstract void Upgrade(IPropertyTileStateHolder holder);
    public abstract void Downgrade(IPropertyTileStateHolder holder);
    public abstract int GetHousesNumber();
    public int GetHotelNumber()
    {
        return 0;
    }
}
public class PropertyTileStateWithNoHouse : PropertyTileStateCanBuildHouse
{
    
    public override int GetHousesNumber()
    {
        return 0;
    }
    public virtual bool CanBuildBeDowngraded()
    {
        return false;
    }
    public override void Upgrade(IPropertyTileStateHolder holder)
    {
        holder.SetPropertyTileState(new PropertyTileStateWithOneHouse());
    }

    public override void Downgrade(IPropertyTileStateHolder holder)
    {
        throw new System.Exception("PropertyTileStateWithNoHouse can't be downgraded");
    }
}
public class PropertyTileStateWithOneHouse : PropertyTileStateCanBuildHouse
{
    
    public override int GetHousesNumber()
    {
        return 1;
    }
    public override void Upgrade(IPropertyTileStateHolder holder)
    {
        holder.SetPropertyTileState(new PropertyTileStateWithTwoHouses());
    }
    
    public override void Downgrade(IPropertyTileStateHolder holder)
    {
        holder.SetPropertyTileState(new PropertyTileStateWithNoHouse());
    }
}
public class PropertyTileStateWithTwoHouses : PropertyTileStateCanBuildHouse
{
    
    public override int GetHousesNumber()
    {
        return 2;
    }
    public override void Upgrade(IPropertyTileStateHolder holder)
    {
        holder.SetPropertyTileState(new PropertyTileStateWithThreeHouses());
    }
    
    public override void Downgrade(IPropertyTileStateHolder holder)
    {
        holder.SetPropertyTileState(new PropertyTileStateWithOneHouse());
    }
}
public class PropertyTileStateWithThreeHouses : PropertyTileStateCanBuildHouse
{
    
    public override int GetHousesNumber()
    {
        return 3;
    }
    public override void Upgrade(IPropertyTileStateHolder holder)
    {
        holder.SetPropertyTileState(new PropertyTileStateWithFourHouses());
    }
    public override void Downgrade(IPropertyTileStateHolder holder)
    {
        holder.SetPropertyTileState(new PropertyTileStateWithTwoHouses());
    }
}
public class PropertyTileStateWithFourHouses : PropertyTileStateCanBuildHouse
{
    
    public override int GetHousesNumber()
    {
        return 4;
    }
    public override void Upgrade(IPropertyTileStateHolder holder)
    {
        holder.SetPropertyTileState(new PropertyTileStateWithOneHotel());
    }
    public override void Downgrade(IPropertyTileStateHolder holder)
    {
        holder.SetPropertyTileState(new PropertyTileStateWithThreeHouses());
    }
}

public class PropertyTileStateWithOneHotel : IPropertyTileState
{
    public bool CanBuildBeUpgraded()
    {
        return false;
    }
    public bool CanBuildBeDowngraded()
    {
        return true;
    }

    public void Upgrade(IPropertyTileStateHolder holder)
    {
        throw new System.Exception("PropertyTileStateCanBuildHotel can't be upgraded");
    }
    public void Downgrade(IPropertyTileStateHolder holder)
    {
        holder.SetPropertyTileState(new PropertyTileStateWithFourHouses());
    }

    public int GetHousesNumber()
    {
        return 4;
    }

    public int GetHotelNumber()
    {
        return 1;
    }
}

public class BrownPropertyGroupTile : PropertyTile
{
    public BrownPropertyGroupTile(GameObject tileGameObject, string name, int[] costs, int houseCost, int hotelCost, int price, TitleDeedCard titleDeedFaceCard, CardBehind titleDeedBehindCard) : base(tileGameObject, name, MonopolyColors.GetColor(MonopolyColors.PropertyColor.Brown), costs, houseCost, hotelCost, price, titleDeedFaceCard, titleDeedBehindCard)
    {
    }
    public override PropertyTile[] GetAllGroupOfThisPropertyTile()
    {
        return monopolyGameManager.GetAllGroupOfThisPropertyTile<BrownPropertyGroupTile>();
    }
}

public class LightBluePropertyGroupTile : PropertyTile
{
    public LightBluePropertyGroupTile(GameObject tileGameObject, string name, int[] costs, int houseCost, int hotelCost, int price, TitleDeedCard titleDeedFaceCard, CardBehind titleDeedBehindCard) : base(tileGameObject, name, MonopolyColors.GetColor(MonopolyColors.PropertyColor.LightBlue), costs, houseCost, hotelCost, price, titleDeedFaceCard, titleDeedBehindCard)
    {
    }
    public override PropertyTile[] GetAllGroupOfThisPropertyTile()
    {
        return monopolyGameManager.GetAllGroupOfThisPropertyTile<LightBluePropertyGroupTile>();
    }
}

public class PinkPropertyGroupTile : PropertyTile
{
    public PinkPropertyGroupTile(GameObject tileGameObject, string name, int[] costs, int houseCost, int hotelCost, int price, TitleDeedCard titleDeedFaceCard, CardBehind titleDeedBehindCard) : base(tileGameObject, name, MonopolyColors.GetColor(MonopolyColors.PropertyColor.Pink), costs, houseCost, hotelCost, price, titleDeedFaceCard, titleDeedBehindCard)
    {
    }
    public override PropertyTile[] GetAllGroupOfThisPropertyTile()
    {
        return monopolyGameManager.GetAllGroupOfThisPropertyTile<PinkPropertyGroupTile>();
    }
}

public class OrangePropertyGroupTile : PropertyTile
{
    public OrangePropertyGroupTile(GameObject tileGameObject, string name, int[] costs, int houseCost, int hotelCost, int price, TitleDeedCard titleDeedFaceCard, CardBehind titleDeedBehindCard) : base(tileGameObject, name, MonopolyColors.GetColor(MonopolyColors.PropertyColor.Orange), costs, houseCost, hotelCost, price, titleDeedFaceCard, titleDeedBehindCard)
    {
    }
    public override PropertyTile[] GetAllGroupOfThisPropertyTile()
    {
        return monopolyGameManager.GetAllGroupOfThisPropertyTile<OrangePropertyGroupTile>();
    }
}

public class RedPropertyGroupTile : PropertyTile
{
    public RedPropertyGroupTile(GameObject tileGameObject, string name, int[] costs, int houseCost, int hotelCost, int price, TitleDeedCard titleDeedFaceCard, CardBehind titleDeedBehindCard) : base(tileGameObject, name, MonopolyColors.GetColor(MonopolyColors.PropertyColor.Red), costs, houseCost, hotelCost, price, titleDeedFaceCard, titleDeedBehindCard)
    {
    }
    public override PropertyTile[] GetAllGroupOfThisPropertyTile()
    {
        return monopolyGameManager.GetAllGroupOfThisPropertyTile<RedPropertyGroupTile>();
    }
}
public class YellowPropertyGroupTile : PropertyTile
{
    public YellowPropertyGroupTile(GameObject tileGameObject, string name, int[] costs, int houseCost, int hotelCost, int price, TitleDeedCard titleDeedFaceCard, CardBehind titleDeedBehindCard) : base(tileGameObject, name, MonopolyColors.GetColor(MonopolyColors.PropertyColor.Yellow), costs, houseCost, hotelCost, price, titleDeedFaceCard, titleDeedBehindCard)
    {
    }
    public override PropertyTile[] GetAllGroupOfThisPropertyTile()
    {
        return monopolyGameManager.GetAllGroupOfThisPropertyTile<YellowPropertyGroupTile>();
    }
}

public class GreenPropertyGroupTile : PropertyTile
{
    public GreenPropertyGroupTile(GameObject tileGameObject, string name, int[] costs, int houseCost, int hotelCost, int price, TitleDeedCard titleDeedFaceCard, CardBehind titleDeedBehindCard) : base(tileGameObject, name, MonopolyColors.GetColor(MonopolyColors.PropertyColor.Green), costs, houseCost, hotelCost, price, titleDeedFaceCard, titleDeedBehindCard)
    {
    }
    
    public override PropertyTile[] GetAllGroupOfThisPropertyTile()
    {
        return monopolyGameManager.GetAllGroupOfThisPropertyTile<GreenPropertyGroupTile>();
    }
}

public class DarkBluePropertyGroupTile : PropertyTile
{
    public DarkBluePropertyGroupTile(GameObject tileGameObject, string name,  int[] costs, int houseCost, int hotelCost, int price, TitleDeedCard titleDeedFaceCard, CardBehind titleDeedBehindCard) : base(tileGameObject, name, MonopolyColors.GetColor(MonopolyColors.PropertyColor.DarkBlue), costs, houseCost, hotelCost, price, titleDeedFaceCard, titleDeedBehindCard)
    {
    }

    public override PropertyTile[] GetAllGroupOfThisPropertyTile()
    {
        return monopolyGameManager.GetAllGroupOfThisPropertyTile<DarkBluePropertyGroupTile>();
    }
}
public abstract class PropertyTile : PurchasableTile, IPropertyTileStateHolder, IPropertyTileActionsPossibilityState
{
    public abstract PropertyTile[] GetAllGroupOfThisPropertyTile();
    public IPropertyTileState propertyTileState { get; private set; }
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
        propertyTileState = new PropertyTileStateWithNoHouse();
        owner = null; // No owner initially
    }

    public void SetPropertyTileState(IPropertyTileState propertyTileStateToSet)
    {
        propertyTileState = propertyTileStateToSet;
    }

    public void UpgradeGood()
    {
        propertyTileState.Upgrade(this);
    }
    public void DowngradeGood()
    {
        propertyTileState.Downgrade(this);
    }

    public bool CanBuildBeUpgraded()
    {
        return propertyTileState.CanBuildBeUpgraded();
    }

    public bool CanBuildBeDowngraded()
    {
        return propertyTileState.CanBuildBeDowngraded();
    }
    
    public override IGood GetMinimumGoodToSell()
    {
        switch (propertyTileState)
        {
            case PropertyTileStateWithOneHotel:
                return new PropertyHotelTileGood(this);
            case PropertyTileStateWithOneHouse:
            case PropertyTileStateWithTwoHouses:
            case PropertyTileStateWithThreeHouses:
            case PropertyTileStateWithFourHouses:
                PropertyTile downgradeablePropertyTileSearchingForHotel = GetAllGroupOfThisPropertyTile()
                    .FirstOrDefault(tile => tile.CanBuildBeDowngraded() && tile != this);
                return downgradeablePropertyTileSearchingForHotel==null? new PropertyHouseTileGood(this):downgradeablePropertyTileSearchingForHotel;
            default:
                PropertyTile downgradeablePropertyTileSearchingForHouse = GetAllGroupOfThisPropertyTile()
                    .FirstOrDefault(tile => tile.CanBuildBeDowngraded() && tile != this);
                return downgradeablePropertyTileSearchingForHouse==null ? base.GetMinimumGoodToSell() : null; // null parce que les autres recherches maineront à lui
        }
    }

    public int GetHousesNumber()
    {
        return propertyTileState.GetHousesNumber();
    }

    public int GetHotelNumber()
    {
        return propertyTileState.GetHotelNumber();
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

    public int GetCost()
    {
        return costs[monopolyGameManager.GetAllGroupOfThisPropertyTile<RailroadTile>().ToArray()
            .Count(railRoadTile => railRoadTile.monopolyPlayer == monopolyPlayer)];
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

public abstract class SpecialBoardTile : BoardTile
{
    
    protected SpecialBoardTile(GameObject tileGameObject, string name): base(tileGameObject, name)
    {
    }

}

public class ChanceSpecialTile : SpecialBoardTile
{
    public ChanceSpecialTile(GameObject tileGameObject)
        : base(tileGameObject, "Chance")
    {
        
    }

}
public class CommunitySpecialTile : SpecialBoardTile
{
    public CommunitySpecialTile(GameObject tileGameObject)
        : base(tileGameObject, "Community")
    {
    }

}
public abstract class SpecialCard
{
    
    public MonopolyGameManager monopolyGameManager { get; }
    public string name { get; }
    public string description { get; }

    protected SpecialCard(MonopolyGameManager monopolyGameManager, string name, string description)
    {
        this.monopolyGameManager = monopolyGameManager;
        this.name = name;
        this.description = description;
    }

    public IEnumerator TriggerEffectMain(MonopolyPlayer monopolyPlayer)
    {
        Debug.Log($"Effet de carte : {description}");
        
        monopolyGameManager.SetGameTextEventsText($"{monopolyPlayer} est arriver sur une tuile {name}.");
        yield return new WaitForSeconds(.5f);
        monopolyGameManager.SetGameTextEventsText($"Effet de carte : {description}");
        yield return new WaitForSeconds(1.5f);
        yield return TriggerEffect(monopolyPlayer);
    }
    protected abstract IEnumerator TriggerEffect(MonopolyPlayer monopolyPlayer);
}

public abstract class ChanceCard : SpecialCard
{
    public ChanceCard(MonopolyGameManager monopolyGameManager, string description)
        : base(monopolyGameManager, "Chance", description)
    {
        
    }

}
public abstract class CommunityCard : SpecialCard
{
    public CommunityCard(MonopolyGameManager monopolyGameManager, string description)
        : base(monopolyGameManager, "Community", description)
    {
    }

}
public class ChancesCards : ShuffableCollection<ChanceCard>
{
    
    public ChancesCards(MonopolyGameManager monopolyGameManager)
    {
        
        AddRange(new List<ChanceCard>{
            new AdvanceToUtilityCard(monopolyGameManager),
            new BankDividendCard(monopolyGameManager),
            new AdvanceToStationCardChance(monopolyGameManager),
            new SpeedingFineCard(monopolyGameManager),
            new RepairCostCard(monopolyGameManager),
            new AdvanceToStartCard(monopolyGameManager),
            new AdvanceToStartCard(monopolyGameManager),
            new GetOutOfJailCard(monopolyGameManager),
            new AdvanceToRueDeLaPaixCard(monopolyGameManager),
            new GoToJailCard(monopolyGameManager),
            new AdvanceToAvenueHenriMartinCard(monopolyGameManager),
            new AdvanceToGareMontparnasseCard(monopolyGameManager),
            new RealEstateLoanCard(monopolyGameManager),
            new MoveBackThreeSpacesCard(monopolyGameManager),
            new AdvanceToBoulevardDeLaVilletteCard(monopolyGameManager),
            new ElectedChairmanCard(monopolyGameManager)
        });
        Shuffle();
    }

}

public class CommunitiesCards : ShuffableCollection<CommunityCard>
{
    
    

    public CommunitiesCards(MonopolyGameManager monopolyGameManager)
    {
        
        AddRange(new List<CommunityCard>{
            new PlaygroundDonationCard(monopolyGameManager),
            new NeighborhoodPartyCard(monopolyGameManager),
            new BakeSaleCard(monopolyGameManager),
            new HousingImprovementCard(monopolyGameManager),
            new CharityCarWashCard(monopolyGameManager),
            new BakeSalePurchaseCard(monopolyGameManager),
            new GardenCleanupCard(monopolyGameManager),
            new HospitalPlayCard(monopolyGameManager),
            new PedestrianPathCleanupCard(monopolyGameManager),
            new ChattingWithElderNeighborCard(monopolyGameManager),
            new AnimalShelterDonationCard(monopolyGameManager),
            new BloodDonationCard(monopolyGameManager),
            new MarathonForHospitalCard(monopolyGameManager),
            new HospitalPlayCard(monopolyGameManager),
            new MarathonForHospitalCard(monopolyGameManager),
            new AdoptPuppyCard(monopolyGameManager),
            new LoudMusicCard(monopolyGameManager),
            new HelpNeighborCard(monopolyGameManager),
            // Ajoutez ici d'autres cartes Community...
        });
        Shuffle();
        
    }
}


public enum GameState
{
    WaitingForRoll,
    MovingPiece,
    TurnEnd,
    GameOver
}
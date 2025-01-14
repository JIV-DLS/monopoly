using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Collections;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Serialization;

namespace Monopoly
{
    public class MonopolyGameManager : RequestManager
    {
        public PlayerElementOnMap playerElementOnMapPrefab;
        public PlayerImageChooser playerImageChooser;
        public CommunitiesCards communitiesCards{get;private set;}
        public ChancesCards chancesCards{get;private set;}
        public GameCardBuy gameCardBuy;
        public GameCardBuild gameCardBuild;
        private PlayersHorizontalView _playersHorizontalView;
        private PlayerPieceOnBoardBuilder _playerPieceOnBoardBuilder;
        public List<MonopolyPlayer> monopolyPlayers;
        private MonopolyPlayer localPlayer;
        private MonopolyPlayer currentPlayer; // The player whose turn it is
        public BaseTextHandler GameTextEvents;
        private Board board;
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
        

            
            monopolyPlayers = new List<MonopolyPlayer>();
            
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                monopolyPlayers.Add(new MonopolyPlayer(player, _playersHorizontalView.CreateNewChildAtEnd(),
                    _playerPieceOnBoardBuilder.Create(playerElementOnMapPrefab, playerImageChooser.SpriteAt((int)player.CustomProperties["image"]), transform),
                    GetComponentInChildren<ThrowDices>(),
                    ChildUtility.GetChildComponentByName<FreeFromPrisonButton>(transform,
                        "CommunityFreeFromPrisonButton"),
                    ChildUtility.GetChildComponentByName<FreeFromPrisonButton>(transform, "ChanceFreeFromPrisonButton"),
                    this
                ));
                if (player.IsLocal)
                    currentPlayer = localPlayer;
            }
            // Access all players in the room
            // Photon.Realtime.Player[] players = PhotonNetwork.PlayerList.Where(player=>player.IsLocal).ToArray();

            // Debug.Log($"There are {players.Length} players in the room:");
            

            /*monopolyPlayers.Add(new MonopolyPlayer("Ami",_playersHorizontalView.CreateNewChildAtEnd(),
            _playerPieceOnBoardBuilder.Create(PlayerPieceEnum.BattleShip, transform),
            GetComponentInChildren<ThrowDices>(),
            ChildUtility.GetChildComponentByName<FreeFromPrisonButton>(transform, "CommunityFreeFromPrisonButton"),
            ChildUtility.GetChildComponentByName<FreeFromPrisonButton>(transform, "ChanceFreeFromPrisonButton"),
            this
        ));*/
            chancesCards = new ChancesCards(this);
            communitiesCards = new CommunitiesCards(this);
            // Start the waiting process
            if(PhotonNetwork.IsMasterClient)
                StartCoroutine(GameLoop());
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            base.OnPlayerLeftRoom(otherPlayer);
        }

        public IEnumerator AskAPlayerToPlay(Player player)
        {
            yield return WaitForRequest(player, "AskAPlayerToPlayRpc", player.ActorNumber);
        }
        
        [PunRPC]
        public void AskAPlayerToPlayRpc(int actorNumber)
        {
            if (actorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
                 while(monopolyPlayers.First(monopolyPlayer => monopolyPlayer.player.ActorNumber == actorNumber).TriggerPlay(rollDiceTimeout, buyTileTimeout).MoveNext());
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
            SetGameTextEventsText($"Le joueur {currentPlayer.name} s'est déplacé à {tile.TileName}");
        
            if (doLandAction)
            {
                yield return new WaitForSeconds(.5f);
                if (tile is StartTile || passHome)
                {
                    yield return PassedHome(player);
                }

                if (tile is TaxTile taxTile)
                {
                
                    SetGameTextEventsText($"{currentPlayer.name} doit payer une taxe de {taxTile.taxAmount}M.");
                    yield return new WaitForSeconds(.8f);
                    yield return PlayerMustPayToBank(player, taxTile.taxAmount);
                }
                else if (tile is GoInPrisonTile)
                {
                    yield return PutPlayerIntoPrison(player);
                } else if (tile is SpecialBoardTile specialBoardTile)
                {
                    SetGameTextEventsText($"{currentPlayer.name} est sur la case speciale {specialBoardTile.TileName}");
            
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
            player.HaveWon(StartTile.GetStartReward());
            SetGameTextEventsText($"Le joueur {currentPlayer.name} est passé par la case départ. Il reçoit 200M.");
            return new WaitForSeconds(.5f);
        }

        private IEnumerator GameLoop()
        {
            currentPlayer = monopolyPlayers[0];
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
                        SetGameTextEventsText($"En attente du joueur {currentPlayer.name}");
                        // Call the player's Play method to start their turn
                        yield return currentPlayer.TriggerPlay(rollDiceTimeout, buyTileTimeout);
                        gameState = GameState.TurnEnd;

                        if (!currentPlayer.CanContinuePlaying())
                        {
                        
                            SetGameTextEventsText($"{currentPlayer.name} ne peut plus continuer de jouer.");
                            yield return new WaitForSeconds(.5f);
                            yield return TakeAllFrom(currentPlayer);
                        }
                        // Switch to the next player
                        // currentPlayer = (currentPlayer == player1) ? player2 : player1;
                        break;

                    case GameState.MovingPiece:
                        break;
                    case GameState.GameOver:
                        foreach (var player in monopolyPlayers.Where(player => player.CanContinuePlaying()))
                        {
                            SetGameTextEventsText($"Le joueur {player.name} a remporté la partie.");
                            break;
                        }
                        break;

                    case GameState.TurnEnd:

                        // Optionally wait 2 seconds before the next player's turn starts
                    
                        SetGameTextEventsText($"{currentPlayer.name} a finit son tour");
                        yield return new WaitForSeconds(1.5f);
                        AdvanceToNextPlayer();
                        if (!currentPlayer.CanContinuePlaying())
                        {
                            gameState = GameState.TurnEnd;
                        }
                        break;
                }

                var remainingPlayers = monopolyPlayers.Where(player => player.CanContinuePlaying()).ToList();

                if (remainingPlayers.Count < 2)
                {
                    if (remainingPlayers.Count > 0)
                    {
                        var winner = remainingPlayers.First();
                        SetGameTextEventsText($"Le joueur {winner.name} a remporté la partie.");
                    }
                    else
                    {
                        SetGameTextEventsText($"Aucun joueur n'a gagné. Ils ont tous perdus.");
                    } 
                    yield return new WaitForSeconds(1.5f);
                    // break;

                }

                // Optionally add a delay between turns
                yield return null; // 1 second delay before next turn
            }
        }

        private IEnumerator TakeAllFrom(MonopolyPlayer monopolyPlayer)
        {
            // Create a copy of the tiles to avoid modifying the collection while iterating
            List<PurchasableTile> tilesToRemove = monopolyPlayer.deck.GetAllTiles().ToList();

            foreach (PurchasableTile tile in tilesToRemove)
            {
                tile.RemoveOwner();
                monopolyPlayer.deck.RemoveByGroupAtIndex(tile.GetTargetType(), tile.groupIndex);
            
                SetGameTextEventsText($"{tile.TileName} est de nouveau disponible sur le marché.");
                yield return new WaitForSeconds(.5f);
            }

            yield return null;
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

        public IEnumerator APlayerRolledDice(MonopolyPlayer player, int rollResult)
        {
            /*yield return MoveAPlayerToATile(player, board.GetTileAtIndex(board.MoveFromTile(player.tile, rollResult, out var passHome)),
            passHome, true);*/
            yield return MoveAPlayerFromTileByJumping(player, rollResult);
            if (player.currentTile.GetTileIndex()!=0)
                yield return MoveAPlayerToATile(player, player.currentTile);
            yield return null;
        }

        public IEnumerator APlayerMoveToAnIndex(MonopolyPlayer player, int targetIndex)
        {
            yield return MoveAPlayerFromTileByJumpingToAnIndex(player, targetIndex);
            if (player.currentTile.GetTileIndex()!=0)
                yield return MoveAPlayerToATile(player, player.currentTile);
            yield return null;
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

        public void DiceThrownResult(int roll)
        {
            //MovePlayer(players[currentPlayerIndex], roll);
        }

        private void AdvanceToNextPlayer()
        {
            currentPlayerIndex = (currentPlayerIndex + 1) % monopolyPlayers.Count;
            currentPlayer = monopolyPlayers[currentPlayerIndex];
            gameState = GameState.WaitingForRoll;
        }

        public IEnumerator MoveAPlayerToNextType<T>(MonopolyPlayer monopolyPlayer)
        {
            IEnumerator<(int tileIndex, bool passHome)> moveEnumerator = board.MoveFromTileToNextType<T>(monopolyPlayer.currentTile);

            // Variable to keep track of the last result
            (int lastTileIndex, bool lastPassHome) = (-1, false);
        
            while (moveEnumerator.MoveNext())
            {
                // Deconstruct the current value
                // Update the last result with the current one
                lastTileIndex = moveEnumerator.Current.tileIndex;
                lastPassHome = moveEnumerator.Current.passHome;

                // Perform actions with the current tile index and passHome flag
                // Debug.Log($"Moved to Tile: {lastTileIndex}, Passed Home: {lastPassHome}");

                // Add logic to update your player position or animations here

                yield return MoveAPlayerToATile(monopolyPlayer, board.GetTileAtIndex(lastTileIndex), false, false);
                // Simulate a delay for each step
                yield return new WaitForSeconds(0.09f);
            }
        }
        public IEnumerator MoveAPlayerFromTileByJumping(MonopolyPlayer monopolyPlayer, int tileIndex)
        {
            IEnumerator<(int tileIndex, bool passHome)> moveEnumerator = board.MoveAPlayerFromTileByJumping(monopolyPlayer.currentTile, tileIndex);

            // Variable to keep track of the last result
            (int lastTileIndex, bool lastPassHome) = (-1, false);
        
            while (moveEnumerator.MoveNext())
            {
                // Deconstruct the current value
                // Update the last result with the current one
                if (lastTileIndex == moveEnumerator.Current.tileIndex)
                {
                    yield break;
                }
                lastTileIndex = moveEnumerator.Current.tileIndex;
                lastPassHome = moveEnumerator.Current.passHome;

                // Perform actions with the current tile index and passHome flag
                //Debug.Log($"Moved to Tile: {lastTileIndex}, Passed Home: {lastPassHome}");

                // Add logic to update your player position or animations here

                yield return MoveAPlayerToATile(monopolyPlayer, board.GetTileAtIndex(lastTileIndex), false, false);
                // Simulate a delay for each step
                yield return new WaitForSeconds(0.09f);
            }
        }
        public IEnumerator MoveAPlayerFromTileByJumpingToAnIndex(MonopolyPlayer monopolyPlayer, int tileIndex)
        {
            do
            {
                // Perform actions with the current tile index and passHome flag
                //Debug.Log($"Moved to Tile: {lastTileIndex}, Passed Home: {lastPassHome}");

                // Add logic to update your player position or animations here

                yield return MoveAPlayerToATile(monopolyPlayer, board.GetTileAtIndex(monopolyPlayer.currentTile.GetTileIndex()+1), false, false);
                // Simulate a delay for each step
                yield return new WaitForSeconds(0.09f);
            } while (monopolyPlayer.currentTile.GetTileIndex() != tileIndex);
        }
        public IEnumerator MoveAPlayerToTileIndex(MonopolyPlayer monopolyPlayer, int tileIndex)
        {
            IEnumerator<(int tileIndex, bool passHome)> moveEnumerator = board.MoveAPlayerToTileIndex(monopolyPlayer.currentTile, tileIndex);

            // Variable to keep track of the last result
            (int lastTileIndex, bool lastPassHome) = (-1, false);
        
            while (moveEnumerator.MoveNext())
            {
                // Deconstruct the current value
                // Update the last result with the current one
                lastTileIndex = moveEnumerator.Current.tileIndex;
                lastPassHome = moveEnumerator.Current.passHome;

                // Perform actions with the current tile index and passHome flag
                //Debug.Log($"Moved to Tile: {lastTileIndex}, Passed Home: {lastPassHome}");

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
            if (payer.CanContinuePlaying() && payer.CanBeChargedOf(dueAmount))
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

        public void BuyPurchasableTileBy(MonopolyPlayer monopolyPlayer, PurchasableTile purchasableTile)
        {
            Debug.Assert(purchasableTile != null, nameof(purchasableTile) + " != null");
            Debug.Assert(monopolyPlayer != null, nameof(monopolyPlayer) + " != null");
            Debug.Assert(monopolyPlayer.CanBeChargedOf(purchasableTile.getPrice()));
            monopolyPlayer.ChargedOf(purchasableTile.getPrice());
            purchasableTile.AssignOwner(monopolyPlayer);
            SetGameTextEventsText($"{monopolyPlayer.name} bought a purchasable {purchasableTile.TileName}.");
        }

        public PurchasableTile[] GetAllGroupOfThisPropertyTile(Type getTargetType)
        {
            return board.GetAllGroupOfThisPropertyTile(getTargetType);
        }
        public int GetAllGroupOfThisPropertyTileIsOwnedByTheSamePlayer(PurchasableTile purchasableTile)
        {
            return purchasableTile.IsOwned()? board.GetAllGroupOfThisPropertyTile(purchasableTile.GetTargetType()).Count(
                _purchasableTile=>_purchasableTile.IsOwnedBy(purchasableTile.GetOwner())): 0;
        }
        public bool DoesAllGroupOfThisPropertyTileIsOwnedByTheSamePlayer(PurchasableTile pourchasableTile)
        {
            return pourchasableTile.IsOwned()? board.GetAllGroupOfThisPropertyTile(pourchasableTile.GetTargetType()).All(
                _pourchasableTile=>_pourchasableTile.IsOwnedBy(pourchasableTile.GetOwner())): false;
        }

        public void BuildOnPropertyTile(PropertyTile propertyTile, MonopolyPlayer monopolyPlayer)
        {
            Debug.Assert(monopolyPlayer.CanBeChargedOf(propertyTile.GetUpgradePrice()), $"{monopolyPlayer} can not be charged of {propertyTile.GetUpgradePrice()}. It only have {monopolyPlayer.money}.");
            monopolyPlayer.ChargedOf(propertyTile.GetUpgradePrice());
            propertyTile.BuildTileGood();
        }
        public void BroadCastDiceState(Vector3 positions1, Quaternion rotations1, Vector3 positions2, Quaternion rotations2)
        {
            
            // Call the RPC method
            photonView.RPC("BroadCastDiceStateRpc", RpcTarget.Others, positions1, rotations1, positions2, rotations2);
        }
        [PunRPC]
        public void BroadCastDiceStateRpc(Vector3 positions1, Quaternion rotations1, Vector3 positions2, Quaternion rotations2)
        {
            dicesManager.UpdateDicesTransform(positions1, rotations1, positions2, rotations2);
        }
    }
}
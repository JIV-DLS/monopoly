using System;
using System.Collections.Generic;
using System.Linq;
using Monopoly;
using UnityEngine;

public class Board
{
    private BoardTile[] tiles;
    private Dictionary<int, BoardTile> tileLookup;
    private Dictionary<BoardTile, int> indexLookup;
    public TitleDeedCard titleDeedCardPrefab { get; }
    public RailRoadCard railRoadCardPrefab { get; }
    public PublicServiceCard publicServiceCardPrefab { get; }
    public CardBehind cardBehindPrefab { get; }
    
    public PurchasableTile[] GetAllGroupOfThisPropertyTile(Type targetType)
    {
        if (!typeof(PurchasableTile).IsAssignableFrom(targetType))
        {
            throw new ArgumentException($"Type {targetType} must inherit from PurchasableTile", nameof(targetType));
        }

        // Use reflection to invoke the method dynamically
        var method = typeof(Board).GetMethod("GetTilesOfType")?.MakeGenericMethod(targetType);
        if (method != null)
        {
            return (PurchasableTile[])method.Invoke(this, null);
        }
        else
        {
            throw new ArgumentException("GetTilesOfType not found");
        }
        
    }
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
                    boardTile = new StartTile(side[i], index);
                    break;
                case 1:
                    boardTile = new BrownPropertyGroupTile(side[i], "Boulevard de Belleville",
                        new int[] { 2, 4, 10, 30, 90, 160, 250 },
                        50, 50, 60, titleDeedCardPrefab, cardBehindPrefab, index, 0);
                    break;
                case 2:
                    boardTile = new CommunitySpecialTile(side[i], index, 0);
                    break;
                case 3:
                    boardTile = new BrownPropertyGroupTile(side[i], "Rue Lecourbe",
                        new int[] { 4, 8, 20, 60, 180, 320, 450 },
                        50, 50, 60, titleDeedCardPrefab, cardBehindPrefab, index, 1);
                    break;
                case 4:
                    boardTile = new TaxTile(side[i], "Impôts sur le revenu", 200, index, 0);
                    break;
                case 5:
                    boardTile = new RailroadTile(side[i], "Gare Mont-Parnasse", railRoadCardPrefab, cardBehindPrefab, index, 0);
                    break;
                case 6:
                    boardTile = new LightBluePropertyGroupTile(side[i], "Rue De Vaugirard",
                        new int[] { 6, 12, 30, 90, 270, 400, 550 },
                        50, 50, 100, titleDeedCardPrefab, cardBehindPrefab, index, 0);
                    break;
                case 7:
                    boardTile = new ChanceSpecialTile(side[i], index, 0);
                    break;
                case 8:
                    boardTile = new LightBluePropertyGroupTile(side[i], "Rue De Courcelles",
                        new int[] { 6, 12, 30, 90, 270, 400, 550 },
                        50, 50, 100, titleDeedCardPrefab, cardBehindPrefab, index, 0);
                    break;
                case 9:
                    boardTile = new LightBluePropertyGroupTile(side[i], "Avenue de la Republique",
                        new int[] { 8, 16, 40, 100, 300, 450, 600 },
                        50, 50, 120, titleDeedCardPrefab, cardBehindPrefab, index, 1);
                    break;
                case 10:
                    boardTile = new PrisonOrVisitTile(side[i], index);
                    break;
                case 11:
                    boardTile = new PinkPropertyGroupTile(side[i], "Boulevard de Villette",
                        new int[] { 10, 20, 50, 150, 450, 625, 750 },
                        100, 100, 140, titleDeedCardPrefab, cardBehindPrefab, index, 0);
                    break;
                case 12:
                    boardTile = new ElectricityTile(side[i], publicServiceCardPrefab, cardBehindPrefab, index, 0);
                    break;
                case 13:
                    boardTile = new PinkPropertyGroupTile(side[i], "Avenue de Neuilly",
                        new int[] { 10, 20, 50, 150, 450, 625, 750 },
                        100, 100, 140, titleDeedCardPrefab, cardBehindPrefab, index, 1);
                    break;
                case 14:
                    boardTile = new PinkPropertyGroupTile(side[i], "Rue de Paradis",
                        new int[] { 12, 24, 60, 180, 500, 700, 900 },
                        100, 100, 160, titleDeedCardPrefab, cardBehindPrefab, index, 2);
                    break;
                case 15:
                    boardTile = new RailroadTile(side[i], "Gare de Lyon", railRoadCardPrefab, cardBehindPrefab, index, 1);
                    break;
                case 16:
                    boardTile = new OrangePropertyGroupTile(side[i], "Avenue Mozart",
                        new int[] { 14, 28, 70, 200, 550, 750, 950 },
                        100, 100, 180, titleDeedCardPrefab, cardBehindPrefab, index, 0);
                    break;
                case 17:
                    boardTile = new CommunitySpecialTile(side[i], index, 1);
                    break;
                case 18:
                    boardTile = new OrangePropertyGroupTile(side[i], "Boulevard Saint-Michel",
                        new int[] { 14, 28, 70, 200, 550, 750, 950 },
                        100, 100, 180, titleDeedCardPrefab, cardBehindPrefab, index, 1);
                    break;
                case 19:
                    boardTile = new OrangePropertyGroupTile(side[i], "Place Pigalle",
                        new int[] { 16, 32, 80, 220, 600, 800, 1000 },
                        100, 100, 200, titleDeedCardPrefab, cardBehindPrefab, index, 2);
                    break;
                case 20:
                    boardTile = new FreeParcTile(side[i], index);
                    break;
                case 21:
                    boardTile = new RedPropertyGroupTile(side[i], "Avenue Matignon",
                        new int[] { 18, 36, 90, 250, 700, 875, 1050 },
                        150, 150, 220, titleDeedCardPrefab, cardBehindPrefab, index, 0);
                    break;
                case 22:
                    boardTile = new ChanceSpecialTile(side[i], index, 1);
                    break;
                case 23:
                    boardTile = new RedPropertyGroupTile(side[i], "Boulevard Malesherbes",
                        new int[] { 18, 36, 90, 250, 700, 875, 1050 },
                        150, 150, 220, titleDeedCardPrefab, cardBehindPrefab, index, 1);
                    break;
                case 24:
                    boardTile = new RedPropertyGroupTile(side[i], "Avenue Henri-Martin",
                        new int[] { 20, 40, 100, 300, 750, 925, 1100 },
                        150, 150, 240, titleDeedCardPrefab, cardBehindPrefab, index, 2);
                    break;
                case 25:
                    boardTile = new RailroadTile(side[i], "Gare du Nord", railRoadCardPrefab, cardBehindPrefab, index, 2);
                    break;
                case 26:
                    boardTile = new YellowPropertyGroupTile(side[i], "Faubourg Saint-Honoré",
                        new int[] { 22, 44, 110, 330, 800, 975, 1150 },
                        150, 150, 260, titleDeedCardPrefab, cardBehindPrefab, index, 0);
                    break;
                case 27:
                    boardTile = new YellowPropertyGroupTile(side[i], "Place de la Bourse",
                        new int[] { 22, 44, 110, 330, 800, 975, 1150 },
                        150, 150, 260, titleDeedCardPrefab, cardBehindPrefab, index, 1);
                    break;
                case 28:
                    boardTile = new WaterPumpTile(side[i], publicServiceCardPrefab, cardBehindPrefab, index, 1);
                    break;
                case 29:
                    boardTile = new YellowPropertyGroupTile(side[i], "Rue de la Fayette",
                        new int[] { 24, 48, 120, 360, 850, 1025, 1200 },
                        150, 150, 280, titleDeedCardPrefab, cardBehindPrefab, index, 2);
                    break;
                case 30:
                    boardTile = new GoInPrisonTile(side[i], index);
                    break;
                case 31:
                    boardTile = new GreenPropertyGroupTile(side[i], "Avenue de Breteuil",
                        new int[] { 26, 52, 130, 390, 900, 1100, 1275 },
                        200, 200, 300, titleDeedCardPrefab, cardBehindPrefab, index, 0);
                    break;
                case 32:
                    boardTile = new GreenPropertyGroupTile(side[i], "Avenue Foch",
                        new int[] { 26, 52, 130, 390, 900, 1100, 1275 },
                        200, 200, 300, titleDeedCardPrefab, cardBehindPrefab, index, 1);
                    break;
                case 33:
                    boardTile = new CommunitySpecialTile(side[i], index, 2);
                    break;
                case 34:
                    boardTile = new GreenPropertyGroupTile(side[i], "Boulvard des Capucines",
                        new int[] { 28, 56, 150, 450, 1000, 1200, 1400 },
                        200, 200, 320, titleDeedCardPrefab, cardBehindPrefab, index, 2);
                    break;
                case 35:
                    boardTile = new RailroadTile(side[i], "Gare Saint Lazare", railRoadCardPrefab, cardBehindPrefab, index, 3);
                    break;
                case 36:
                    boardTile = new ChanceSpecialTile(side[i], index, 2);
                    break;
                case 37:
                    
                    boardTile = new DarkBluePropertyGroupTile(side[i], "Avenue des Champs-Elysées",
                        new int[] { 35, 70, 175, 500, 1100, 1300, 1500 },
                        200, 200, 350, titleDeedCardPrefab, cardBehindPrefab, index, 0);
                    break;
                case 38:

                    boardTile = new TaxTile(side[i], "Taxe de Luxe", 100, index, 1);
                    break;
                case 39:
                    
                    boardTile = new DarkBluePropertyGroupTile(side[i], "Rue de la Paix",
                        new int[] { 50, 100, 200, 600, 1400, 1700, 2000 },
                        200, 200, 400, titleDeedCardPrefab, cardBehindPrefab, index, 1);
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
        return tileLookup.TryGetValue(index % tiles.Length, out var tile) ? tile : null;
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
        int index = GetTileIndex(monopolyPlayer.currentTile)-i;
        if (index < 0)
        {
            index = (index + tiles.Length) % tiles.Length;
        }

        return index;
    }
    public IEnumerator<(int tileIndex, bool passHome)> MoveAPlayerFromTileByJumping(BoardTile monopolyPlayerTile, int jumps)
    {
        int currentIndex = GetTileIndex(monopolyPlayerTile);
        if (currentIndex == -1)
        {
            throw new ArgumentException("The provided tile is not part of the board.");
        }

        bool passHome = false;

        for (int i = 0; i < jumps; i++)
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
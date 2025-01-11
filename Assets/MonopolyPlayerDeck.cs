using System;
using System.Collections.Generic;
using System.Linq;

public class MonopolyPlayerDeck
{
    private readonly Dictionary<Type, Dictionary<int, PurchasableTile>> _purchasableTiles = new Dictionary<Type, Dictionary<int, PurchasableTile>>();

    // Add any object of type T where T inherits from PurchasableTile
    public void Add<T>(T value) where T : PurchasableTile
    {
        var targetType = value.GetTargetType();
        var groupIndex = value.groupIndex;

        if (!_purchasableTiles.ContainsKey(targetType))
        {
            _purchasableTiles[targetType] = new Dictionary<int, PurchasableTile>();
        }

        if (_purchasableTiles[targetType].ContainsKey(groupIndex))
        {
            Console.WriteLine($"A {typeof(T).Name} with group index {groupIndex} already exists in the {targetType.Name} group.");
            return;
        }

        _purchasableTiles[targetType][groupIndex] = value;
        Console.WriteLine($"Added {typeof(T).Name} with group index {groupIndex} to target type {targetType.Name}.");
    }

    public bool RemoveByGroupAtIndex(Type targetType, int groupIndex)
    {
        if (!typeof(PurchasableTile).IsAssignableFrom(targetType))
        {
            throw new ArgumentException($"Type {targetType} must inherit from PurchasableTile", nameof(targetType));
        }

        // Use reflection to invoke the method dynamically
        var method = typeof(MonopolyPlayerDeck).GetMethod("Remove")?.MakeGenericMethod(targetType);
        if (method != null)
        {
            return (bool)method.Invoke(this, new Object[] { groupIndex });
        }
        else
        {
            throw new ArgumentException("Remove not found");
        }
        
    }
    // Remove an object by group index
    public bool Remove<T>(int groupIndex) where T : PurchasableTile
    {
        var targetType = typeof(T);

        if (_purchasableTiles.ContainsKey(targetType) && _purchasableTiles[targetType].ContainsKey(groupIndex))
        {
            _purchasableTiles[targetType].Remove(groupIndex);
            Console.WriteLine($"Removed {typeof(T).Name} with group index {groupIndex} from {targetType.Name} group.");
            return true;
        }

        return false;
    }

    // Get an object by group index
    public T Get<T>(int groupIndex) where T : PurchasableTile
    {
        var targetType = typeof(T);

        if (_purchasableTiles.ContainsKey(targetType) && _purchasableTiles[targetType].ContainsKey(groupIndex))
        {
            return (T)_purchasableTiles[targetType][groupIndex];
        }

        return null;
    }

    // Display all entries grouped by type
    public void Display()
    {
        foreach (var group in _purchasableTiles)
        {
            Console.WriteLine($"Type: {group.Key.Name}");
            foreach (var item in group.Value)
            {
                Console.WriteLine($"    Group Index: {item.Key}, Tile: {item.Value}");
            }
        }
    }

    // Get all objects of a specific type
    public IEnumerable<T> GetAllOfType<T>() where T : PurchasableTile
    {
        var targetType = typeof(T);

        // Collect all tiles where the type matches or is assignable to T
        return _purchasableTiles
            .Where(pair => targetType.IsAssignableFrom(pair.Key)) // Check if the type (or child type) matches
            .SelectMany(pair => pair.Value.Values) // Flatten the nested dictionary
            .OfType<T>(); // Ensure the result is cast to T
    }

    // Get the count of objects of a specific type
    public int GetCountOfType<T>() where T : PurchasableTile
    {
        var targetType = typeof(T);

        return _purchasableTiles.ContainsKey(targetType) ? _purchasableTiles[targetType].Count : 0;
    }

    // Check if a specific type has at least one non-mortgaged card
    public bool HaveAtLeastOneNotMortgagedCard()
    {
        return GetAllTiles().Any(tile => !tile.IsMortgaged);
    }

    // Get the smallest good to sell across all groups
    public IGood GetSmallestGoodToSell()
    {
        return _purchasableTiles.Values
            .SelectMany(group => group.Values)
            .Select(tile => tile.GetMinimumGoodToSell())
            .Where(good => good != null)
            .OrderBy(good => good.GetSellPrice())
            .FirstOrDefault();
    }

    // Access the entire collection (read-only)
    public IReadOnlyDictionary<Type, Dictionary<int, PurchasableTile>> GetCollection()
    {
        return _purchasableTiles;
    }
    public IEnumerable<PurchasableTile> GetAllGroupOfThisPropertyTile(Type targetType)
    {
        if (!typeof(PurchasableTile).IsAssignableFrom(targetType))
        {
            throw new ArgumentException($"Type {targetType} must inherit from PurchasableTile", nameof(targetType));
        }

        // Use reflection to invoke the method dynamically
        var method = typeof(MonopolyPlayerDeck).GetMethod("GetAllOfType")?.MakeGenericMethod(targetType);
        if (method != null)
        {
            return (IEnumerable<PurchasableTile>)method.Invoke(this, null);
        }
        else
        {
            throw new ArgumentException("GetAllOfType not found");
        }
        
    }
    // Get an array of all PurchasableTile objects sorted by their TileIndex
    public PurchasableTile[] GetAllTilesSortedByTileIndex()
    {
        return GetAllTiles() // Flatten all tiles into a single collection
            .OrderBy(tile => tile.GetTileIndex()) // Sort by TileIndex
            .ToArray(); // Convert to an array
    }

    public IEnumerable<PurchasableTile> GetAllTiles()
    {
        return _purchasableTiles.Values
            .SelectMany(group => group.Values);
    }
}
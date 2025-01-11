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
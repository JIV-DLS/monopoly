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
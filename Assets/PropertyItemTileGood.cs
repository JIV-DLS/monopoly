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
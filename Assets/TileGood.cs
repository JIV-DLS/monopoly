public abstract class TileGood : IGood
{
    protected PurchasableTile purchasableTile{ get; }
    public string goodName{ get; }

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

    public bool CanBeSelled()
    {
        return purchasableTile.CanCurrentTileGoodBeSelled();
    }

    public PurchasableTile GetPurchasableTile()
    {
        return purchasableTile;
    }

    public string GetSellText()
    {
        return $"a vendu 1 {goodName} de {purchasableTile.TileName}";
    }

    public string GetGoodName()
    {
        return goodName;
    }
}
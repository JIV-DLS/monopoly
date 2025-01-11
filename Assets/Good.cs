public abstract class Good: IGood
{
    // You can leave this method abstract or provide a default implementation.
    public abstract int GetSellPrice();
    public abstract int Sell();
    public abstract bool CanBeSelled();
    public abstract PurchasableTile GetPurchasableTile();
    public abstract string GetSellText();

    public abstract string GetGoodName();
}
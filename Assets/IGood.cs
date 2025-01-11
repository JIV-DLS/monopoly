public interface IGood
{
    public int GetSellPrice();
    public int Sell();
    public bool CanBeSelled();
    public PurchasableTile GetPurchasableTile();
    string GetSellText();
}
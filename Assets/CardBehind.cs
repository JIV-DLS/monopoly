using UnityEngine;

public class CardBehind : PurchasableBehindCard
{
    private BaseTextHandler _backCardTitleBaseTextHandler;
    private BaseTextHandler _mortgageValueBaseTextHandler;
    private BaseTextHandler _mortgageFinishedValueBaseTextHandler;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Init()
    {
        base.Init();
        _backCardTitleBaseTextHandler = ChildUtility.GetChildComponentByName<BaseTextHandler>(transform, "BackCardTitle");

        if (_backCardTitleBaseTextHandler == null)
        {
            Debug.LogError("Back card title value not found");
        }
        _mortgageValueBaseTextHandler = ChildUtility.GetChildComponentByName<BaseTextHandler>(transform, "MortgageValue");
        if (_mortgageValueBaseTextHandler == null)
        {
            Debug.LogError("Mortgage value not found");
        }
        _mortgageFinishedValueBaseTextHandler = ChildUtility.GetChildComponentByName<BaseTextHandler>(transform, "MortgageFinishedValue");
        if (_mortgageFinishedValueBaseTextHandler == null)
        {
            Debug.LogError("mortgage finished value not found");
        }
        
    }

    public CardBehind Clone (PurchasableTile purchasableTile)
    {
        return Clone().UpdateTile(purchasableTile);
    }
    public CardBehind Clone ()
    {
        return base.Clone<CardBehind>();
    }
    public CardBehind UpdateTile(PurchasableTile purchasableTile)
    {
        base.UpdateTile(purchasableTile);
        _backCardTitleBaseTextHandler.SetText(purchasableTile.TileName.ToUpper());
        _mortgageValueBaseTextHandler.SetText($"HYPOTHÉQUÉE POUR {purchasableTile.mortgageCost}M");
        _mortgageFinishedValueBaseTextHandler.SetText($"POUR LEVER L'HYPOTHÈQUE, PAYER {purchasableTile.mortgageFinishedCost}M");
        return this;
    }
    
}

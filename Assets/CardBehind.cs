using UnityEngine;

public class CardBehind : MonoBehaviour
{
    private BaseTextHandler _backCardTitleBaseTextHandler;
    private BaseTextHandler _mortgageValueBaseTextHandler;
    private BaseTextHandler _mortgageFinishedValueBaseTextHandler;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
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

    public void UpdateTile(PurchasableTile puchasableTile)
    {
        _backCardTitleBaseTextHandler.SetText(puchasableTile.TileName);
        _mortgageValueBaseTextHandler.SetText($"HYPOTHÉQUÉ POUR {puchasableTile.mortgageCost}M");
        _mortgageFinishedValueBaseTextHandler.SetText($"POUR LEVER L'HYPOTHÈQUE, PAYER {puchasableTile.mortgageFinishedCost}M");
    }
    
}

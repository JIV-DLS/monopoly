using UnityEngine;

public abstract class PurchasableActionButton : IClickableButtonHandler
{
    private PurchasableTile _purchasableTile;
    private GameCardBuyActions _buyActions;

    protected PurchasableActionButton(GameCardBuyActions buyActions, PurchasableTile purchasableTile)
    {
        _purchasableTile = purchasableTile;
        _buyActions = buyActions;
    }

    protected abstract void OnClickAction();

    public void OnClick()
    {
        OnClickAction();
        _buyActions.Hide();
    }
}

public class BuyActionButton : PurchasableActionButton
{
    public BuyActionButton(GameCardBuyActions buyActions, PurchasableTile purchasableTile) : base(buyActions, purchasableTile)
    {
    }

    protected override void OnClickAction()
    {
        throw new System.NotImplementedException();
    }
}
public class CancelActionButton : PurchasableActionButton
{
    public CancelActionButton(GameCardBuyActions buyActions, PurchasableTile purchasableTile) : base(buyActions, purchasableTile)
    {
    }

    protected override void OnClickAction()
    {
        throw new System.NotImplementedException();
    }
}

public class GameCardBuyActions : MonoBehaviour
{
    private PurchasableTile _purchasableTile;

    public ButtonHandler cancelButton;
    public ButtonHandler buyButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ShowPurchasableTile(PurchasableTile purchasableTile)
    {
        _purchasableTile = purchasableTile;
        buyButton.Handler = new BuyActionButton(this, purchasableTile);
        cancelButton.Handler = new CancelActionButton(this, purchasableTile);
    }

    public void Hide()
    {
        
    }
}

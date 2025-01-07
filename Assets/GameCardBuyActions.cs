using UnityEngine;

public abstract class PurchasableActionButton : IClickableButtonHandler
{
    protected PurchasableTile _purchasableTile;
    protected GameCardBuyActions _buyActions;
    protected MonopolyPlayer _monopolyPlayer;

    protected PurchasableActionButton(GameCardBuyActions buyActions, PurchasableTile purchasableTile, MonopolyPlayer monopolyPlayer)
    {
        _purchasableTile = purchasableTile;
        _buyActions = buyActions;
        _monopolyPlayer = monopolyPlayer;
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
    public BuyActionButton(GameCardBuyActions buyActions, PurchasableTile purchasableTile, MonopolyPlayer monopolyPlayer) : base(buyActions, purchasableTile, monopolyPlayer)
    {
    }

    protected override void OnClickAction()
    {
        _monopolyPlayer.Buy(_purchasableTile);
    }
}
public class CancelActionButton : PurchasableActionButton
{
    public CancelActionButton(GameCardBuyActions buyActions, PurchasableTile purchasableTile, MonopolyPlayer monopolyPlayer) : base(buyActions, purchasableTile, monopolyPlayer)
    {
    }

    protected override void OnClickAction()
    {
    }
}

public class GameCardBuyActions : MonoBehaviour
{
    private PurchasableTile _purchasableTile;
    public GameCardBuy gameCardBuy;
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
    public void ShowPurchasableTile(PurchasableTile purchasableTile, MonopolyPlayer monopolyPlayer)
    {
        _purchasableTile = purchasableTile;
        buyButton.Handler = new BuyActionButton(this, purchasableTile, monopolyPlayer);
        cancelButton.Handler = new CancelActionButton(this, purchasableTile, monopolyPlayer);
        buyButton.SetButtonText($"ACHETER {purchasableTile.getPrice()}M");
    }

    public void Hide()
    {
        gameCardBuy.Hide();
    }
}

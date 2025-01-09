using System;
using UnityEngine;
using UnityEngine.Serialization;
public interface IAction
{
    void Execute();
    void Execute(MonopolyPlayer player, PurchasableTile tile);
    void Cancel();
}
public class ActionButtonHandler : IClickableButtonHandler
{
    private readonly IAction _action;

    public ActionButtonHandler(IAction action)
    {
        _action = action ?? throw new ArgumentNullException(nameof(action));
    }

    public void OnClick()
    {
        _action.Execute();
    }
}
public interface IGameCardActions
{
    void ShowActions(PurchasableTile purchasableTile, MonopolyPlayer monopolyPlayer);
    void Hide();
}
public abstract class PurchasableActionButton : IClickableButtonHandler
{
    private readonly IAction _action;
    private readonly MonopolyPlayer _player;
    private readonly PurchasableTile _tile;

    protected PurchasableActionButton(IAction action, MonopolyPlayer player, PurchasableTile tile)
    {
        _action = action;
        _player = player;
        _tile = tile;
    }

    protected abstract void OnClickAction();

    public void OnClick()
    {
        Debug.Log("OnClick");
        _action.Execute(_player, _tile);
        OnClickAction();
    }
}

public class BuyAction : IAction
{
    private readonly MonopolyPlayer _player;
    private readonly PurchasableTile _tile;

    public BuyAction(MonopolyPlayer player, PurchasableTile tile)
    {
        _player = player ?? throw new ArgumentNullException(nameof(player));
        _tile = tile ?? throw new ArgumentNullException(nameof(tile));
    }

    public void Execute()
    {
        Execute(_player, _tile);
    }

    public void Execute(MonopolyPlayer player = null, PurchasableTile tile = null)
    {
        _player.Buy(_tile);
    }

    public void Cancel()
    {
        Debug.Log("Buy action canceled.");
    }
}

public class CancelAction : IAction
{
    public void Execute()
    {
        Execute(null, null);
    }

    public void Execute(MonopolyPlayer player, PurchasableTile tile)
    {
        Debug.Log("Cancel action executed.");
    }

    public void Cancel()
    {
        Debug.Log("Cancel action canceled.");
    }
}
public class PurchasableButton : PurchasableActionButton
{
    public PurchasableButton(IAction action, MonopolyPlayer player, PurchasableTile tile)
        : base(action, player, tile)
    {
    }

    protected override void OnClickAction()
    {
        Debug.Log("Additional logic for PurchasableButton.");
    }
}
public abstract class GameCardActionsBase : MonoBehaviour
{
    protected PurchasableTile _purchasableTile;
    public GameCardLayout gameCardLayout;
    public ButtonHandler primaryButton; // Shared button for main action (buy/build)
    public ButtonHandler cancelButton;

    public void ShowActions(PurchasableTile purchasableTile, MonopolyPlayer monopolyPlayer)
    {
        _purchasableTile = purchasableTile;

        // Assign handlers dynamically using the wrapper
        primaryButton.Handler = new ActionButtonHandler(CreatePrimaryAction(monopolyPlayer, purchasableTile));
        cancelButton.Handler = new ActionButtonHandler(CreateCancelAction());

        // Set button text dynamically
        primaryButton.SetButtonText(GetPrimaryButtonText(purchasableTile));
    }

    public void Hide()
    {
        gameCardLayout.Hide();
    }

    public void ShowPurchasableTile(PurchasableTile purchasableTile, MonopolyPlayer monopolyPlayer)
    {
        throw new NotImplementedException();
    }
    protected abstract IAction CreatePrimaryAction(MonopolyPlayer monopolyPlayer, PurchasableTile purchasableTile);
    protected abstract IAction CreateCancelAction();
    protected abstract string GetPrimaryButtonText(PurchasableTile purchasableTile);
}
public class GameCardBuyActions : GameCardActionsBase
{
    protected override IAction CreatePrimaryAction(MonopolyPlayer monopolyPlayer, PurchasableTile purchasableTile)
    {
        return new BuyAction(monopolyPlayer, purchasableTile);
    }

    protected override IAction CreateCancelAction()
    {
        return new CancelAction();
    }

    protected override string GetPrimaryButtonText(PurchasableTile purchasableTile)
    {
        return $"ACHETER {purchasableTile.getPrice()}M";
    }

}

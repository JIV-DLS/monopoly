using System;
using Monopoly;
using UnityEngine;
using UnityEngine.Serialization;
public interface IAction
{
    void Execute();
    void Execute(MonopolyPlayer player, PurchasableTile tile, GameCardLayout gameCardLayout);
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
        _action.Execute(_player, _tile, null);
        OnClickAction();
    }
}

public abstract class ActionOnCardLayout : IAction
{
    protected readonly MonopolyPlayer _player;
    protected readonly PurchasableTile _tile;
    protected readonly GameCardLayout _gameCardLayout;

    public ActionOnCardLayout(MonopolyPlayer player, PurchasableTile tile, GameCardLayout gameCardLayout)
    {
        _player = player ?? throw new ArgumentNullException(nameof(player));
        _tile = tile ?? throw new ArgumentNullException(nameof(tile));
        _gameCardLayout = gameCardLayout ?? throw new ArgumentNullException(nameof(gameCardLayout));
    }

    public virtual void Execute()
    {
        Execute(_player, _tile, _gameCardLayout);
    }

    public abstract void Execute(MonopolyPlayer player, PurchasableTile tile, GameCardLayout gameCardLayout);

    public virtual void Cancel()
    {
        Debug.Log("Buy action canceled.") ;
    }
}
public class BuyAction : ActionOnCardLayout
{

    public BuyAction(MonopolyPlayer player, PurchasableTile tile, GameCardLayout gameCardLayout) : base(player, tile, gameCardLayout)
    {
    }

    public override void Execute(MonopolyPlayer player, PurchasableTile tile, GameCardLayout gameCardLayout)
    {
        _player.Buy(_tile);
        gameCardLayout.Hide();
    }
}

public class CancelAction : ActionOnCardLayout
{
    public CancelAction(MonopolyPlayer player, PurchasableTile tile, GameCardLayout gameCardLayout) : base(player, tile, gameCardLayout)
    {
    }

    public override void Execute(MonopolyPlayer player, PurchasableTile tile, GameCardLayout gameCardLayout)
    {
        Debug.Log("Cancel action executed.");
        gameCardLayout.Hide();
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
        primaryButton.Handler = new ActionButtonHandler(CreatePrimaryAction(monopolyPlayer, purchasableTile, gameCardLayout));
        cancelButton.Handler = new ActionButtonHandler(CreateCancelAction(monopolyPlayer, purchasableTile, gameCardLayout));

        // Set button text dynamically
        primaryButton.SetButtonText(GetPrimaryButtonText(purchasableTile));
    }

    public void Hide()
    {
        gameCardLayout.Hide();
    }

    public void ShowPurchasableTile(PurchasableTile purchasableTile, MonopolyPlayer monopolyPlayer)
    {
        gameObject.SetActive(true);
        ShowActions(purchasableTile, monopolyPlayer);
    }
    protected abstract IAction CreatePrimaryAction(MonopolyPlayer monopolyPlayer, PurchasableTile purchasableTile, GameCardLayout gameCardLayout);
    protected abstract IAction CreateCancelAction(MonopolyPlayer monopolyPlayer, PurchasableTile purchasableTile, GameCardLayout gameCardLayout);
    protected abstract string GetPrimaryButtonText(PurchasableTile purchasableTile);
}
public class GameCardBuyActions : GameCardActionsBase
{
    protected override IAction CreatePrimaryAction(MonopolyPlayer monopolyPlayer, PurchasableTile purchasableTile, GameCardLayout gameCardLayout)
    {
        return new BuyAction(monopolyPlayer, purchasableTile, gameCardLayout);
    }

    protected override IAction CreateCancelAction(MonopolyPlayer monopolyPlayer, PurchasableTile purchasableTile, GameCardLayout gameCardLayout)
    {
        return new CancelAction(monopolyPlayer, purchasableTile, gameCardLayout);
    }

    protected override string GetPrimaryButtonText(PurchasableTile purchasableTile)
    {
        return $"ACHETER {purchasableTile.getPrice()}M";
    }

}

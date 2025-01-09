using System;
using UnityEngine;
public class BuildAction : IAction
{
    private readonly MonopolyPlayer _player;
    private readonly PurchasableTile _tile;

    public BuildAction(MonopolyPlayer player, PurchasableTile tile)
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
        // Check if the player can build on the tile
        if (_tile.CanBeUpgraded() && _player.canBeChargedOf(_tile.GetUpgradePrice()) && _tile is PropertyTile propertyTile)
        {
            _player.BuildOnPropertyTile(propertyTile);
        }
        else
        {
            Debug.LogError("Build action cannot be executed. Either tile or player conditions are not met.");
        }
    }

    public void Cancel()
    {
        Debug.Log("Build action canceled.");
    }
}
public class CancelBuildAction : IAction
{
    public void Execute()
    {
        Execute(null, null);
    }

    public void Execute(MonopolyPlayer player = null, PurchasableTile tile = null)
    {
        Debug.Log("Cancel build action executed.");
    }

    public void Cancel()
    {
        Debug.Log("Cancel build action canceled.");
    }
}
public class GameCardBuildActions : GameCardActionsBase
{
    protected override IAction CreatePrimaryAction(MonopolyPlayer monopolyPlayer, PurchasableTile purchasableTile)
    {
        return new BuildAction(monopolyPlayer, purchasableTile);
    }

    protected override IAction CreateCancelAction()
    {
        return new CancelBuildAction();
    }

    protected override string GetPrimaryButtonText(PurchasableTile purchasableTile)
    {
        return purchasableTile is PropertyTile propertyTile?$"CONSTRUIRE ({propertyTile.GetUpgradePrice()}M)":"IMPOSSIBLE DE CONSTRUIRE";
    }
}
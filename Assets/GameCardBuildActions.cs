using System;
using UnityEngine;
public class BuildAction : ActionOnCardLayout
{
    public BuildAction(MonopolyPlayer player, PurchasableTile tile, GameCardLayout gameCardLayout) : base(player, tile, gameCardLayout)
    {
    }

    public override void Execute(MonopolyPlayer player, PurchasableTile tile, GameCardLayout gameCardLayout)
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

}
public class CancelBuildAction : ActionOnCardLayout
{
    public CancelBuildAction(MonopolyPlayer player, PurchasableTile tile, GameCardLayout gameCardLayout) : base(player, tile, gameCardLayout)
    {
    }

    public override void Execute(MonopolyPlayer player, PurchasableTile tile, GameCardLayout gameCardLayout)
    {
        Debug.Log("Cancel build action executed.");
    }

}
public class GameCardBuildActions : GameCardActionsBase
{

    protected override IAction CreatePrimaryAction(MonopolyPlayer monopolyPlayer, PurchasableTile purchasableTile,
        GameCardLayout gameCardLayout)
    {
        return new BuildAction(monopolyPlayer, purchasableTile, gameCardLayout);
    }

    protected override IAction CreateCancelAction(MonopolyPlayer monopolyPlayer, PurchasableTile purchasableTile,
        GameCardLayout gameCardLayout)
    {
        return new CancelBuildAction(monopolyPlayer, purchasableTile, gameCardLayout);
    }

    protected override string GetPrimaryButtonText(PurchasableTile purchasableTile)
    {
        return purchasableTile is PropertyTile propertyTile?$"CONSTRUIRE ({propertyTile.GetUpgradePrice()}M)":"IMPOSSIBLE DE CONSTRUIRE";
    }
}
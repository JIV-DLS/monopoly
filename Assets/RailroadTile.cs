using System;
using System.Linq;
using UnityEngine;

public class RailroadTile : PurchasableTile
{
    public override Type GetTargetType()
    {
        return typeof(RailroadTile);
    }
    public RailRoadCard railRoadCard { get; private set; }
    public CardBehind titleDeedBehindCard { get; private set; }

    public override PurchasableFaceCard GetFaceCard()
    {
        return railRoadCard;
    }
    public  override PurchasableBehindCard GetBehindCard()
    {
        return titleDeedBehindCard;
    }
    public RailroadTile(GameObject tileGameObject, string name,
        RailRoadCard railRoadCard, CardBehind titleDeedBehindCard, int index, int groupIndex)
        : base(tileGameObject, name, new int[] { 25, 50, 100, 200 }, 200, index, groupIndex)
    {
        this.railRoadCard = (RailRoadCard)railRoadCard.Clone(this);
        this.titleDeedBehindCard = (CardBehind)titleDeedBehindCard.Clone(this);
    }

    public int GetCost()
    {
        return costs[monopolyGameManager.GetAllGroupOfThisPropertyTile<RailroadTile>().ToArray()
            .Count(railRoadTile => railRoadTile.monopolyPlayer == monopolyPlayer)];
    }
}
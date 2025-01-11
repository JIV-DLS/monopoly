using System;
using UnityEngine;

public class RedPropertyGroupTile : PropertyTile
{
    public override Type GetTargetType()
    {
        return typeof(RedPropertyGroupTile);
    }
    public RedPropertyGroupTile(GameObject tileGameObject, string name, int[] costs, int houseCost, int hotelCost, int price, TitleDeedCard titleDeedFaceCard, CardBehind titleDeedBehindCard, int index, int groupIndex) : base(tileGameObject, name, MonopolyColors.GetColor(MonopolyColors.PropertyColor.Red), costs, houseCost, hotelCost, price, titleDeedFaceCard, titleDeedBehindCard, index, groupIndex)
    {
    }
    public override PropertyTile[] GetAllGroupOfThisPropertyTile()
    {
        return monopolyGameManager.GetAllGroupOfThisPropertyTile<RedPropertyGroupTile>();
    }
}
using System;
using UnityEngine;

public class BrownPropertyGroupTile : PropertyTile
{
    public override Type GetTargetType()
    {
        return typeof(BrownPropertyGroupTile);
    }
    public BrownPropertyGroupTile(GameObject tileGameObject, string name, int[] costs, int houseCost, int hotelCost, int price, TitleDeedCard titleDeedFaceCard, CardBehind titleDeedBehindCard, int index, int groupIndex) : base(tileGameObject, name, MonopolyColors.GetColor(MonopolyColors.PropertyColor.Brown), costs, houseCost, hotelCost, price, titleDeedFaceCard, titleDeedBehindCard, index, groupIndex)
    {
    }
    public override PropertyTile[] GetAllGroupOfThisPropertyTile()
    {
        return monopolyGameManager.GetAllGroupOfThisPropertyTile<BrownPropertyGroupTile>();
    }
}
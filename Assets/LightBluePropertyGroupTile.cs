using System;
using UnityEngine;

public class LightBluePropertyGroupTile : PropertyTile
{
    public override Type GetTargetType()
    {
        return typeof(LightBluePropertyGroupTile);
    }
    public LightBluePropertyGroupTile(GameObject tileGameObject, string name, int[] costs, int houseCost, int hotelCost, int price, TitleDeedCard titleDeedFaceCard, CardBehind titleDeedBehindCard, int index, int groupIndex) : base(tileGameObject, name, MonopolyColors.GetColor(MonopolyColors.PropertyColor.LightBlue), costs, houseCost, hotelCost, price, titleDeedFaceCard, titleDeedBehindCard, index, groupIndex)
    {
    }
    public override PropertyTile[] GetAllGroupOfThisPropertyTile()
    {
        return monopolyGameManager.GetAllGroupOfThisPropertyTile<LightBluePropertyGroupTile>();
    }
}
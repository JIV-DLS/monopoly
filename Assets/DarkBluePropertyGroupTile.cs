using System;
using UnityEngine;

public class DarkBluePropertyGroupTile : PropertyTile
{
    public override Type GetTargetType()
    {
        return typeof(DarkBluePropertyGroupTile);
    }
    public DarkBluePropertyGroupTile(GameObject tileGameObject, string name,  int[] costs, int houseCost, int hotelCost, int price, TitleDeedCard titleDeedFaceCard, CardBehind titleDeedBehindCard, int index, int groupIndex) : base(tileGameObject, name, MonopolyColors.GetColor(MonopolyColors.PropertyColor.DarkBlue), costs, houseCost, hotelCost, price, titleDeedFaceCard, titleDeedBehindCard, index, groupIndex)
    {
    }

    public override PropertyTile[] GetAllGroupOfThisPropertyTile()
    {
        return monopolyGameManager.GetAllGroupOfThisPropertyTile<DarkBluePropertyGroupTile>();
    }
}
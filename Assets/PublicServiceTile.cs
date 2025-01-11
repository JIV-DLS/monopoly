using System;
using UnityEngine;

public abstract class PublicServiceTile : PurchasableTile
{
    public PublicServiceCard publicService { get; private set; }
    public CardBehind titleDeedBehindCard { get; private set; }

    public override Type GetTargetType()
    {
        return typeof(PublicServiceTile);
    }
    public override PurchasableFaceCard GetFaceCard()
    {
        return publicService;
    }
    public  override PurchasableBehindCard GetBehindCard()
    {
        return titleDeedBehindCard;
    }

    public PublicServiceTile(GameObject tileGameObject, string name, PublicServiceCard publicService,
        CardBehind titleDeedBehindCard, int index, int groupIndex)
        : base(tileGameObject, name, new int[] { 4, 10 }, 150, index, groupIndex)
    {
        this.publicService = (PublicServiceCard)publicService.Clone(this);
        this.titleDeedBehindCard = (CardBehind)titleDeedBehindCard.Clone(this);
    }

    public abstract Sprite GetImageSprite();
    
    public override string GetCostText()
    {
        return $"{GetLevelCost()}x M le résultat le lancé";
    }

}
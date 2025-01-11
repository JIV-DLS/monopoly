using UnityEngine;

public class ElectricityTile : PublicServiceTile
{
    
    public ElectricityTile(GameObject tileGameObject, PublicServiceCard publicService,
        CardBehind titleDeedBehindCard, int index, int groupIndex)
        : base(tileGameObject, "Compagnie de distribution d'électricité", publicService, titleDeedBehindCard, index, groupIndex)
    {
    }

    public override Sprite GetImageSprite()
    {
        return ChildUtility.GetChildComponentByName<BaseImageHandler>(tileGameObject.transform.parent, "ElectricityPrefab").GetSprite();
    }

}
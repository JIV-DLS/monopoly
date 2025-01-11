using UnityEngine;

public class WaterPumpTile : PublicServiceTile
{
    public WaterPumpTile(GameObject tileGameObject, PublicServiceCard publicService,
        CardBehind titleDeedBehindCard, int index, int groupIndex)
        : base(tileGameObject, "Compagnie de distribution des Eaux", publicService, titleDeedBehindCard, index, groupIndex)
    {
    }

    public override Sprite GetImageSprite()
    {
        return ChildUtility.GetChildComponentByName<BaseImageHandler>(tileGameObject.transform.parent, "WaterPrefab").GetSprite();
    }
}
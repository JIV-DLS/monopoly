using UnityEngine;

public class PublicServiceCard : PurchasableFaceCard
{
    private BaseTextHandler _publicServiceNameValue;
    private BaseImageHandler _serviceImageNameValue;
    private TitleValuePlayerPosition _haveOneValueTitleValuePlayerPosition;
    private TitleValuePlayerPosition _haveTwoValueTitleValuePlayerPosition;


    public override void OtherInit()
    {
        base.OtherInit();
        _publicServiceNameValue = ChildUtility.GetChildComponentByName<BaseTextHandler>(transform, "PublicServiceNameValue");
        if (_publicServiceNameValue == null)
        {
            Debug.LogError("public service is null");
        }
        _serviceImageNameValue = ChildUtility.GetChildComponentByName<BaseImageHandler>(transform, "ServiceImage");
        if (_serviceImageNameValue == null)
        {
            Debug.LogError("service image is null");
        }
        
        _haveOneValueTitleValuePlayerPosition  = ChildUtility.GetChildComponentByName<TitleValuePlayerPosition>(transform, "haveOneValue");
        if (_haveOneValueTitleValuePlayerPosition == null)
        {
            Debug.LogError("rent with 2 houses value not found");
        }
        _haveTwoValueTitleValuePlayerPosition  = ChildUtility.GetChildComponentByName<TitleValuePlayerPosition>(transform, "haveTwoValue");
        if (_haveTwoValueTitleValuePlayerPosition == null)
        {
            Debug.LogError("rent with 3 houses value not found");
        }
    }

    
    
    public override PurchasableCard Clone (PurchasableTile purchasableTile)
    {
        return ((PublicServiceCard)Clone()).UpdateTile((PublicServiceTile)purchasableTile);
    }

    private PublicServiceCard UpdateTile(PublicServiceTile publicServiceTile)
    {
        base.UpdateTile(publicServiceTile);
        _publicServiceNameValue.SetText(publicServiceTile.TileName);
        _serviceImageNameValue.UpdateImage(publicServiceTile.GetImageSprite());
        return this;
    }
    
    public override PurchasableCard Clone ()
    {
        return base.Clone<PublicServiceCard>();
    }
    public override void HandlePurchase(IPurchasableTileLevel purchasableTileLevel)
    {
        CleanPurchases();
        
        
        switch (purchasableTileLevel.GetLevel())
        {
            case 0:
                _haveOneValueTitleValuePlayerPosition.SetPlayerPosition(purchasableTileLevel.GetOwner()._playerElementOnMap.GetSprite());
                break;
            case 1:
                _haveTwoValueTitleValuePlayerPosition.SetPlayerPosition(purchasableTileLevel.GetOwner()._playerElementOnMap.GetSprite());
                break;
        }
    }

    public override void CleanPurchases()
    {
        _haveOneValueTitleValuePlayerPosition.ClearPlayerPosition();
        _haveTwoValueTitleValuePlayerPosition.ClearPlayerPosition();
    }
}

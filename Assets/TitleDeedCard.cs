using UnityEngine;
using UnityEngine.UI; // Required for the Image component

public class TitleDeedCard : PurchasableFaceCard
{
    private Image _cardHeaderImage;
    private BaseTextHandler _tileDeedValueTitleValuePlayerPosition;
    private TitleValuePlayerPosition _rentValueTitleValuePlayerPosition;
    private TitleValuePlayerPosition _rentFullGroupValueTitleValuePlayerPosition;
    private TitleValuePlayerPosition _rentWith1HouseValueTitleValuePlayerPosition;
    private TitleValuePlayerPosition _rentWith2HousesValueTitleValuePlayerPosition;
    private TitleValuePlayerPosition _rentWith3HousesValueTitleValuePlayerPosition;
    private TitleValuePlayerPosition _rentWith4HousesValueTitleValuePlayerPosition;
    private TitleValuePlayerPosition _rentWith1HotelValueTitleValuePlayerPosition;
    private BaseTextHandler _houseCostValueTitleValuePlayerPosition;
    private BaseTextHandler _hotelCostValueTitleValuePlayerPosition;
    
    public override void OtherInit()
    {
        base.OtherInit();
        // Use the utility method to get the Image component from a child named "ChildWithImage"
        _cardHeaderImage = ChildUtility.GetChildComponentByName<Image>(transform, "CardHeader");

        if (_cardHeaderImage == null)
        {
            Debug.LogError("Card header image value not found");
        }
        _tileDeedValueTitleValuePlayerPosition = ChildUtility.GetChildComponentByName<BaseTextHandler>(transform, "TitleDeedValue");

        if (_tileDeedValueTitleValuePlayerPosition == null)
        {
            Debug.LogError("Tile deed value not found");
        }
        _rentValueTitleValuePlayerPosition = ChildUtility.GetChildComponentByName<TitleValuePlayerPosition>(transform, "RentValue");
        if (_rentValueTitleValuePlayerPosition == null)
        {
            Debug.LogError("rent value not found");
        }
        _rentFullGroupValueTitleValuePlayerPosition = ChildUtility.GetChildComponentByName<TitleValuePlayerPosition>(transform, "RentFullGroupValue");
        if (_rentFullGroupValueTitleValuePlayerPosition == null)
        {
            Debug.LogError("rent full group value not found");
        }
        _rentWith1HouseValueTitleValuePlayerPosition  = ChildUtility.GetChildComponentByName<TitleValuePlayerPosition>(transform, "RentWith1HouseValue");
        if (_rentWith1HouseValueTitleValuePlayerPosition == null)
        {
            Debug.LogError("rent with 1 house value not found");
        }
        _rentWith2HousesValueTitleValuePlayerPosition  = ChildUtility.GetChildComponentByName<TitleValuePlayerPosition>(transform, "RentWith2HousesValue");
        if (_rentWith2HousesValueTitleValuePlayerPosition == null)
        {
            Debug.LogError("rent with 2 houses value not found");
        }
        _rentWith3HousesValueTitleValuePlayerPosition  = ChildUtility.GetChildComponentByName<TitleValuePlayerPosition>(transform, "RentWith3HousesValue");
        if (_rentWith3HousesValueTitleValuePlayerPosition == null)
        {
            Debug.LogError("rent with 3 houses value not found");
        }
        _rentWith4HousesValueTitleValuePlayerPosition  = ChildUtility.GetChildComponentByName<TitleValuePlayerPosition>(transform, "RentWith4HousesValue");
        if (_rentWith4HousesValueTitleValuePlayerPosition == null)
        {
            Debug.LogError("rent with 4 houses value not found");
        }
        _rentWith1HotelValueTitleValuePlayerPosition  = ChildUtility.GetChildComponentByName<TitleValuePlayerPosition>(transform, "RentWith1HotelValue");
        if (_rentWith1HotelValueTitleValuePlayerPosition == null)
        {
            Debug.LogError("rent with 1 hotel value not found");
        }
        _houseCostValueTitleValuePlayerPosition  = ChildUtility.GetChildComponentByName<BaseTextHandler>(transform, "HouseCostValue");
        if (_houseCostValueTitleValuePlayerPosition == null)
        {
            Debug.LogError("house cost value not found");
        }
        _hotelCostValueTitleValuePlayerPosition  = ChildUtility.GetChildComponentByName<BaseTextHandler>(transform, "HotelCostValue");
        if (_hotelCostValueTitleValuePlayerPosition == null)
        {
            Debug.LogError("hotel cost value not found");
        }
    }

    public override PurchasableCard Clone ()
    {
        return base.Clone<TitleDeedCard>();
    }
    public override PurchasableCard Clone (PurchasableTile purchasableTile)
    {
        return ((TitleDeedCard)Clone()).UpdateTile((PropertyTile)purchasableTile);
    }

    private TitleDeedCard UpdateTile(PropertyTile propertyTile)
    {
        base.UpdateTile(propertyTile);
        _cardHeaderImage.color = propertyTile.color;
        _tileDeedValueTitleValuePlayerPosition.SetText(propertyTile.TileName.ToUpper());
        _rentValueTitleValuePlayerPosition.SetText($"{propertyTile.costs[0]}M");
        _rentFullGroupValueTitleValuePlayerPosition.SetText($"{propertyTile.costs[1]}M");
        _rentWith1HouseValueTitleValuePlayerPosition.SetText($"{propertyTile.costs[2]}M");
        _rentWith2HousesValueTitleValuePlayerPosition.SetText($"{propertyTile.costs[3]}M");
        _rentWith3HousesValueTitleValuePlayerPosition.SetText($"{propertyTile.costs[4]}M");
        _rentWith4HousesValueTitleValuePlayerPosition.SetText($"{propertyTile.costs[5]}M");
        _rentWith1HotelValueTitleValuePlayerPosition.SetText($"{propertyTile.costs[6]}M");
        _houseCostValueTitleValuePlayerPosition.SetText($"{propertyTile.houseCost} M chacune");
        _hotelCostValueTitleValuePlayerPosition.SetText($"{propertyTile.hotelCost} M chacun (Plus 4 maisons)");
        return this;
    }

    public override void HandlePurchase(IPurchasableTileLevel purchasableTileLevel)
    {
        CleanPurchases();
        
        switch (purchasableTileLevel.GetLevel())
        {
            case 0:
                _rentValueTitleValuePlayerPosition.SetPlayerPosition(purchasableTileLevel.GetOwner().playerElementOnMap.GetSprite());
                break;
            case 1:
                _rentFullGroupValueTitleValuePlayerPosition.SetPlayerPosition(purchasableTileLevel.GetOwner().playerElementOnMap.GetSprite());
                break;
            case 2:
                _rentWith1HouseValueTitleValuePlayerPosition.SetPlayerPosition(purchasableTileLevel.GetOwner().playerElementOnMap.GetSprite());
                break;
            case 3:
                _rentWith2HousesValueTitleValuePlayerPosition.SetPlayerPosition(purchasableTileLevel.GetOwner().playerElementOnMap.GetSprite());
                break;
            case 4:
                _rentWith3HousesValueTitleValuePlayerPosition.SetPlayerPosition(purchasableTileLevel.GetOwner().playerElementOnMap.GetSprite());
                break;
            case 5:
                _rentWith4HousesValueTitleValuePlayerPosition.SetPlayerPosition(purchasableTileLevel.GetOwner().playerElementOnMap.GetSprite());
                break;
            case 6:
                _rentWith1HotelValueTitleValuePlayerPosition.SetPlayerPosition(purchasableTileLevel.GetOwner().playerElementOnMap.GetSprite());
                break;
        }

            


    }

    public override void CleanPurchases()
    {
        _rentValueTitleValuePlayerPosition.ClearPlayerPosition();
        _rentFullGroupValueTitleValuePlayerPosition.ClearPlayerPosition();
        _rentWith1HouseValueTitleValuePlayerPosition.ClearPlayerPosition();
        _rentWith2HousesValueTitleValuePlayerPosition.ClearPlayerPosition();
        _rentWith3HousesValueTitleValuePlayerPosition.ClearPlayerPosition();
        _rentWith4HousesValueTitleValuePlayerPosition.ClearPlayerPosition();
        _rentWith1HotelValueTitleValuePlayerPosition.ClearPlayerPosition();
    }
}

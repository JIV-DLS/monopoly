using UnityEngine;

public class RailRoadCard : PurchasableFaceCard
{
    private BaseTextHandler _railroadNameValue;
    private TitleValuePlayerPosition _rentValueTitleValuePlayerPosition;
    private TitleValuePlayerPosition _rentWith2TrainsStationValueTitleValuePlayerPosition;
    private TitleValuePlayerPosition _rentWith3TrainsStationValueTitleValuePlayerPosition;

    private TitleValuePlayerPosition _rentWith4TrainsStationValueTitleValuePlayerPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void OtherInit()
    {
        base.OtherInit();
        _railroadNameValue =
            ChildUtility.GetChildComponentByName<BaseTextHandler>(transform, "RailroadNameValue");

        if (_railroadNameValue == null)
        {
            Debug.LogError("Rail road name value not found");
        }

        _rentValueTitleValuePlayerPosition =
            ChildUtility.GetChildComponentByName<TitleValuePlayerPosition>(transform, "RentValue");
        if (_rentValueTitleValuePlayerPosition == null)
        {
            Debug.LogError("rent value not found");
        }

        _rentWith2TrainsStationValueTitleValuePlayerPosition =
            ChildUtility.GetChildComponentByName<TitleValuePlayerPosition>(transform, "RentWith2TrainsStationValue");
        if (_rentWith2TrainsStationValueTitleValuePlayerPosition == null)
        {
            Debug.LogError("rent with 2 houses value not found");
        }

        _rentWith3TrainsStationValueTitleValuePlayerPosition =
            ChildUtility.GetChildComponentByName<TitleValuePlayerPosition>(transform, "RentWith3TrainsStationValue");
        if (_rentWith3TrainsStationValueTitleValuePlayerPosition == null)
        {
            Debug.LogError("rent with 3 houses value not found");
        }

        _rentWith4TrainsStationValueTitleValuePlayerPosition =
            ChildUtility.GetChildComponentByName<TitleValuePlayerPosition>(transform, "RentWith4TrainsStationValue");
        if (_rentWith4TrainsStationValueTitleValuePlayerPosition == null)
        {
            Debug.LogError("rent with 4 houses value not found");
        }
    }


    public override PurchasableCard Clone ()
    {
        return base.Clone<RailRoadCard>();
    }
    public override PurchasableCard Clone(PurchasableTile purchasableTile)
    {
        return ((RailRoadCard)Clone()).UpdateTile((RailroadTile)purchasableTile);
    }
    public RailRoadCard UpdateTile(RailroadTile railroadTile)
    {
        base.UpdateTile(railroadTile);
        _railroadNameValue.SetText(railroadTile.TileName.ToUpper());
        _rentValueTitleValuePlayerPosition.SetText($"{railroadTile.costs[0]}M");
        _rentWith2TrainsStationValueTitleValuePlayerPosition.SetText($"{railroadTile.costs[1]}M");
        _rentWith3TrainsStationValueTitleValuePlayerPosition.SetText($"{railroadTile.costs[2]}M");
        _rentWith4TrainsStationValueTitleValuePlayerPosition.SetText($"{railroadTile.costs[3]}M");
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
                _rentWith2TrainsStationValueTitleValuePlayerPosition.SetPlayerPosition(purchasableTileLevel.GetOwner().playerElementOnMap.GetSprite());
                break;
            case 2:
                _rentWith3TrainsStationValueTitleValuePlayerPosition.SetPlayerPosition(purchasableTileLevel.GetOwner().playerElementOnMap.GetSprite());
                break;
            case 3:
                _rentWith4TrainsStationValueTitleValuePlayerPosition.SetPlayerPosition(purchasableTileLevel.GetOwner().playerElementOnMap.GetSprite());
                break;

        }
    }

    public override void CleanPurchases()
    {
        _rentValueTitleValuePlayerPosition.ClearPlayerPosition();
        _rentWith2TrainsStationValueTitleValuePlayerPosition.ClearPlayerPosition();
        _rentWith3TrainsStationValueTitleValuePlayerPosition.ClearPlayerPosition();
        _rentWith4TrainsStationValueTitleValuePlayerPosition.ClearPlayerPosition();
    }
}

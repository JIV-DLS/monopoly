using UnityEngine;

public class RailRoadCard : PurchasableFaceCard
{
    private BaseTextHandler _railroadNameValue;
    private BaseTextHandler _rentValueBaseTextHandler;
    private BaseTextHandler _rentWith2TrainsStationValueBaseTextHandler;
    private BaseTextHandler _rentWith3TrainsStationValueBaseTextHandler;
    private BaseTextHandler _rentWith4TrainsStationValueBaseTextHandler;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _railroadNameValue = ChildUtility.GetChildComponentByName<BaseTextHandler>(transform, "RailroadNameValue");

        if (_railroadNameValue == null)
        {
            Debug.LogError("Rail road name value not found");
        }
        _rentValueBaseTextHandler = ChildUtility.GetChildComponentByName<BaseTextHandler>(transform, "RentValue");
        if (_rentValueBaseTextHandler == null)
        {
            Debug.LogError("rent value not found");
        }
        _rentWith2TrainsStationValueBaseTextHandler  = ChildUtility.GetChildComponentByName<BaseTextHandler>(transform, "RentWith2TrainsStationValue");
        if (_rentWith2TrainsStationValueBaseTextHandler == null)
        {
            Debug.LogError("rent with 2 houses value not found");
        }
        _rentWith3TrainsStationValueBaseTextHandler  = ChildUtility.GetChildComponentByName<BaseTextHandler>(transform, "RentWith3TrainsStationValue");
        if (_rentWith3TrainsStationValueBaseTextHandler == null)
        {
            Debug.LogError("rent with 3 houses value not found");
        }
        _rentWith4TrainsStationValueBaseTextHandler  = ChildUtility.GetChildComponentByName<BaseTextHandler>(transform, "RentWith4TrainsStationValue");
        if (_rentWith4TrainsStationValueBaseTextHandler == null)
        {
            Debug.LogError("rent with 4 houses value not found");
        }
        
    }

    public RailRoadCard Clone(PurchasableTile purchasableTile)
    {
        return Clone().UpdateTile(purchasableTile);
    }
    public RailRoadCard Clone ()
    {
        return base.Clone<RailRoadCard>();
    }
    public RailRoadCard UpdateTile(PurchasableTile purchasableTile)
    {
        base.UpdateTile(purchasableTile);
        _railroadNameValue.SetText(purchasableTile.TileName.ToUpper());
        _rentValueBaseTextHandler.SetText($"{purchasableTile.costs[0]}M");
        _rentWith2TrainsStationValueBaseTextHandler.SetText($"{purchasableTile.costs[1]}M");
        _rentWith3TrainsStationValueBaseTextHandler.SetText($"{purchasableTile.costs[2]}M");
        _rentWith4TrainsStationValueBaseTextHandler.SetText($"{purchasableTile.costs[3]}M");
        return this;
    }
}

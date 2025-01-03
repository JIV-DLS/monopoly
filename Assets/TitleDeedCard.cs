using UnityEngine;

public class TitleDeedCard : MonoBehaviour
{
    private BaseTextHandler _titleDeedValueBaseTextHandler;
    private BaseTextHandler _rentValueBaseTextHandler;
    private BaseTextHandler _rentFullGroupValueBaseTextHandler;
    private BaseTextHandler _rentWith1HouseValueBaseTextHandler;
    private BaseTextHandler _rentWith2HousesValueBaseTextHandler;
    private BaseTextHandler _rentWith3HousesValueBaseTextHandler;
    private BaseTextHandler _rentWith4HousesValueBaseTextHandler;
    private BaseTextHandler _rentWith1HotelValueBaseTextHandler;
    private BaseTextHandler _houseCostValueBaseTextHandler;
    private BaseTextHandler _hotelCostValueBaseTextHandler;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _titleDeedValueBaseTextHandler = ChildUtility.GetChildComponentByName<BaseTextHandler>(transform, "TitleDeedValue");

        if (_titleDeedValueBaseTextHandler == null)
        {
            Debug.LogError("Title deed value not found");
        }
        _rentValueBaseTextHandler = ChildUtility.GetChildComponentByName<BaseTextHandler>(transform, "RentValue");
        if (_rentValueBaseTextHandler == null)
        {
            Debug.LogError("rent value not found");
        }
        _rentFullGroupValueBaseTextHandler = ChildUtility.GetChildComponentByName<BaseTextHandler>(transform, "RentFullGroupValue");
        if (_rentFullGroupValueBaseTextHandler == null)
        {
            Debug.LogError("rent full group value not found");
        }
        _rentWith1HouseValueBaseTextHandler  = ChildUtility.GetChildComponentByName<BaseTextHandler>(transform, "RentWith1HouseValue");
        if (_rentWith1HouseValueBaseTextHandler == null)
        {
            Debug.LogError("rent with 1 house value not found");
        }
        _rentWith2HousesValueBaseTextHandler  = ChildUtility.GetChildComponentByName<BaseTextHandler>(transform, "RentWith2HousesValue");
        if (_rentWith2HousesValueBaseTextHandler == null)
        {
            Debug.LogError("rent with 2 houses value not found");
        }
        _rentWith3HousesValueBaseTextHandler  = ChildUtility.GetChildComponentByName<BaseTextHandler>(transform, "RentWith3HousesValue");
        if (_rentWith3HousesValueBaseTextHandler == null)
        {
            Debug.LogError("rent with 3 houses value not found");
        }
        _rentWith4HousesValueBaseTextHandler  = ChildUtility.GetChildComponentByName<BaseTextHandler>(transform, "RentWith4HousesValue");
        if (_rentWith4HousesValueBaseTextHandler == null)
        {
            Debug.LogError("rent with 4 houses value not found");
        }
        _rentWith1HotelValueBaseTextHandler  = ChildUtility.GetChildComponentByName<BaseTextHandler>(transform, "RentWith1HotelValue");
        if (_rentWith1HotelValueBaseTextHandler == null)
        {
            Debug.LogError("rent with 1 hotel value not found");
        }
        _houseCostValueBaseTextHandler  = ChildUtility.GetChildComponentByName<BaseTextHandler>(transform, "HouseCostValue");
        if (_houseCostValueBaseTextHandler == null)
        {
            Debug.LogError("house cost value not found");
        }
        _hotelCostValueBaseTextHandler  = ChildUtility.GetChildComponentByName<BaseTextHandler>(transform, "HotelCostValue");
        if (_hotelCostValueBaseTextHandler == null)
        {
            Debug.LogError("hotel cost value not found");
        }
    }

}

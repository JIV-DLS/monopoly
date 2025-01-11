public abstract class PropertyTileStateCanBuildHouse : PropertyTileState
{
    protected PropertyTileStateCanBuildHouse(PropertyTile propertyTile) : base(propertyTile)
    {
    }

    public override bool CanBuildBeUpgraded()
    {
        return true;
    }
    public override bool CanBuildBeDowngraded()
    {
        return true;
    }

    public override int GetHotelNumber()
    {
        return 0;
    }

    public override int GetLevel()
    {
        return GetHousesNumber();
    }

    public override string GetName()
    {
        return "maison";
    }
}
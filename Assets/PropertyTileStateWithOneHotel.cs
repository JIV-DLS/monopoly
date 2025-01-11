public class PropertyTileStateWithOneHotel : PropertyTileState
{
    public PropertyTileStateWithOneHotel(PropertyTile propertyTile) : base(propertyTile)
    {
    }

    public override bool CanBuildBeUpgraded()
    {
        return false;
    }
    public override bool CanBuildBeDowngraded()
    {
        return true;
    }

    public override void Upgrade(IPropertyTileStateHolder holder)
    {
        throw new System.Exception("PropertyTileStateCanBuildHotel can't be upgraded");
    }
    public override void Downgrade(IPropertyTileStateHolder holder)
    {
        holder.SetPropertyTileState(new PropertyTileStateWithFourHouses(PropertyTile));
    }

    public override int GetHousesNumber()
    {
        return 4;
    }


    public override int GetHotelNumber()
    {
        return 1;
    }

    public override int GetLevel()
    {
        return 5;
    }
    
    public override string GetName()
    {
        return "maison";
    }
}
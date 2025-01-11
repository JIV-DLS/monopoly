public class PropertyTileStateWithOneHouse : PropertyTileStateCanBuildHouse
{
    public PropertyTileStateWithOneHouse(PropertyTile propertyTile) : base(propertyTile)
    {
    }

    public override int GetHousesNumber()
    {
        return 1;
    }
    public override void Upgrade(IPropertyTileStateHolder holder)
    {
        holder.SetPropertyTileState(new PropertyTileStateWithTwoHouses(PropertyTile));
    }
    
    public override void Downgrade(IPropertyTileStateHolder holder)
    {
        holder.SetPropertyTileState(new PropertyTileStateWithNoHouse(PropertyTile));
    }
}
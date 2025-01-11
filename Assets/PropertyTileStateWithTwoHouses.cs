public class PropertyTileStateWithTwoHouses : PropertyTileStateCanBuildHouse
{
    public PropertyTileStateWithTwoHouses(PropertyTile propertyTile) : base(propertyTile)
    {
    }

    public override int GetHousesNumber()
    {
        return 2;
    }
    public override void Upgrade(IPropertyTileStateHolder holder)
    {
        holder.SetPropertyTileState(new PropertyTileStateWithThreeHouses(PropertyTile));
    }
    
    public override void Downgrade(IPropertyTileStateHolder holder)
    {
        holder.SetPropertyTileState(new PropertyTileStateWithOneHouse(PropertyTile));
    }
}
public class PropertyTileStateWithThreeHouses : PropertyTileStateCanBuildHouse
{
    public PropertyTileStateWithThreeHouses(PropertyTile propertyTile) : base(propertyTile)
    {
    }

    public override int GetHousesNumber()
    {
        return 3;
    }
    public override void Upgrade(IPropertyTileStateHolder holder)
    {
        holder.SetPropertyTileState(new PropertyTileStateWithFourHouses(PropertyTile));
    }
    public override void Downgrade(IPropertyTileStateHolder holder)
    {
        holder.SetPropertyTileState(new PropertyTileStateWithTwoHouses(PropertyTile));
    }
}
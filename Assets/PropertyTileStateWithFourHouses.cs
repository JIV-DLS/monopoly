public class PropertyTileStateWithFourHouses : PropertyTileStateCanBuildHouse
{
    public PropertyTileStateWithFourHouses(PropertyTile propertyTile) : base(propertyTile)
    {
    }

    public override bool CanBuildBeUpgraded()
    {
        return PropertyTile.CheckIfOwnerDeckGroupHasBuildAtLeastFourHouses();
    }
    public override bool CanBuildBeDowngraded()
    {
        return PropertyTile.CheckIfOwnerDeckGroupHasNoHotel();
    }
    public override int GetHousesNumber()
    {
        return 4;
    }
    public override void Upgrade(IPropertyTileStateHolder holder)
    {
        holder.SetPropertyTileState(new PropertyTileStateWithOneHotel(PropertyTile));
    }
    public override void Downgrade(IPropertyTileStateHolder holder)
    {
        holder.SetPropertyTileState(new PropertyTileStateWithThreeHouses(PropertyTile));
    }
}
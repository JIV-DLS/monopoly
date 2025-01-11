public class PropertyTileStateWithNoHouse : PropertyTileStateCanBuildHouse
{
    public PropertyTileStateWithNoHouse(PropertyTile propertyTile) : base(propertyTile)
    {
    }

    public override int GetHousesNumber()
    {
        return 0;
    }
    public override bool CanBuildBeUpgraded()
    {
        return PropertyTile.CheckIfOwnerDeckHasThisGroup();
    }
    public override bool CanBuildBeDowngraded()
    {
        return false;
    }


    public override void Upgrade(IPropertyTileStateHolder holder)
    {
        holder.SetPropertyTileState(new PropertyTileStateWithOneHouse(PropertyTile));
    }

    public override void Downgrade(IPropertyTileStateHolder holder)
    {
        throw new System.Exception("PropertyTileStateWithNoHouse can't be downgraded");
    }
}
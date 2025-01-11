using Monopoly;

public abstract class PropertyTileState:IPropertyTileActionsPossibilityState, IPurchasableTileLevel
{
    protected readonly PropertyTile PropertyTile;

    protected PropertyTileState(PropertyTile propertyTile)
    {
        PropertyTile = propertyTile;
    }

    public int CompareTo(PropertyTileState other)
    {
        if (other == null)
        {
            return 1;
        }
        int currentLevel = GetLevel();
        int otherLevel = other.GetLevel();
        if (currentLevel > otherLevel)
        {
            return 1;
        }
        if (currentLevel < otherLevel)
        {
            return -1;
        }

        return 0;

    }
    public abstract void Upgrade(IPropertyTileStateHolder holder);
    public abstract void Downgrade(IPropertyTileStateHolder holder);
    public abstract int GetHousesNumber();
    public abstract int GetHotelNumber();
    public abstract bool CanBuildBeUpgraded();

    public abstract bool CanBuildBeDowngraded();

    public abstract int GetLevel();
    public MonopolyPlayer GetOwner()
    {
        return PropertyTile.GetOwner();
    }

    public abstract string GetName();
}
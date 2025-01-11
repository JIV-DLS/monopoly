using System;
using System.Linq;
using UnityEngine;

public abstract class PropertyTile : PurchasableTile, IPropertyTileStateHolder, IPropertyTileActionsPossibilityState
{
    public abstract PropertyTile[] GetAllGroupOfThisPropertyTile();
    public PropertyTileState propertyTileState { get; private set; }
    public int houseCost { get; private set; }
    public int hotelCost { get; private set; }
    public string owner { get; private set; }
    public Color color { get; set; }
    public TitleDeedCard titleDeedFaceCard { get; private set; }
    public CardBehind titleDeedBehindCard { get; private set; }

    public override void RemoveOwner()
    {
        base.RemoveOwner();
        propertyTileState = new PropertyTileStateWithNoHouse(this);
    }
    public override bool HaveTilesGood()
    {
        return CanBuildBeDowngraded();
    }
    public override int GetLevelOnTypeCount()
    {
        if (!DoesAllGroupOfThisPropertyTileIsOwnedByTheSamePlayer())
        {
            return 0;
        }

        return propertyTileState.GetLevel()+1;
    }
    public bool CheckIfOwnerDeckGroupHasBuildAtLeastFourHouses()
    {
        return GetOwner().deck.GetAllGroupOfThisPropertyTile(GetTargetType()).All(propertyTile =>
            ((PropertyTile)propertyTile).HaveFourHousesOrHotel());
    }
    public bool HaveFourHousesOrHotel()
    {
        return propertyTileState is PropertyTileStateWithFourHouses or PropertyTileStateWithOneHotel;
    }
    private bool DoesAllGroupOfThisPropertyTileIsOwnedByTheSamePlayer()
    {
        return monopolyGameManager.DoesAllGroupOfThisPropertyTileIsOwnedByTheSamePlayer(this);
    }

    public override PurchasableFaceCard GetFaceCard()
    {
        return titleDeedFaceCard;
    }
    public  override PurchasableBehindCard GetBehindCard()
    {
        return titleDeedBehindCard;
    }
    
    public PropertyTile(GameObject tileGameObject, string name, Color color, int[] costs, int houseCost, int hotelCost,
        int price, TitleDeedCard titleDeedFaceCard, CardBehind titleDeedBehindCard, int index, int groupIndex)
        : base(tileGameObject, name, costs, price, index, groupIndex)
    {
        this.houseCost = houseCost;
        this.hotelCost = hotelCost;
        this.color = color;
        this.titleDeedFaceCard = (TitleDeedCard)titleDeedFaceCard.Clone(this);
        this.titleDeedBehindCard = (CardBehind)titleDeedBehindCard.Clone(this);
        propertyTileState = new PropertyTileStateWithNoHouse(this);
        owner = null; // No owner initially
    }

    public void SetPropertyTileState(PropertyTileState propertyTileStateToSet)
    {
        propertyTileState = propertyTileStateToSet;
    }

    public override bool CanBeUpgraded()
    {
        return CanBuildBeUpgraded();
    }
    
    public override int GetUpgradePrice()
    {
        switch (propertyTileState)
        {
            case PropertyTileStateWithNoHouse:
            case PropertyTileStateWithOneHouse:
            case PropertyTileStateWithTwoHouses:
            case PropertyTileStateWithThreeHouses:
                return houseCost;
            case PropertyTileStateWithFourHouses:
                return hotelCost;
        }
        throw new NotImplementedException();
    }
    public void UpgradeGood()
    {
        if (CanBuildBeUpgraded())
        {
            propertyTileState.Upgrade(this);
        }
    }
    public void DowngradeGood()
    {
        if (CanBuildBeDowngraded())
        {
            propertyTileState.Downgrade(this);
        }
    }

    public bool CanBuildBeUpgraded()
    {
        return propertyTileState.CanBuildBeUpgraded();
    }

    public bool CanBuildBeDowngraded()
    {
        return propertyTileState.CanBuildBeDowngraded();
    }
    
    public override IGood GetMinimumGoodToSell()
    {
        switch (propertyTileState)
        {
            case PropertyTileStateWithOneHotel:
                return new PropertyHotelTileGood(this);
            case PropertyTileStateWithOneHouse:
            case PropertyTileStateWithTwoHouses:
            case PropertyTileStateWithThreeHouses:
            case PropertyTileStateWithFourHouses:
                PropertyTile downgradeablePropertyTileSearchingForHotel = GetAllGroupOfThisPropertyTile()
                    .FirstOrDefault(tile => tile.CanBuildBeDowngraded() && tile != this);
                return downgradeablePropertyTileSearchingForHotel==null? new PropertyHouseTileGood(this):downgradeablePropertyTileSearchingForHotel;
            default:
                PropertyTile downgradeablePropertyTileSearchingForHouse = GetAllGroupOfThisPropertyTile()
                    .FirstOrDefault(tile => tile.CanBuildBeDowngraded() && tile != this);
                return downgradeablePropertyTileSearchingForHouse==null ? base.GetMinimumGoodToSell() : null; // null parce que les autres recherches maineront à lui
        }
    }

    public int GetHousesNumber()
    {
        return propertyTileState.GetHousesNumber();
    }

    public int GetHotelNumber()
    {
        return propertyTileState.GetHotelNumber();
    }

    public void BuildTileGood()
    {
        UpgradeGood();
    }
    
    public override string GetLevelText()
    {
        if (!IsOwned())
        {
            return "Unknown.";
        }
        var allGroupPossession = string.Join(", ", GetOwner().deck.GetAllGroupOfThisPropertyTile(GetTargetType()).Select(purchasableTile => purchasableTile.TileName));
        if (DoesAllGroupOfThisPropertyTileIsOwnedByTheSamePlayer())
        {
            switch (propertyTileState)
            {
                case PropertyTileStateWithNoHouse:
                    return $"Vous possédez tout le groupe. Vous possédez {allGroupPossession}.";
                case PropertyTileStateWithOneHotel:
                    return $"Vous possédez {propertyTileState.GetHotelNumber()} hotel.";
                case PropertyTileStateWithOneHouse:
                case PropertyTileStateWithTwoHouses:
                case PropertyTileStateWithThreeHouses:
                case PropertyTileStateWithFourHouses:
                    return $"Vous possédez {propertyTileState.GetHousesNumber()} maison(s).";
            }
            
        }
        else
        {
            return $"Vous ne possédez pas tout le groupe. Vous possédez {allGroupPossession}.";
        }

        throw new NotImplementedException();
    }

    public bool CheckIfOwnerDeckGroupHasNoHotel()
    {
        return GetOwner().deck.GetAllGroupOfThisPropertyTile(GetTargetType()).All(propertyTile =>
            ((PropertyTile)propertyTile).propertyTileState is PropertyTileStateWithOneHotel);
    }
    public bool CheckIfOwnerDeckHasThisGroup()
    {
        return monopolyGameManager.DoesAllGroupOfThisPropertyTileIsOwnedByTheSamePlayer(this); 
    }
}
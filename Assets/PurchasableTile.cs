using System;
using System.Collections.Generic;
using Monopoly;
using UnityEngine;

public abstract class PurchasableTile : BoardTile, IGood, IPurchasableTileLevel
{
    public int[] costs { get; }
    private int price { get; }
    public int mortgageCost { get; }
    public int mortgageFinishedCost { get; private set;  }
    public bool IsMortgaged { get; private set;  }
    public List<TileGood> TileGoods { get; private set; }

    public string GetSellText()
    {
        return $"a hypothéqué {TileName}";
    }
    public MonopolyPlayer monopolyPlayer { get; private set; }
    public override bool CanBeBought()
    {
        return monopolyPlayer == null;
    }
    
    public override int getPrice()
    {
        return price;
    }
    private PurchasableTile(GameObject tileGameObject, string name, int[] costs, int price,
        int mortgageCost, int mortgageFinishedCost, int index, int groupIndex)
        : base(tileGameObject, name, index, groupIndex)
    {
        this.costs = costs;
        this.price = price;
        this.mortgageCost = mortgageCost;
        this.mortgageFinishedCost = mortgageFinishedCost;
        TileGoods = new List<TileGood>();
        IsMortgaged = false;
    }

    public abstract PurchasableFaceCard GetFaceCard();
    public abstract PurchasableBehindCard GetBehindCard();
    protected PurchasableTile(GameObject tileGameObject, string name, int[] costs, int price, int index, int groupIndex)
        : this(tileGameObject, name, costs, price, price / 2, price / 2 + (int)Math.Round(price / 20.0), index, groupIndex)
    {
    }
    
    public bool IsOwned()
    {
        return monopolyPlayer != null;
    }
    
    public bool IsFullyOwned()
    {
        return IsOwned() && !IsMortgaged;
    }

    public abstract Type GetTargetType();
    public virtual int GetLevel()
    {
        if (!IsOwned())
        {
            return -1;
        }
        return GetLevelOnTypeCount();
    }
    public virtual int GetLevelOnTypeCount()
    {
        return monopolyGameManager.GetAllGroupOfThisPropertyTileIsOwnedByTheSamePlayer(this) - 1;
    }
    public bool IsOwnedBy(MonopolyPlayer testMonopolyPlayer)
    {
        return monopolyPlayer == testMonopolyPlayer;
    }


    public MonopolyPlayer GetOwner()
    {
        return monopolyPlayer;
    }

    public virtual IGood GetMinimumGoodToSell()
    {
        return IsFullyOwned()? this:null;
    }

    public int GetSellPrice()
    {
        return mortgageCost;
    }

    public int Sell()
    {
        IsMortgaged = true;
        return GetSellPrice();
    }

    public bool CanBeSelled()
    {
        return !IsMortgaged;
    }

    public PurchasableTile GetPurchasableTile()
    {
        return this;
    }

    public string GetGoodName()
    {
        return $"Hypothèque sur {this}";
    }

    public void AssignOwner(MonopolyPlayer newOwner)
    {
        Debug.Assert(CanBeBought()||newOwner==monopolyPlayer, $"You can't assign owner to {this} tile");
        monopolyPlayer = newOwner;
        monopolyPlayer.deck.Add(this);
    }

    public virtual void RemoveOwner()
    {
        IsMortgaged = false;
        monopolyPlayer = null;
    }

    public bool CanCurrentTileGoodBeSelled()
    {
        return HaveTilesGood();
    }

    public virtual bool HaveTilesGood()
    {
        return false;
    }

    public virtual bool CanBeUpgraded()
    {
        return false;
    }

    public virtual int GetUpgradePrice()
    {
        return -1;
    }

    public int GetLevelCost()
    {
        return costs[GetLevel()];
    }

    public virtual string GetLevelText()
    {
        return IsOwned()? $"vous en possédez {GetLevel() + 1} au total":"Inconnu";
    }

    public virtual string GetCostText()
    {
        return GetLevelCost().ToString()+'M';
    }

}
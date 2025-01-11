using UnityEngine;

public class TaxTile : BoardTile
{
    public int taxAmount { get; }

    public TaxTile(GameObject tileGameObject, string name, int taxAmount, int index, int groupIndex)
        : base(tileGameObject, name, index, groupIndex)
    {
        this.taxAmount = taxAmount;
    }

}
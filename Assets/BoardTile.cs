using Monopoly;
using UnityEngine;

public class BoardTile
{
    
    public int groupIndex{ get; private set; }
    public int index { get; }
    public string TileName { get; private set; }
    public GameObject tileGameObject { get; }
    public MonopolyGameManager monopolyGameManager { get; private set; }
    public BoardTile(GameObject tileGameObject, string name, int index, int groupIndex)
    {
        this.tileGameObject = tileGameObject;
        monopolyGameManager = tileGameObject.transform.parent.GetComponent<MonopolyGameManager>();
        TileName = name;
        this.index = index;
        this.groupIndex = groupIndex;
    }

    public int GetTileIndex()
    {
        return index;
    }
    public Transform getTransform()
    {
        return tileGameObject.transform;
    }

    public virtual bool CanBeBought()
    {
        return false;
    }

    public virtual int getPrice()
    {
        return 0;
    }
    public void OnPlayerLanded(MonopolyPlayer player)
    {
        // Debug.Log($"Player {player} landed on {TileName}.");
    }

}
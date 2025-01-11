using UnityEngine;

public abstract class SpecialBoardTile : BoardTile
{
    
    protected SpecialBoardTile(GameObject tileGameObject, string name, int index, int groupIndex): base(tileGameObject, name, index, groupIndex)
    {
    }

}
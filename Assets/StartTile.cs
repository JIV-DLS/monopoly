using UnityEngine;

public class StartTile : CornerTile
{
    public static int GetStartReward(){
        return 200;
    }
    public StartTile(GameObject tileGameObject, int index)
        : base(tileGameObject, "Depart", index, 0)
    {
    }

}
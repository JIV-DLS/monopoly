using UnityEngine;

public class PlaceValue : BaseTextHandlerWithMonopolyPlayer
{
    public void setTile(BoardTile tile)
    {
        textComponent.text = tile.TileName;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using Monopoly;
using UnityEngine;

public class GameCardBuild : GameCardLayout
{
    
    public GameCardBuildActions actions; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ShowPurchasableCard(PurchasableTile purchasableTile, 
        MonopolyPlayer monopolyPlayer)
    {
        Show(purchasableTile, monopolyPlayer);
        actions.ShowPurchasableTile(purchasableTile, monopolyPlayer);
    }
}

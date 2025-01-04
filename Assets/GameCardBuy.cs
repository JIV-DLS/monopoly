using UnityEngine;
using UnityEngine.PlayerLoop;

public class GameCardBuy : MonoBehaviour
{
    public GameCardBuyPurchasable purchasable; 
    public GameCardBuyActions actions; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowPurchasableTile(PurchasableTile purchasableTile)
    {
        purchasable.ShowPurchasableTile(purchasableTile);
        actions.ShowPurchasableTile(purchasableTile);
    }
}

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

    public void Hide()
    {
        gameObject.SetActive(false);
    }
    public void ShowPurchasableTile(PurchasableTile purchasableTile, MonopolyPlayer monopolyPlayer)
    {
        gameObject.SetActive(true);   
        purchasable.ShowPurchasableTile(purchasableTile, monopolyPlayer);
        actions.ShowPurchasableTile(purchasableTile, monopolyPlayer);
    }
}

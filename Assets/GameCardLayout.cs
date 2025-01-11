using Monopoly;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class GameCardLayout : MonoBehaviour
{
    public GameCardBuyPurchasable purchasable; 
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
    public void Show(PurchasableTile purchasableTile, MonopolyPlayer monopolyPlayer)
    {
        gameObject.SetActive(true);   
        purchasable.ShowPurchasableTile(purchasableTile, monopolyPlayer);
    }
}

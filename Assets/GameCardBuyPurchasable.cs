using UnityEngine;

public class GameCardBuyPurchasable : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ReplaceChildren(PurchasableTile purchasableTile)
    {
        // Remove all children of the parent GameObject
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // Create and add the new child
        PurchasableFaceCard purchasableFaceCard = (PurchasableFaceCard)purchasableTile.GetFaceCard().Clone();
        purchasableFaceCard.transform.SetParent(transform, false); // Maintain local transform settings
        purchasableFaceCard.Show();
    }
    public void ShowPurchasableTile(PurchasableTile purchasableTile)
    {
        ReplaceChildren(purchasableTile);
    }
}

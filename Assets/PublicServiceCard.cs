using UnityEngine;

public class PublicServiceCard : PurchasableFaceCard
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    protected override void Init()
    {
        base.Init();
    }

    public PublicServiceCard Clone (PurchasableTile purchasableTile)
    {
        return Clone().UpdateTile(purchasableTile);
    }
    public PublicServiceCard Clone ()
    {
        return base.Clone<PublicServiceCard>();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    
    public PublicServiceCard UpdateTile(PurchasableTile purchasableTile)
    {
        base.UpdateTile(purchasableTile);
        return this;
    }
}

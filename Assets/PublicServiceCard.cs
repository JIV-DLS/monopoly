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

    
    
    public override PurchasableCard Clone (PurchasableTile purchasableTile)
    {
        return ((PublicServiceCard)Clone()).UpdateTile((PublicServiceTile)purchasableTile);
    }
    public PurchasableCard Clone ()
    {
        return base.Clone<PublicServiceCard>();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    
    public PublicServiceCard UpdateTile(PublicServiceTile publicServiceTile)
    {
        base.UpdateTile(publicServiceTile);
        return this;
    }
}

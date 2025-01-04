using UnityEngine;
using UnityEngine.UIElements;

public class PublicServiceCard : PurchasableFaceCard
{
    private BaseTextHandler _publicServiceNameValue;
    private BaseImageHandler _serviceImageNameValue;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    protected override void Init()
    {
        base.Init();
        _publicServiceNameValue = ChildUtility.GetChildComponentByName<BaseTextHandler>(transform, "PublicServiceNameValue");
        if (_publicServiceNameValue == null)
        {
            Debug.LogError("public service is null");
        }
        _serviceImageNameValue = ChildUtility.GetChildComponentByName<BaseImageHandler>(transform, "ServiceImage");
        if (_serviceImageNameValue == null)
        {
            Debug.LogError("service image is null");
        }
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
        _publicServiceNameValue.SetText(publicServiceTile.TileName);
        _serviceImageNameValue.UpdateImage(publicServiceTile.GetImageSprite());
        return this;
    }
}

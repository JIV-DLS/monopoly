using UnityEngine;
using UnityEngine.UI; // Required for the Image component

public class TitleValuePlayerPosition : BaseTextHandler
{
    
    private Image _tileValuePositionImage;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OtherInit()
    {
        base.OtherInit();
        _tileValuePositionImage = ChildUtility.GetChildComponentByName<Image>(transform, "Image");
        if (_tileValuePositionImage == null)
        {
            Debug.LogError("Title value position image value not found");
        }
        
    }

    public void ClearPlayerPosition()
    {
        _tileValuePositionImage.sprite = null;
    }
    public void SetPlayerPosition(Sprite playerSprite)
    {
        _tileValuePositionImage.sprite = playerSprite;
    }
}

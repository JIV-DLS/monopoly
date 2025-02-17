using System;
using UnityEngine;
using UnityEngine.UI;

public class BaseImageHandler : MonoBehaviourWithInitComponent
{
    
    protected Image image;

    private void Awake()
    {
        Init();
    }

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
        
        // Get the Text component
        image = GetComponentInChildren<Image>(true);

        if (image == null)
        {
            Debug.LogError("Image component not found!");
        }
    }

    public Sprite GetSprite()
    {
        return image.sprite;
    }
    public Sprite SetSprite(Sprite sprite)
    {
        image.sprite = sprite;
        return sprite;
    }
    public void UpdateImage(Sprite SpriteToSet)
    {
        image.sprite = SpriteToSet;
    }
}

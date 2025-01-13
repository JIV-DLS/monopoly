using System.Collections.Generic;
using UnityEngine;

public class PlayerImageChooser : BaseImageHandler
{
    public List<Sprite> chosenImages = new List<Sprite>();
    private int _currentIndex = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (chosenImages.Count == 0)
        {
            Debug.LogWarning("No images in the chosenImages list.");
        }
        // SetCurrentIndex(0);
        image.sprite = chosenImages[_currentIndex];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void SetNext()
    {
        image.sprite = Next();
    }
    private Sprite Next()
    {
        if (chosenImages.Count == 0)
        {
            Debug.LogWarning("No images to select from.");
            return null;
        }

        SetCurrentIndex((_currentIndex + 1) % chosenImages.Count);
        return chosenImages[_currentIndex];
    }

    protected virtual void SetCurrentIndex(int chosenImagesCount)
    {
        _currentIndex = chosenImagesCount;
    }

    public void SetPrevious()
    {
        image.sprite = Previous();
    }
    private Sprite Previous()
    {
        if (chosenImages.Count == 0)
        {
            Debug.LogWarning("No images to select from.");
            return null;
        }

        SetCurrentIndex((_currentIndex - 1 + chosenImages.Count) % chosenImages.Count);
        return chosenImages[_currentIndex];
    }


    public void SetImageFromIndex(int playerCustomProperty)
    {
        SetCurrentIndex(playerCustomProperty);
        image.sprite = chosenImages[_currentIndex];
    }

    public Sprite SpriteAt(int index)
    {
        return chosenImages[index];
    }
}

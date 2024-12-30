using UnityEngine;

public class PlaceButtonAction : ButtonHandlerWithSelfMadePlayer
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Method called when the button is clicked
    protected override void OnButtonClick()
    {
        Debug.Log("Place button clicked!");
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}

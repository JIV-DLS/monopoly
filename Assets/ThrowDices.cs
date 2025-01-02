using UnityEngine;

public class ThrowDices : ButtonHandlerWithMonopolyPlayer
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Method called when the button is clicked
    protected override void OnButtonClick()
    {
        SelfMadePlayer.AskPlayFromButton();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}

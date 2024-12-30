using UnityEngine;

public class ThrowDices : ButtonHandlerWithSelfMadePlayer
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Method called when the button is clicked
    protected override void OnButtonClick()
    {
        Debug.Log("Throw dices clicked!");
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}

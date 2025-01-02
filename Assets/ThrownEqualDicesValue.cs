using UnityEngine;

public class ThrownEqualDicesValue : BaseTextHandlerWithMonopolyPlayer
{
    private int numberOfEqualThrown = 0;
    public void Happened()
    {
        numberOfEqualThrown++;
        if (numberOfEqualThrown == 3)
        {
            numberOfEqualThrown = 0;
        }
        textComponent.text = numberOfEqualThrown.ToString();

    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

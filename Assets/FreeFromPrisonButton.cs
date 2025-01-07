using UnityEngine;

public class FreeFromPrisonButton : ButtonHandler {
    public NumberOfFreeCardScript NumberOfFreeCardScript;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void SetCardCanBeUsedInFuture(int numberOfFreeCards)
    {   
        NumberOfFreeCardScript.SetNumberOfFreeCard(numberOfFreeCards);
        if (numberOfFreeCards < 1)
        {
            SetButtonInteractable(false);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}

using UnityEngine;
using System.Linq;
public class DicesManager : MonoBehaviour
{
    // Create a public array of the custom class
    private DiceRoller[] dices;
    public int dicesTargetNumber=2;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        dices = new DiceRoller[dicesTargetNumber];
        // Get the first child that has MyCustomClass attached
        DiceRoller initialDieRoller = GetComponentInChildren<DiceRoller>();

        if (initialDieRoller != null)
        {
            // Do something with the single child
            Debug.Log($"Found single child: {initialDieRoller.gameObject.name}");
        }
        else
        {
            Debug.Log("No child with MyCustomClass found.");
        }

        dices[0] = initialDieRoller;
        for (int i = 1; i < dicesTargetNumber; i++)
        {
            dices[i] = initialDieRoller.GetNewer(i);
        }
    }

    public void ThrowDice()
    {
        if (dices != null && dices.All(die => die.CanBeThrown())) // Check for null and that all dices can be thrown
        {
            for (int i = 0; i < dices.Length; i++) // Iterate through all dices
            {
                dices[i].ThrowDice(); // Throw each dice
            }
        }
        else
        {
            Debug.Log("Not all dices can be thrown.");
        }
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}

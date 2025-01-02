using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Threading;
public class DicesManager : MonoBehaviour
{
    // Create a public array of the custom class
    public MonopolyGameManager monopolyGameManager;
    private MonopolyPlayer playerWaitingForResult;
    private static readonly object lockObject = new object();

    private List<DiceRoller> dices = new List<DiceRoller>();
    private HashSet<DiceRoller> dicesResponses = new HashSet<DiceRoller>();
    public int dicesTargetNumber=2;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
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

        dices.Add(initialDieRoller);
        for (int i = 1; i < dicesTargetNumber; i++)
        {
            dices.Add(initialDieRoller.GetNewer(i));
        }
    }

    // Method called by children
    public void NotifyResponse(DiceRoller diceRoller)
    {
        lock (lockObject)
        {
            if (dices.Contains(diceRoller))
                    {
                        dicesResponses.Add(diceRoller);
                        // Debug.Log($"Received response from: {diceRoller}");
                        
                        // Check if all responses are received
                        if (dicesResponses.SetEquals(dices))
                        {
                            AllResponsesReceived();
                        }
                    }
        }
        
    }

    private void AllResponsesReceived()
    {
        try
        {
            if (playerWaitingForResult!=null)
            {
                // Code that might throw an exception
                monopolyGameManager.DicesRoll(playerWaitingForResult,
                    dicesResponses.Sum(die => die.LastRoll()), 
                    dicesResponses.All(die=>die.LastRoll()==dicesResponses.First().LastRoll()));
                playerWaitingForResult = null;
            }
        }
        catch (Exception ex)
        {
            // Code that runs if an exception occurs
            // You can access the exception details through 'ex'
            Debug.Log($"An error occurred: {ex.Message} {ex.StackTrace}");
        }
        dicesResponses.Clear();
        // Perform the next action here
    }
    public void ThrowDice(MonopolyPlayer player)
    {
        //Debug.Log("thrown");
        playerWaitingForResult = player;
        if (dices != null && dices.All(die => die.CanBeThrown())) // Check for null and that all dices can be thrown
        {
            dices.ForEach(die=>die.ThrowDice());
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

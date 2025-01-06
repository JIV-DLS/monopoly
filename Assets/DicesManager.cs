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

    public IEnumerable<int> RollDiceAndGetResult(MonopolyPlayer monopolyPlayer)
    {
        List<int> rolledResult = new List<int>();
        foreach (List<int> rolledResultFromEnumerator in AskAPlayerToRollDices(
                     monopolyPlayer))
        {
            yield return 0;
            rolledResult = rolledResultFromEnumerator;
        }

        yield return rolledResult.Sum();
    }
    public IEnumerable<List<int>> AskAPlayerToRollDices(MonopolyPlayer monopolyPlayer)
    {
        IEnumerator<List<int>> rollDicesAndGetResult = RollDicesAndGetResult();
        while (rollDicesAndGetResult.MoveNext())
        {
            yield return rollDicesAndGetResult.Current;
        }
    }
    public IEnumerator<List<int>> RollDicesAndGetResult()
    {

        // Create an array of enumerators for each DiceRoller
        List<IEnumerator<int>> rollEnumerators = new List<IEnumerator<int>>();
        foreach (var dice in dices)
        {
            rollEnumerators.Add(dice.Roll());
        }

        // Yield initial results (0) from all dice
        List<int> initialResults = new List<int>();
        foreach (var enumerator in rollEnumerators)
        {
            // Initially yield 0 for all dice
            if (enumerator.MoveNext()) initialResults.Add(enumerator.Current);
        }
        yield return initialResults;

        // Simulate a delay before rolling
        yield return null;

        // Now gather the final results
        List<int> finalResults = new List<int>();
        bool allDone = false;

        while (!allDone)
        {
            allDone = true;
            foreach (var enumerator in rollEnumerators)
            {
                if (enumerator.MoveNext())
                {
                    // Continue rolling, but only add final result once finished
                    allDone = false; // At least one die is still rolling
                }
                else
                {
                    // Only add the final result (after MoveNext() returns false)
                    finalResults.Add(enumerator.Current);
                }
            }

            if (!allDone)
            {
                // Yield again until all dice are rolled
                yield return null;
            }
        }

        // After all dice rolls are completed, yield the final results
        yield return finalResults;
    }
}

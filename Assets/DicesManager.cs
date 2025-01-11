using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Threading;
using Monopoly;

public class DicesManager : MonoBehaviour
{

    private readonly List<DiceRoller> _dices = new List<DiceRoller>();
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

        _dices.Add(initialDieRoller);
        for (int i = 1; i < dicesTargetNumber; i++)
        {
            _dices.Add(initialDieRoller.GetNewer(i));
        }
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
        
        int sumResult = rolledResult.Sum();

        yield return sumResult;
    }
    public IEnumerable<List<int>> AskAPlayerToRollDices(MonopolyPlayer monopolyPlayer)
    {
        IEnumerator<List<int>> rollDicesAndGetResult = RollDicesAndGetResult();
        while (rollDicesAndGetResult.MoveNext())
        {
            yield return rollDicesAndGetResult.Current;
        }
        yield return rollDicesAndGetResult.Current;
    }
    public IEnumerator<List<int>> RollDicesAndGetResult()
    {
        // Create a dictionary to track each dice roll and its corresponding result.
        Dictionary<IEnumerator<int>, int> rollResults = new Dictionary<IEnumerator<int>, int>();

        // Initialize the roll enumerators for each dice.
        List<IEnumerator<int>> rollEnumerators = new List<IEnumerator<int>>();
        foreach (var dice in _dices)
        {
            var enumerator = dice.Roll();
            rollEnumerators.Add(enumerator);
            rollResults[enumerator] = 0; // Initialize the results as 0 for each dice.
        }

        // Yield initial results (0) from all dice
        List<int> initialResults = rollEnumerators.Select(enumerator => rollResults[enumerator]).ToList();
        yield return initialResults;

        // Simulate a delay before rolling
        yield return null;

        // Now gather the final results
        bool allDone = false;

        while (!allDone)
        {
            allDone = true;
        
            // Loop over all enumerators and move them forward if possible
            foreach (var enumerator in rollEnumerators)
            {
                if (enumerator.MoveNext())
                {
                    // At least one die is still rolling
                    allDone = false;
                }
                else
                {
                    // Add final result once the enumerator has finished
                    rollResults[enumerator] = enumerator.Current;
                }
            }

            // Yield again if some dice are still rolling
            if (!allDone)
            {
                yield return null;
            }
        }

        // After all dice rolls are completed, yield the final results
        List<int> finalResults = rollEnumerators.Select(enumerator => rollResults[enumerator]).ToList();
        yield return finalResults;
    }
}

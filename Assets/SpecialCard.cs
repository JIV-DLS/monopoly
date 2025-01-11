using System.Collections;
using UnityEngine;

public abstract class SpecialCard
{
    
    public MonopolyGameManager monopolyGameManager { get; }
    public string name { get; }
    public string description { get; }

    protected SpecialCard(MonopolyGameManager monopolyGameManager, string name, string description)
    {
        this.monopolyGameManager = monopolyGameManager;
        this.name = name;
        this.description = description;
    }

    public IEnumerator TriggerEffectMain(MonopolyPlayer monopolyPlayer)
    {
        Debug.Log($"Effet de carte : {description}");
        
        monopolyGameManager.SetGameTextEventsText($"{monopolyPlayer.name} est arriver sur une tuile {name}.");
        yield return new WaitForSeconds(.9f);
        monopolyGameManager.SetGameTextEventsText($"Effet de carte : {description}");
        yield return new WaitForSeconds(1.5f);
        yield return TriggerEffect(monopolyPlayer);
    }
    protected abstract IEnumerator TriggerEffect(MonopolyPlayer monopolyPlayer);
}
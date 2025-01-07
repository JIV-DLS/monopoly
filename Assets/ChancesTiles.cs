using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;


public class ShuffableCollection<T>
{
    // Internal collection of T instances
    private List<T> collection;

    private static readonly System.Random Random = new System.Random();

    public void AddRange(IEnumerable<T> foreignCollection)
    {
        this.collection.AddRange(foreignCollection);
    }
    // Constructor
    public ShuffableCollection()
    {
        collection = new List<T>();
    }
    // Add an item to the end of the collection
    public void AddToEnd(T item)
    {
        collection.Add(item);
    }

    // Get an item from the start of the collection
    public T GetFromStart()
    {
        if (collection.Count == 0)
            throw new InvalidOperationException("The collection is empty.");

        return collection[0];
    }
    // Take an item from the start of the collection
    public T TakeFromStart()
    {
        if (collection.Count == 0)
            throw new InvalidOperationException("The collection is empty.");

        var firstItem = collection[0];
        collection.RemoveAt(0);
        return firstItem;
    }
    // Add an item to the collection
    public void Add(T item)
    {
        collection.Add(item);
    }

    // Shuffle the collection using Fisher-Yates algorithm
    public void Shuffle()
    {
        for (int i = collection.Count - 1; i > 0; i--)
        {
            int j = Random.Next(i + 1);
            (collection[i], collection[j]) = (collection[j], collection[i]);
        }
    }

    // Rotate the collection (move first to last)
    public void Rotate()
    {
        if (collection.Count > 1)
        {
            var first = collection[0];
            collection.RemoveAt(0);
            collection.Add(first);
        }
    }

    // Access the collection (read-only)
    public IReadOnlyList<T> GetCollection()
    {
        return collection.AsReadOnly();
    }
}

public class ElectedChairmanCard : ChanceCard
{
    public ElectedChairmanCard(MonopolyGameManager monopolyGameManager)
        : base(monopolyGameManager, "VOUS AVEZ ÉTÉ ELU RESPONSABLE DU CONSEIL D'ADMINISTRATION. PAYEZ À CHAQUE JOUEUR 50M.")
    {
    }

    protected override IEnumerator TriggerEffect(MonopolyPlayer monopolyPlayer)
    {
        
        monopolyGameManager.SetGameTextEventsText(
            $"{monopolyPlayer} doit payer 50M à chaque joueur.");
        yield return monopolyGameManager.PlayerMustPayToEachPlayer(monopolyPlayer, 50);
        yield return new WaitForSeconds(1f);
    }
}
public class AdvanceToBoulevardDeLaVilletteCard : ChanceCard
{
    public AdvanceToBoulevardDeLaVilletteCard(MonopolyGameManager tileMonopolyGameManager)
        : base(tileMonopolyGameManager, "AVANCEZ JUSQU'AU BOULEVARD DE LA VILLETE. SI VOUS PASSEZ PAR LA CASE DÉPART, RECEVEZ 200M.")
    {
    }

    
    protected override IEnumerator TriggerEffect(MonopolyPlayer monopolyPlayer)
    {
        
        
        monopolyGameManager.SetGameTextEventsText($"{monopolyPlayer} doit se déplacer au boulevard de la villette.");
        yield return monopolyGameManager.MoveAPlayerToATile(monopolyPlayer, 11);
        yield return new WaitForSeconds(.5f);
    }
}
public class GetOutOfJailCard : ChanceCard
{
    public GetOutOfJailCard(MonopolyGameManager tileMonopolyGameManager)
        : base(tileMonopolyGameManager, "VOUS ÊTES LIBÉRÉ DE PRISON. Cette carte peut être conservée jusqu'à ce qu'elle soit utilisée, échangée ou vendue.")
    {
    }

    protected override IEnumerator TriggerEffect(MonopolyPlayer monopolyPlayer)
    {
        
        
        monopolyGameManager.SetGameTextEventsText($"{monopolyPlayer} a la possibilité de sortir en prison si il y va.");
        yield return monopolyGameManager.GiveChanceCardToPlayerGetOutOfJailCard(monopolyPlayer);
        yield return new WaitForSeconds(.5f);
    }
}
public class MoveBackThreeSpacesCard : ChanceCard
{
    public MoveBackThreeSpacesCard(MonopolyGameManager tileMonopolyGameManager)
        : base(tileMonopolyGameManager, "RECULEZ DE TROIS CASES.")
    {
    }

    
    protected override IEnumerator TriggerEffect(MonopolyPlayer monopolyPlayer)
    {
        
        
        monopolyGameManager.SetGameTextEventsText($"{monopolyPlayer} doit reculer de 3 cases.");
        yield return monopolyGameManager.MoveBackTo(monopolyPlayer, 3);
        yield return new WaitForSeconds(.5f);
    }
}
public class RealEstateLoanCard : ChanceCard
{
    public RealEstateLoanCard(MonopolyGameManager tileMonopolyGameManager)
        : base(tileMonopolyGameManager, "VOTRE PRÊT IMMOBILIER VOUS RAPPORTE. RECEVEZ 150M.")
    {
    }


    protected override IEnumerator TriggerEffect(MonopolyPlayer monopolyPlayer)
    {
        
        monopolyPlayer.HaveWon(150);
        monopolyGameManager.SetGameTextEventsText(
            $"Votre prêt vous rapporte 150M à {monopolyPlayer}.");
        yield return new WaitForSeconds(1f);
    }
}
public class AdvanceToGareMontparnasseCard : ChanceCard
{
    public AdvanceToGareMontparnasseCard(MonopolyGameManager tileMonopolyGameManager)
        : base(tileMonopolyGameManager,"RENDEZ-VOUS À LA GARE MONTPARNASSE. SI VOUS PASSEZ PAR LA CASE DÉPART, RECEVEZ 200M.")
    {
    }

    protected override IEnumerator TriggerEffect(MonopolyPlayer monopolyPlayer)
    {
        
        
        monopolyGameManager.SetGameTextEventsText($"{monopolyPlayer} doit se déplacer à la GARE MONTPARNASSE.");
        yield return monopolyGameManager.MoveAPlayerToATile(monopolyPlayer, 11);
        yield return new WaitForSeconds(.5f);
    }
}
public class AdvanceToAvenueHenriMartinCard : ChanceCard
{
    public AdvanceToAvenueHenriMartinCard(MonopolyGameManager tileMonopolyGameManager)
        : base(tileMonopolyGameManager, "AVANCEZ JUSQU'À L'AVENUE HENRI-MARTIN. SI VOUS PASSEZ PAR LA CASE DÉPART, RECEVEZ 200M.")
    {
    }
    
    protected override IEnumerator TriggerEffect(MonopolyPlayer monopolyPlayer)
    {
        
        
        monopolyGameManager.SetGameTextEventsText($"{monopolyPlayer} doit se déplacer à l'avenue Henri Martin.");
        yield return monopolyGameManager.MoveAPlayerToATile(monopolyPlayer, 24);
        yield return new WaitForSeconds(.5f);
    }
}
public class GoToJailCard : ChanceCard
{
    public GoToJailCard(MonopolyGameManager tileMonopolyGameManager)
        : base(tileMonopolyGameManager, "ALLEZ EN PRISON. ALLEZ TOUT DROIT EN PRISON. NE PASSEZ PAS PAR LA CASE DÉPART. NE RECEVEZ PAS 200M.")
    {
    }

    protected override IEnumerator TriggerEffect(MonopolyPlayer monopolyPlayer)
    {
        monopolyGameManager.SetGameTextEventsText($"{monopolyPlayer} doit allez en prison.");
        yield return monopolyGameManager.PutPlayerIntoPrison(monopolyPlayer);
        yield return new WaitForSeconds(.5f);
    }
}
public class AdvanceToRueDeLaPaixCard : ChanceCard
{
    public AdvanceToRueDeLaPaixCard(MonopolyGameManager tileMonopolyGameManager)
        : base(tileMonopolyGameManager, "AVANCEZ JUSQU'À LA RUE DE LA PAIX.")
    {
    }


    protected override IEnumerator TriggerEffect(MonopolyPlayer monopolyPlayer)
    {
        monopolyGameManager.SetGameTextEventsText($"{monopolyPlayer} doit se déplacer à LA RUE DE LA PAIX.");
        yield return monopolyGameManager.MoveAPlayerToLastTile(monopolyPlayer);
        yield return new WaitForSeconds(.5f);
    }
}
public class AdvanceToStartCard : ChanceCard
{
    public AdvanceToStartCard(MonopolyGameManager tileMonopolyGameManager)
        : base(tileMonopolyGameManager, "AVANCEZ JUSQU'À LA CASE DÉPART. RECEVEZ 200M.")
    {
    }

    protected override IEnumerator TriggerEffect(MonopolyPlayer monopolyPlayer)
    {
        monopolyGameManager.SetGameTextEventsText($"{monopolyPlayer} doit se déplacer à la case départ.");
        yield return monopolyGameManager.MoveAPlayerToStartTile(monopolyPlayer);
        yield return new WaitForSeconds(.5f);

    }
}
public class RepairCostCard : ChanceCard
{
    public RepairCostCard(MonopolyGameManager tileMonopolyGameManager)
        : base(tileMonopolyGameManager, "FAITES DES RÉPARATIONS SUR TOUTES VOS PROPRIÉTÉS : POUR CHAQUE MAISON, PAYEZ 25M. POUR CHAQUE HÔTEL, PAYEZ 100M.")
    {
    }

    protected override IEnumerator TriggerEffect(MonopolyPlayer monopolyPlayer)
    {
        

        int houseRepairCost = 25;
        int hotelRepairCost = 100;
        int totalRepairCost;
        int housesNumber = monopolyPlayer.GetAllHousesNumber();
        int hotelsNumber = monopolyPlayer.GetAllHotelsNumber();
        if (housesNumber > 0 && hotelsNumber > 0)
        {
            monopolyGameManager.SetGameTextEventsText($"{monopolyPlayer} detient {housesNumber} maisons et {hotelsNumber} hôtels.");
        }
        else if (housesNumber > 0)
        {
            monopolyGameManager.SetGameTextEventsText($"{monopolyPlayer} detient {housesNumber} maisons.");
        }
        else if (hotelsNumber > 0)
        {
            monopolyGameManager.SetGameTextEventsText($"{monopolyPlayer} detient {hotelsNumber} hôtels.");
        }
        else
        {
            monopolyGameManager.SetGameTextEventsText($"{monopolyPlayer} ne detient aucune maison ni hôtel.");
        }
        
        yield return new WaitForSeconds(1.5f);
        
        if (housesNumber > 0 || hotelsNumber > 0)
        {
            totalRepairCost = houseRepairCost * housesNumber + hotelRepairCost * hotelsNumber;
            monopolyGameManager.SetGameTextEventsText($"{monopolyPlayer} doit payer {totalRepairCost}M à la banque.");
            yield return new WaitForSeconds(.5f);
            yield return monopolyGameManager.PlayerMustPayToBank(monopolyPlayer, totalRepairCost);
            yield return new WaitForSeconds(1.5f);
        }

    }
    
}
public class SpeedingFineCard : ChanceCard
{
    public SpeedingFineCard(MonopolyGameManager tileMonopolyGameManager)
        : base(tileMonopolyGameManager,"AMENDE POUR EXCÈS DE VITESSE. PAYEZ 15M.")
    {
    }

    
    protected override IEnumerator TriggerEffect(MonopolyPlayer monopolyPlayer)
    {
        
        monopolyGameManager.SetGameTextEventsText(
            $"Excès de vitesse {monopolyPlayer} doit payer 15M à la banque.");
        yield return monopolyGameManager.PlayerMustPayToBank(monopolyPlayer, 15);
        yield return new WaitForSeconds(1f);
    }
}
public class AdvanceToStationCardChance : ChanceCard
{
    public AdvanceToStationCardChance(MonopolyGameManager tileMonopolyGameManager)
        : base(tileMonopolyGameManager, "AVANCEZ JUQU'À LA PROCHAINE GARE. SI ELLE N'APPARTIENT À PERSONNE, vous pouvez l'acheter à la banque. SI ELLE APPARTIENT À UN JOUEUR, payez au propriétaire le double du loyer auquel il a droit. SI VOUS PASSEZ PAR LA CASE DÉPART, RECEVEZ 200M.")
    {
    }
    
    protected override IEnumerator TriggerEffect(MonopolyPlayer monopolyPlayer)
    {
        yield return monopolyGameManager.MoveAPlayerToNextType<RailroadTile>(monopolyPlayer);
        RailroadTile nextRailRoadTile = (RailroadTile)monopolyPlayer.tile;
        Debug.Assert(nextRailRoadTile!=null, "Next rail road not found");
        if ( nextRailRoadTile.IsOwned())
        {
            if(!nextRailRoadTile.IsOwnedBy(monopolyPlayer))
            {
                MonopolyPlayer tileOwner = nextRailRoadTile.GetOwner();
                monopolyGameManager.SetGameTextEventsText($"{nextRailRoadTile} est détenu par {tileOwner}");
                yield return new WaitForSeconds(1.5f);
                
                int dueAmount = nextRailRoadTile.GetCost() * 2;

                
                monopolyGameManager.SetGameTextEventsText(
                    $"{monopolyPlayer} doit payer {dueAmount} à {tileOwner}");
                yield return monopolyGameManager.PlayerAPayPlayerB(monopolyPlayer, tileOwner, dueAmount);
                yield return new WaitForSeconds(1f);
            }
            else
            {
                
                monopolyGameManager.SetGameTextEventsText(
                    $"{monopolyPlayer} possède {nextRailRoadTile}.");
                yield return new WaitForSeconds(1f);
            }
            

        }
        else
        {
            monopolyGameManager.SetGameTextEventsText(
                $"aucun joueur ne possède {nextRailRoadTile}.");
            yield return new WaitForSeconds(1f);
        }

        yield return null;
    }

}
public class BankDividendCard : ChanceCard
{
    public BankDividendCard(MonopolyGameManager tileMonopolyGameManager)
        : base(tileMonopolyGameManager, "La banque vous verse un dividende de 50M.")
    {
    }

    protected override IEnumerator TriggerEffect(MonopolyPlayer monopolyPlayer)
    {
        monopolyPlayer.HaveWon(50);
        monopolyGameManager.SetGameTextEventsText(
            $"La banque verse 50M à {monopolyPlayer}.");
        yield return new WaitForSeconds(1f);
    }
}
public class AdvanceToUtilityCard : ChanceCard
{
    public AdvanceToUtilityCard(MonopolyGameManager tileMonopolyGameManager) 
        : base(tileMonopolyGameManager,"Avancez jusqu'au prochain service public. S'il n'appartient à personne, vous pouvez l'acheter à la banque. S'il appartient à un joueur, lancez les dés et payez 10x le montant du résultat au propriétaire. Si vous passez par la case Départ, recevez 200M.")
    {
    }

    protected override IEnumerator TriggerEffect(MonopolyPlayer monopolyPlayer)
    {
        yield return monopolyGameManager.MoveAPlayerToNextType<PublicServiceTile>(monopolyPlayer);
        PublicServiceTile nextServiceTile = (PublicServiceTile)monopolyPlayer.tile;
        Debug.Assert(nextServiceTile!=null, "Next service not found");
        if ( nextServiceTile.IsOwned())
        {
            if(!nextServiceTile.IsOwnedBy(monopolyPlayer))
            {
                MonopolyPlayer tileOwner = nextServiceTile.GetOwner();
                monopolyGameManager.SetGameTextEventsText($"{nextServiceTile} est détenu par {tileOwner}");
                yield return new WaitForSeconds(1.5f);


                int rolledResult = 0;
                foreach (int gottenResult in monopolyGameManager.AskAPlayerToRollDices(
                             monopolyPlayer))
                {
                    yield return null;
                    rolledResult = gottenResult;
                    if (rolledResult>0)
                    {
                        break;
                    }
                }

                monopolyGameManager.SetGameTextEventsText($"{monopolyPlayer} a joué {rolledResult}");
                yield return new WaitForSeconds(1f);
                int dueAmount = rolledResult * 10;

                
                monopolyGameManager.SetGameTextEventsText(
                    $"{monopolyPlayer} a joué {rolledResult}, donc doit payer {dueAmount}");
                yield return monopolyGameManager.PlayerAPayPlayerB(monopolyPlayer, tileOwner, dueAmount);
                yield return new WaitForSeconds(1f);
            }
            else
            {
                
                monopolyGameManager.SetGameTextEventsText(
                    $"{monopolyPlayer} possède {nextServiceTile}.");
                yield return new WaitForSeconds(1f);
            }
            

        }
        else
        {
            monopolyGameManager.SetGameTextEventsText(
                $"aucun joueur ne possède {nextServiceTile}.");
            yield return new WaitForSeconds(1f);
        }

        yield return null;
    }

}
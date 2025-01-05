using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChancesTiles
{
    
}


public class ElectedChairmanCard : ChanceTile
{
    public ElectedChairmanCard()
        : base("VOUS AVEZ ÉTÉ ELU RESPONSABLE DU CONSEIL D'ADMINISTRATION. PAYEZ À CHAQUE JOUEUR 50M.")
    {
    }

    public override IEnumerator TriggerEffect(MonopolyPlayer MonopolyPlayer)
    {
        Debug.Log($"Effet de carte : {description}");

        // Récupérer la liste de tous les joueurs
        var allMonopolyPlayers = GameManager.Instance.GetAllMonopolyPlayers();
        int totalToPay = 0;

        foreach (var otherMonopolyPlayer in allMonopolyPlayers)
        {
            if (otherMonopolyPlayer != MonopolyPlayer)
            {
                MonopolyPlayer.Money -= 50;
                otherMonopolyPlayer.Money += 50;
                totalToPay += 50;

                Debug.Log($"{MonopolyPlayer.Name} paie 50M à {otherMonopolyPlayer.Name}.");
            }
        }

        Debug.Log($"{MonopolyPlayer.Name} a payé un total de {totalToPay}M aux autres joueurs.");
    }
}
public class AdvanceToBoulevardDeLaVilletteCard : ChanceTile
{
    public AdvanceToBoulevardDeLaVilletteCard()
        : base("AVANCEZ JUQU'AU BOULEVARD DE LA VILLETE. SI VOUS PASSEZ PAR LA CASE DÉPART, RECEVEZ 200M.")
    {
    }

    public override IEnumerator TriggerEffect(MonopolyPlayer MonopolyPlayer)
    {
        Debug.Log($"Effet de carte : {description}");

        // Position du Boulevard de la Villette
        int targetPosition = 11; // Exemple : index de la case sur le plateau
        bool passesStart = MonopolyPlayer.CurrentPosition > targetPosition;

        if (passesStart)
        {
            // Ajouter 200M pour avoir passé la case départ
            MonopolyPlayer.Money += 200;
            Debug.Log($"{MonopolyPlayer.Name} passe par la case départ et reçoit 200M.");
        }

        // Déplacer le joueur sur le Boulevard de la Villette
        MonopolyPlayer.MoveToPosition(targetPosition);

        Debug.Log($"{MonopolyPlayer.Name} avance jusqu'au Boulevard de la Villette.");
    }
}
public class GetOutOfJailCard : ChanceTile
{
    public GetOutOfJailCard()
        : base("VOUS ÊTES LIBÉRÉ DE PRISON. Cette carte peut être conservée jusqu'à ce qu'elle soit utilisée, échangée ou vendue.")
    {
    }

    public override IEnumerator TriggerEffect(MonopolyPlayer MonopolyPlayer)
    {
        Debug.Log($"Effet de carte : {description}");

        // Ajouter cette carte à l'inventaire du joueur
        MonopolyPlayer.AddToInventory(this);

        Debug.Log($"{MonopolyPlayer.Name} conserve la carte 'Libéré de prison' pour une utilisation future.");
    }
}
public class MoveBackThreeSpacesCard : ChanceTile
{
    public MoveBackThreeSpacesCard()
        : base("RECULEZ DE TROIS CASES.")
    {
    }

    public override IEnumerator TriggerEffect(MonopolyPlayer MonopolyPlayer)
    {
        Debug.Log($"Effet de carte : {description}");

        // Le joueur recule de trois cases
        int newPosition = MonopolyPlayer.CurrentPosition - 3;

        // S'assurer que la position ne soit pas inférieure à zéro (ne pas sortir du plateau)
        if (newPosition < 0)
        {
            newPosition = 0; // La première case du plateau
        }

        MonopolyPlayer.MoveToPosition(newPosition);

        Debug.Log($"{MonopolyPlayer.Name} recule de trois cases.");
    }
}
public class RealEstateLoanCard : ChanceTile
{
    public RealEstateLoanCard()
        : base("VOTRE PRÊT IMMOBILIER VOUS RAPPORTE. RECEVEZ 150M.")
    {
    }

    public override IEnumerator TriggerEffect(MonopolyPlayer MonopolyPlayer)
    {
        Debug.Log($"Effet de carte : {description}");

        // Le joueur reçoit 150 millions
        MonopolyPlayer.Money += 150;
        
        Debug.Log($"{MonopolyPlayer.Name} reçoit 150M grâce à son prêt immobilier.");
    }
}
public class AdvanceToGareMontparnasseCard : ChanceTile
{
    public AdvanceToGareMontparnasseCard()
        : base("RENDEZ-VOUS À LA GARE MONTPARNASSE. SI VOUS PASSEZ PAR LA CASE DÉPART, RECEVEZ 200M.")
    {
    }

    public override IEnumerator TriggerEffect(MonopolyPlayer MonopolyPlayer)
    {
        Debug.Log($"Effet de carte : {description}");

        // Supposons que la Gare Montparnasse est à la position 15
        int gareMontparnassePosition = 15;
        int currentPosition = MonopolyPlayer.CurrentPosition;
        
        // Le joueur se déplace à la Gare Montparnasse
        MonopolyPlayer.MoveToPosition(gareMontparnassePosition);

        // Vérifier si le joueur est passé par la case Départ
        if (currentPosition < gareMontparnassePosition)
        {
            MonopolyPlayer.Money += 200; // Le joueur reçoit 200 millions s'il passe par la case Départ
            Debug.Log($"{MonopolyPlayer.Name} passe par la case Départ et reçoit 200M.");
        }

        Debug.Log($"{MonopolyPlayer.Name} avance à la Gare Montparnasse.");
    }
}
public class AdvanceToAvenueHenriMartinCard : ChanceTile
{
    public AdvanceToAvenueHenriMartinCard()
        : base("AVANCEZ JUSQU'À L'AVENUE HENRI-MARTIN. SI VOUS PASSEZ PAR LA CASE DÉPART, RECEVEZ 200M.")
    {
    }

    public override IEnumerator TriggerEffect(MonopolyPlayer MonopolyPlayer)
    {
        Debug.Log($"Effet de carte : {description}");

        // Supposons que l'Avenue Henri-Martin est à la position 17
        int avenueHenriMartinPosition = 17;
        int currentPosition = MonopolyPlayer.CurrentPosition;
        
        // Le joueur se déplace à l'Avenue Henri-Martin
        MonopolyPlayer.MoveToPosition(avenueHenriMartinPosition);

        // Vérifier si le joueur est passé par la case Départ
        if (currentPosition < avenueHenriMartinPosition)
        {
            MonopolyPlayer.Money += 200; // Le joueur reçoit 200 millions s'il passe par la case Départ
            Debug.Log($"{MonopolyPlayer.Name} passe par la case Départ et reçoit 200M.");
        }

        Debug.Log($"{MonopolyPlayer.Name} avance à l'Avenue Henri-Martin.");
    }
}
public class GoToJailCard : ChanceTile
{
    public GoToJailCard(GameObject tileGameObject)
        : base(tileGameObject, "ALLEZ EN PRISON. ALLEZ TOUT DROIT EN PRISON. NE PASSEZ PAS PAR LA CASE DÉPART. NE RECEVEZ PAS 200M.")
    {
    }

    public override IEnumerator TriggerEffect(MonopolyPlayer monopolyPlayer)
    {
        Debug.Log($"Effet de carte : {description}");
        
        monopolyGameManager.SetGameTextEventsText($"{monopolyPlayer} doit allez en prison.");
        yield return monopolyGameManager.PutPlayerIntoPrison(monopolyPlayer);
        yield return new WaitForSeconds(.5f);
    }
}
public class AdvanceToRueDeLaPaixCard : ChanceTile
{
    public AdvanceToRueDeLaPaixCard(GameObject tileGameObject)
        : base(tileGameObject, "AVANCEZ JUSQU'À LA RUE DE LA PAIX.")
    {
    }


    public override IEnumerator TriggerEffect(MonopolyPlayer monopolyPlayer)
    {
        Debug.Log($"Effet de carte : {description}");
        
        monopolyGameManager.SetGameTextEventsText($"{monopolyPlayer} doit se déplacer à la case départ.");
        yield return monopolyGameManager.MoveAPlayerToLastTile(monopolyPlayer);
        yield return new WaitForSeconds(.5f);

    }
}
public class AdvanceToStartCard : ChanceTile
{
    public AdvanceToStartCard(GameObject tileGameObject)
        : base(tileGameObject, "AVANCEZ JUSQU'À LA CASE DÉPART. RECEVEZ 200M.")
    {
    }

    public override IEnumerator TriggerEffect(MonopolyPlayer monopolyPlayer)
    {
        Debug.Log($"Effet de carte : {description}");
        
        monopolyGameManager.SetGameTextEventsText($"{monopolyPlayer} doit se déplacer à la case départ.");
        yield return monopolyGameManager.MoveAPlayerToStartTile(monopolyPlayer);
        yield return new WaitForSeconds(.5f);

    }
}
public class RepairCostCard : ChanceTile
{
    public RepairCostCard(GameObject tileGameObject)
        : base(tileGameObject, "FAITES DES RÉPARATIONS SUR TOUTES VOS PROPRIÉTÉS : POUR CHAQUE MAISON, PAYEZ 25M. POUR CHAQUE HÔTEL, PAYEZ 100M.")
    {
    }

    public override IEnumerator TriggerEffect(MonopolyPlayer monopolyPlayer)
    {
        Debug.Log($"Effet de carte : {description}");

        int houseRepairCost = 25;
        int hotelRepairCost = 100;
        int totalRepairCost = 0;
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
public class SpeedingFineCard : ChanceTile
{
    public SpeedingFineCard(GameObject tileGameObject)
        : base(tileGameObject,"AMENDE POUR EXCÈS DE VITESSE. PAYEZ 15M.")
    {
    }

    
    public override IEnumerator TriggerEffect(MonopolyPlayer monopolyPlayer)
    {
        Debug.Log($"Effet de carte : {description}");
        monopolyGameManager.SetGameTextEventsText(
            $"Excès de vitesse {monopolyPlayer} doit payer 15M à la banque.");
        yield return monopolyGameManager.PlayerMustPayToBank(monopolyPlayer, 15);
        yield return new WaitForSeconds(1f);
    }
}
public class AdvanceToStationCardChance : ChanceTile
{
    public AdvanceToStationCardChance(GameObject tileGameObject)
        : base(tileGameObject, "AVANCEZ JUQU'À LA PROCHAINE GARE. SI ELLE N'APPARTIENT À PERSONNE, vous pouvez l'acheter à la banque. SI ELLE APPARTIENT À UN JOUEUR, payez au propriétaire le double du loyer auquel il a droit. SI VOUS PASSEZ PAR LA CASE DÉPART, RECEVEZ 200M.")
    {
    }
    
    public override IEnumerator TriggerEffect(MonopolyPlayer monopolyPlayer)
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

                Debug.Log($"Effet de carte : {description}");
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
public class BankDividendCard : ChanceTile
{
    public BankDividendCard(GameObject tileGameObject)
        : base(tileGameObject, "La banque vous verse un dividende de 50M.")
    {
    }

    public override IEnumerator TriggerEffect(MonopolyPlayer monopolyPlayer)
    {
        Debug.Log($"Effet de carte : {description}");
        monopolyPlayer.HaveWon(50);
        monopolyGameManager.SetGameTextEventsText(
            $"La banque verse 50M à {monopolyPlayer}.");
        yield return new WaitForSeconds(1f);
    }
}
public class AdvanceToUtilityCard : ChanceTile
{
    public AdvanceToUtilityCard(GameObject tileGameObject) 
        : base(tileGameObject,"Avancez jusqu'au prochain service public. S'il n'appartient à personne, vous pouvez l'acheter à la banque. S'il appartient à un joueur, lancez les dés et payez 10x le montant du résultat au propriétaire. Si vous passez par la case Départ, recevez 200M.")
    {
    }

    public override IEnumerator TriggerEffect(MonopolyPlayer monopolyPlayer)
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


                List<int> rolledResult = new List<int>();
                foreach (List<int> rolledResultFromEnumerator in monopolyGameManager.AskAPlayerToRollDices(
                             monopolyPlayer))
                {
                    yield return null;
                    rolledResult = rolledResultFromEnumerator;
                }

                int resultPlayed = rolledResult.Sum();
                monopolyGameManager.SetGameTextEventsText($"{monopolyPlayer} a joué {resultPlayed}");
                yield return new WaitForSeconds(1f);
                int dueAmount = resultPlayed * 10;

                Debug.Log($"Effet de carte : {description}");
                monopolyGameManager.SetGameTextEventsText(
                    $"{monopolyPlayer} a joué {resultPlayed}, donc doit payer {dueAmount}");
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
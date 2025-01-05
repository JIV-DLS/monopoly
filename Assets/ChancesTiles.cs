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
    public GoToJailCard()
        : base("ALLEZ EN PRISON. ALLEZ TOUT DROIT EN PRISON. NE PASSEZ PAS PAR LA CASE DÉPART. NE RECEVEZ PAS 200M.")
    {
    }

    public override IEnumerator TriggerEffect(MonopolyPlayer MonopolyPlayer)
    {
        Debug.Log($"Effet de carte : {description}");

        // Le joueur va en prison
        MonopolyPlayer.MoveToPosition(10); // Supposons que la case Prison est à la position 10
        MonopolyPlayer.InPrison = true; // Le joueur est en prison

        Debug.Log($"{MonopolyPlayer.Name} va directement en prison. Il ne passe pas par la case Départ et ne reçoit pas 200M.");
    }
}
public class AdvanceToRueDeLaPaixCard : ChanceTile
{
    public AdvanceToRueDeLaPaixCard()
        : base("AVANCEZ JUSQU'À LA RUE DE LA PAIX.")
    {
    }

    public override IEnumerator TriggerEffect(MonopolyPlayer MonopolyPlayer)
    {
        Debug.Log($"Effet de carte : {description}");

        // Le joueur se déplace à la Rue de la Paix
        int rueDeLaPaixPosition = 39; // Supposons que la Rue de la Paix est à la position 39
        MonopolyPlayer.MoveToPosition(rueDeLaPaixPosition);

        Debug.Log($"{MonopolyPlayer.Name} avance à la Rue de la Paix.");
    }
}
public class AdvanceToStartCard : ChanceTile
{
    public AdvanceToStartCard()
        : base("AVANCEZ JUSQU'À LA CASE DÉPART. RECEVEZ 200M.")
    {
    }

    public override IEnumerator TriggerEffect(MonopolyPlayer MonopolyPlayer)
    {
        Debug.Log($"Effet de carte : {description}");

        // Le joueur se déplace à la case Départ
        MonopolyPlayer.MoveToPosition(0); // Supposons que la case Départ est à la position 0
        MonopolyPlayer.Money += 200; // Le joueur reçoit 200 millions

        Debug.Log($"{MonopolyPlayer.Name} avance à la case Départ et reçoit 200M. Nouveau solde : {MonopolyPlayer.Money}M.");
    }
}
public class RepairCostCard : ChanceTile
{
    public RepairCostCard()
        : base("FAITES DES RÉPARATIONS SUR TOUTES VOS PROPRIÉTÉS : POUR CHAQUE MAISON, PAYEZ 25M. POUR CHAQUE HÔTEL, PAYEZ 100M.")
    {
    }

    public override IEnumerator TriggerEffect(MonopolyPlayer MonopolyPlayer)
    {
        Debug.Log($"Effet de carte : {description}");

        int houseRepairCost = 25;
        int hotelRepairCost = 100;
        int totalRepairCost = 0;

        // Parcourir les propriétés du joueur et appliquer les réparations
        foreach (var property in MonopolyPlayer.Properties)
        {
            if (property.HasHouse)
            {
                totalRepairCost += houseRepairCost;
                Debug.Log($"{MonopolyPlayer.Name} paie {houseRepairCost}M pour chaque maison sur {property.Name}.");
            }

            if (property.HasHotel)
            {
                totalRepairCost += hotelRepairCost;
                Debug.Log($"{MonopolyPlayer.Name} paie {hotelRepairCost}M pour chaque hôtel sur {property.Name}.");
            }
        }

        // Déduire le coût total des réparations
        MonopolyPlayer.Money -= totalRepairCost;

        // Affichage du solde après les réparations
        Debug.Log($"{MonopolyPlayer.Name} paie {totalRepairCost}M pour les réparations. Nouveau solde : {MonopolyPlayer.Money}M.");
    }
}
public class SpeedingFineCard : ChanceTile
{
    public SpeedingFineCard()
        : base("AMENDE POUR EXCÈS DE VITESSE. PAYEZ 15M.")
    {
    }

    public override IEnumerator TriggerEffect(MonopolyPlayer MonopolyPlayer)
    {
        Debug.Log($"Effet de carte : {description}");
        
        int fineAmount = 15;
        MonopolyPlayer.Money -= fineAmount;
        
        Debug.Log($"{MonopolyPlayer.Name} paie une amende de {fineAmount}M. Nouveau solde : {MonopolyPlayer.Money}M.");
    }
}
public class AdvanceToStationCardChance : ChanceTile
{
    public AdvanceToStationCardChance()
        : base("AVANCEZ JUQU'À LA PROCHAINE GARE. SI ELLE N'APPARTIENT À PERSONNE, vous pouvez l'acheter à la banque. SI ELLE APPARTIENT À UN JOUEUR, payez au propriétaire le double du loyer auquel il a droit. SI VOUS PASSEZ PAR LA CASE DÉPART, RECEVEZ 200M.")
    {
    }

    public override IEnumerator TriggerEffect(MonopolyPlayer MonopolyPlayer)
    {
        Debug.Log($"Effet de carte : {description}");
        int nextStationPosition = FindNextStation(MonopolyPlayer.CurrentPosition);

        if (nextStationPosition < MonopolyPlayer.CurrentPosition) 
        {
            MonopolyPlayer.Money += 200;
            Debug.Log($"{MonopolyPlayer.Name} passe par la case Départ et reçoit 200M. Nouveau solde : {MonopolyPlayer.Money}M.");
        }

        MonopolyPlayer.MoveToPosition(nextStationPosition);
        HandleStation(MonopolyPlayer, nextStationPosition);
    }

    private int FindNextStation(int currentPosition)
    {
        int[] stationPositions = { 5, 15, 25, 35 }; // Positions des gares

        foreach (int position in stationPositions)
        {
            if (position > currentPosition) return position;
        }

        return stationPositions[0]; // Retourne à la première gare
    }

    private void HandleStation(MonopolyPlayer MonopolyPlayer, int stationPosition)
    {
        Tile station = Board.GetTileAtIndex(stationPosition);

        if (station.Owner == null)
        {
            Debug.Log($"La gare à la position {stationPosition} n'appartient à personne. Vous pouvez l'acheter.");
        }
        else if (station.Owner != MonopolyPlayer)
        {
            int rent = station.Rent * 2; // Double du loyer
            MonopolyPlayer.Money -= rent;
            station.Owner.Money += rent;
            Debug.Log($"{MonopolyPlayer.Name} paie {rent}M au propriétaire {station.Owner.Name}. Nouveau solde : {MonopolyPlayer.Money}M.");
        }
        else
        {
            Debug.Log($"{MonopolyPlayer.Name} possède déjà cette gare.");
        }
    }
}
public class AdvanceToStationCard : ChanceTile
{
    public AdvanceToStationCard()
        : base("Avancez jusqu'à la prochaine gare. Si elle n'appartient à personne, vous pouvez l'acheter à la banque. Si elle appartient à un joueur, payez au propriétaire le double du loyer auquel il a droit. Si vous passez par la case Départ, recevez 200M.")
    {
    }

    public override IEnumerator TriggerEffect(MonopolyPlayer MonopolyPlayer)
    {
        Debug.Log($"Effet de carte : {description}");
        int nextStationPosition = FindNextStation(MonopolyPlayer.CurrentPosition);

        if (nextStationPosition < MonopolyPlayer.CurrentPosition) 
        {
            MonopolyPlayer.Money += 200;
            Debug.Log($"{MonopolyPlayer.Name} passe par la case Départ et reçoit 200M. Nouveau solde : {MonopolyPlayer.Money}M.");
        }

        MonopolyPlayer.MoveToPosition(nextStationPosition);
        HandleStation(MonopolyPlayer, nextStationPosition);
    }

    private int FindNextStation(int currentPosition)
    {
        int[] stationPositions = { 5, 15, 25, 35 }; // Positions des gares

        foreach (int position in stationPositions)
        {
            if (position > currentPosition) return position;
        }

        return stationPositions[0]; // Retourne à la première gare
    }

    private void HandleStation(MonopolyPlayer MonopolyPlayer, int stationPosition)
    {
        Tile station = Board.GetTileAtIndex(stationPosition);

        if (station.Owner == null)
        {
            Debug.Log($"La gare à la position {stationPosition} n'appartient à personne. Vous pouvez l'acheter.");
        }
        else if (station.Owner != MonopolyPlayer)
        {
            int rent = station.Rent * 2; // Double du loyer
            MonopolyPlayer.Money -= rent;
            station.Owner.Money += rent;
            Debug.Log($"{MonopolyPlayer.Name} paie {rent}M au propriétaire {station.Owner.Name}. Nouveau solde : {MonopolyPlayer.Money}M.");
        }
        else
        {
            Debug.Log($"{MonopolyPlayer.Name} possède déjà cette gare.");
        }
    }
}
public class AdvanceToStationCard : ChanceTile
{
    public AdvanceToStationCard()
        : base("Avancez jusqu'à la prochaine gare. Si elle n'appartient à personne, vous pouvez l'acheter à la banque. Si elle appartient à un joueur, payez au propriétaire le double du loyer auquel il a droit. Si vous passez par la case Départ, recevez 200M.")
    {
    }

    public override IEnumerator TriggerEffect(MonopolyPlayer MonopolyPlayer)
    {
        Debug.Log($"Effet de carte : {description}");

        int nextStationPosition = FindNextStation(MonopolyPlayer.CurrentPosition);

        // Si le joueur passe par la case Départ
        if (nextStationPosition < MonopolyPlayer.CurrentPosition)
        {
            MonopolyPlayer.Money += 200;
            Debug.Log($"{MonopolyPlayer.Name} passe par la case Départ et reçoit 200M. Nouveau solde : {MonopolyPlayer.Money}M.");
        }

        MonopolyPlayer.MoveToPosition(nextStationPosition);
        HandleStationOwnership(MonopolyPlayer, nextStationPosition);
    }

    private int FindNextStation(int currentPosition)
    {
        // Simuler les positions des gares (par exemple : 5, 15, 25, 35)
        int[] stationPositions = { 5, 15, 25, 35 };

        foreach (int position in stationPositions)
        {
            if (position > currentPosition)
                return position;
        }

        // Retour à la première gare s'il n'y en a plus après
        return stationPositions[0];
    }

    private void HandleStationOwnership(MonopolyPlayer MonopolyPlayer, int stationPosition)
    {
        bool isOwned = UnityEngine.Random.value > 0.5f; // Simuler si la gare est possédée
        if (!isOwned)
        {
            Debug.Log($"La gare à la position {stationPosition} n'appartient à personne. Vous pouvez l'acheter.");
        }
        else
        {
            int rent = CalculateStationRent() * 2; // Double du loyer
            MonopolyPlayer.Money -= rent;
            Debug.Log($"{MonopolyPlayer.Name} paie {rent}M au propriétaire. Nouveau solde : {MonopolyPlayer.Money}M.");
        }
    }

    private int CalculateStationRent()
    {
        // Simuler un calcul de loyer (par exemple : entre 25 et 100M)
        return UnityEngine.Random.Range(25, 101);
    }
}
public class BankDividendCard : ChanceTile
{
    public BankDividendCard()
        : base("La banque vous verse un dividende de 50M.")
    {
    }

    public override IEnumerator TriggerEffect(MonopolyPlayer MonopolyPlayer)
    {
        Debug.Log($"Effet de carte : {description}");
        MonopolyPlayer.Money += 50;
        Debug.Log($"{MonopolyPlayer.Name} reçoit un dividende de 50M. Nouveau solde : {MonopolyPlayer.Money}M.");
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
        if ( nextServiceTile.IsOwned() && !nextServiceTile.IsOwnedBy(monopolyPlayer))
        {
            MonopolyPlayer tileOwner = nextServiceTile.GetOwner();
            monopolyGameManager.SetGameTextEventsText($"{nextServiceTile} est détenu par {tileOwner}");
            yield return new WaitForSeconds(1.5f);
            
            
            List<int> rolledResult = new List<int>();
            foreach (List<int> rolledResultFromEnumerator in monopolyGameManager.AskAPlayerToRollDices(monopolyPlayer))
            {
                yield return null;
                rolledResult = rolledResultFromEnumerator;
            }

            int resultPlayed = rolledResult.Sum();
            monopolyGameManager.SetGameTextEventsText($"{monopolyPlayer} a joué {resultPlayed}");
            yield return new WaitForSeconds(1f);
            int dueAmount = resultPlayed * 10;
            monopolyGameManager.SetGameTextEventsText($"{monopolyPlayer} a joué {resultPlayed}, donc doit payer {dueAmount}");
            yield return monopolyGameManager.PlayerAPayPlayerB(monopolyPlayer, tileOwner, dueAmount);
            yield return new WaitForSeconds(1f);
            

        }
        Debug.Log($"Effet de carte : {description}");

        int nextUtilityPosition = FindNextUtility(MonopolyPlayer.CurrentPosition);
        if (nextUtilityPosition < MonopolyPlayer.CurrentPosition) // Passe par la case Départ
        {
            MonopolyPlayer.Money += 200;
            Debug.Log($"{MonopolyPlayer.Name} passe par la case Départ et reçoit 200M. Nouveau solde : {MonopolyPlayer.Money}M.");
        }

        MonopolyPlayer.MoveToPosition(nextUtilityPosition);
        HandleUtilityOwnership(MonopolyPlayer, nextUtilityPosition);
        yield return null;
    }

    private int FindNextUtility(int currentPosition)
    {
        int[] utilityPositions = { 12, 28 };
        foreach (int position in utilityPositions)
        {
            if (position > currentPosition)
                return position;
        }
        return utilityPositions[0];
    }

    private void HandleUtilityOwnership(MonopolyPlayer monopolyPlayer, int utilityPosition)
    {
        monopolyGameManager.
        bool isOwned = UnityEngine.Random.value > 0.5f;
        int diceResult = UnityEngine.Random.Range(1, 7) + UnityEngine.Random.Range(1, 7);
        int rent = 10 * diceResult;
        monopolyPlayer.Money -= rent;
        Debug.Log($"{monopolyPlayer.Name} paie {rent}M au propriétaire. Nouveau solde : {monopolyPlayer.Money}M.");
    }
}
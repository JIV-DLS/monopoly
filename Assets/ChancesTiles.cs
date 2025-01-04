using System.Collections.Generic;
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

    public override void ApplyEffect(Player player)
    {
        Debug.Log($"Effet de carte : {Description}");

        // Récupérer la liste de tous les joueurs
        var allPlayers = GameManager.Instance.GetAllPlayers();
        int totalToPay = 0;

        foreach (var otherPlayer in allPlayers)
        {
            if (otherPlayer != player)
            {
                player.Money -= 50;
                otherPlayer.Money += 50;
                totalToPay += 50;

                Debug.Log($"{player.Name} paie 50M à {otherPlayer.Name}.");
            }
        }

        Debug.Log($"{player.Name} a payé un total de {totalToPay}M aux autres joueurs.");
    }
}
public class AdvanceToBoulevardDeLaVilletteCard : ChanceTile
{
    public AdvanceToBoulevardDeLaVilletteCard()
        : base("AVANCEZ JUQU'AU BOULEVARD DE LA VILLETE. SI VOUS PASSEZ PAR LA CASE DÉPART, RECEVEZ 200M.")
    {
    }

    public override void ApplyEffect(Player player)
    {
        Debug.Log($"Effet de carte : {Description}");

        // Position du Boulevard de la Villette
        int targetPosition = 11; // Exemple : index de la case sur le plateau
        bool passesStart = player.CurrentPosition > targetPosition;

        if (passesStart)
        {
            // Ajouter 200M pour avoir passé la case départ
            player.Money += 200;
            Debug.Log($"{player.Name} passe par la case départ et reçoit 200M.");
        }

        // Déplacer le joueur sur le Boulevard de la Villette
        player.MoveToPosition(targetPosition);

        Debug.Log($"{player.Name} avance jusqu'au Boulevard de la Villette.");
    }
}
public class Player
{
    public string Name { get; private set; }
    public int Money { get; set; }
    public int CurrentPosition { get; private set; }
    public bool IsInJail { get; set; }
    private List<ChanceTile> inventory;

    public Player(string name)
    {
        Name = name;
        Money = 1500; // Exemple de montant de départ
        CurrentPosition = 0;
        IsInJail = false;
        inventory = new List<ChanceTile>();
    }

    public void MoveToPosition(int position)
    {
        CurrentPosition = position;
        Debug.Log($"{Name} se déplace à la position {CurrentPosition}.");
    }

    public void AddToInventory(ChanceTile card)
    {
        inventory.Add(card);
        Debug.Log($"{Name} ajoute une carte à son inventaire : {card.Description}");
    }

    public bool UseGetOutOfJailCard()
    {
        var card = inventory.OfType<GetOutOfJailCard>().FirstOrDefault();
        if (card != null)
        {
            inventory.Remove(card);
            IsInJail = false;
            Debug.Log($"{Name} utilise une carte 'Libéré de prison' et sort de prison.");
            return true;
        }

        Debug.Log($"{Name} n'a pas de carte 'Libéré de prison'.");
        return false;
    }
}
public class GetOutOfJailCard : ChanceTile
{
    public GetOutOfJailCard()
        : base("VOUS ÊTES LIBÉRÉ DE PRISON. Cette carte peut être conservée jusqu'à ce qu'elle soit utilisée, échangée ou vendue.")
    {
    }

    public override void ApplyEffect(Player player)
    {
        Debug.Log($"Effet de carte : {Description}");

        // Ajouter cette carte à l'inventaire du joueur
        player.AddToInventory(this);

        Debug.Log($"{player.Name} conserve la carte 'Libéré de prison' pour une utilisation future.");
    }
}
public class MoveBackThreeSpacesCard : ChanceTile
{
    public MoveBackThreeSpacesCard()
        : base("RECULEZ DE TROIS CASES.")
    {
    }

    public override void ApplyEffect(Player player)
    {
        Debug.Log($"Effet de carte : {Description}");

        // Le joueur recule de trois cases
        int newPosition = player.CurrentPosition - 3;

        // S'assurer que la position ne soit pas inférieure à zéro (ne pas sortir du plateau)
        if (newPosition < 0)
        {
            newPosition = 0; // La première case du plateau
        }

        player.MoveToPosition(newPosition);

        Debug.Log($"{player.Name} recule de trois cases.");
    }
}
public class RealEstateLoanCard : ChanceTile
{
    public RealEstateLoanCard()
        : base("VOTRE PRÊT IMMOBILIER VOUS RAPPORTE. RECEVEZ 150M.")
    {
    }

    public override void ApplyEffect(Player player)
    {
        Debug.Log($"Effet de carte : {Description}");

        // Le joueur reçoit 150 millions
        player.Money += 150;
        
        Debug.Log($"{player.Name} reçoit 150M grâce à son prêt immobilier.");
    }
}
public class AdvanceToGareMontparnasseCard : ChanceTile
{
    public AdvanceToGareMontparnasseCard()
        : base("RENDEZ-VOUS À LA GARE MONTPARNASSE. SI VOUS PASSEZ PAR LA CASE DÉPART, RECEVEZ 200M.")
    {
    }

    public override void ApplyEffect(Player player)
    {
        Debug.Log($"Effet de carte : {Description}");

        // Supposons que la Gare Montparnasse est à la position 15
        int gareMontparnassePosition = 15;
        int currentPosition = player.CurrentPosition;
        
        // Le joueur se déplace à la Gare Montparnasse
        player.MoveToPosition(gareMontparnassePosition);

        // Vérifier si le joueur est passé par la case Départ
        if (currentPosition < gareMontparnassePosition)
        {
            player.Money += 200; // Le joueur reçoit 200 millions s'il passe par la case Départ
            Debug.Log($"{player.Name} passe par la case Départ et reçoit 200M.");
        }

        Debug.Log($"{player.Name} avance à la Gare Montparnasse.");
    }
}
public class AdvanceToAvenueHenriMartinCard : ChanceTile
{
    public AdvanceToAvenueHenriMartinCard()
        : base("AVANCEZ JUSQU'À L'AVENUE HENRI-MARTIN. SI VOUS PASSEZ PAR LA CASE DÉPART, RECEVEZ 200M.")
    {
    }

    public override void ApplyEffect(Player player)
    {
        Debug.Log($"Effet de carte : {Description}");

        // Supposons que l'Avenue Henri-Martin est à la position 17
        int avenueHenriMartinPosition = 17;
        int currentPosition = player.CurrentPosition;
        
        // Le joueur se déplace à l'Avenue Henri-Martin
        player.MoveToPosition(avenueHenriMartinPosition);

        // Vérifier si le joueur est passé par la case Départ
        if (currentPosition < avenueHenriMartinPosition)
        {
            player.Money += 200; // Le joueur reçoit 200 millions s'il passe par la case Départ
            Debug.Log($"{player.Name} passe par la case Départ et reçoit 200M.");
        }

        Debug.Log($"{player.Name} avance à l'Avenue Henri-Martin.");
    }
}
public class GoToJailCard : ChanceTile
{
    public GoToJailCard()
        : base("ALLEZ EN PRISON. ALLEZ TOUT DROIT EN PRISON. NE PASSEZ PAS PAR LA CASE DÉPART. NE RECEVEZ PAS 200M.")
    {
    }

    public override void ApplyEffect(Player player)
    {
        Debug.Log($"Effet de carte : {Description}");

        // Le joueur va en prison
        player.MoveToPosition(10); // Supposons que la case Prison est à la position 10
        player.InPrison = true; // Le joueur est en prison

        Debug.Log($"{player.Name} va directement en prison. Il ne passe pas par la case Départ et ne reçoit pas 200M.");
    }
}
public class AdvanceToRueDeLaPaixCard : ChanceTile
{
    public AdvanceToRueDeLaPaixCard()
        : base("AVANCEZ JUSQU'À LA RUE DE LA PAIX.")
    {
    }

    public override void ApplyEffect(Player player)
    {
        Debug.Log($"Effet de carte : {Description}");

        // Le joueur se déplace à la Rue de la Paix
        int rueDeLaPaixPosition = 39; // Supposons que la Rue de la Paix est à la position 39
        player.MoveToPosition(rueDeLaPaixPosition);

        Debug.Log($"{player.Name} avance à la Rue de la Paix.");
    }
}
public class AdvanceToStartCard : ChanceTile
{
    public AdvanceToStartCard()
        : base("AVANCEZ JUSQU'À LA CASE DÉPART. RECEVEZ 200M.")
    {
    }

    public override void ApplyEffect(Player player)
    {
        Debug.Log($"Effet de carte : {Description}");

        // Le joueur se déplace à la case Départ
        player.MoveToPosition(0); // Supposons que la case Départ est à la position 0
        player.Money += 200; // Le joueur reçoit 200 millions

        Debug.Log($"{player.Name} avance à la case Départ et reçoit 200M. Nouveau solde : {player.Money}M.");
    }
}
public class RepairCostCard : ChanceTile
{
    public RepairCostCard()
        : base("FAITES DES RÉPARATIONS SUR TOUTES VOS PROPRIÉTÉS : POUR CHAQUE MAISON, PAYEZ 25M. POUR CHAQUE HÔTEL, PAYEZ 100M.")
    {
    }

    public override void ApplyEffect(Player player)
    {
        Debug.Log($"Effet de carte : {Description}");

        int houseRepairCost = 25;
        int hotelRepairCost = 100;
        int totalRepairCost = 0;

        // Parcourir les propriétés du joueur et appliquer les réparations
        foreach (var property in player.Properties)
        {
            if (property.HasHouse)
            {
                totalRepairCost += houseRepairCost;
                Debug.Log($"{player.Name} paie {houseRepairCost}M pour chaque maison sur {property.Name}.");
            }

            if (property.HasHotel)
            {
                totalRepairCost += hotelRepairCost;
                Debug.Log($"{player.Name} paie {hotelRepairCost}M pour chaque hôtel sur {property.Name}.");
            }
        }

        // Déduire le coût total des réparations
        player.Money -= totalRepairCost;

        // Affichage du solde après les réparations
        Debug.Log($"{player.Name} paie {totalRepairCost}M pour les réparations. Nouveau solde : {player.Money}M.");
    }
}
public class SpeedingFineCard : ChanceTile
{
    public SpeedingFineCard()
        : base("AMENDE POUR EXCÈS DE VITESSE. PAYEZ 15M.")
    {
    }

    public override void ApplyEffect(Player player)
    {
        Debug.Log($"Effet de carte : {Description}");
        
        int fineAmount = 15;
        player.Money -= fineAmount;
        
        Debug.Log($"{player.Name} paie une amende de {fineAmount}M. Nouveau solde : {player.Money}M.");
    }
}
public class AdvanceToStationCardChance : ChanceTile
{
    public AdvanceToStationCardChance()
        : base("AVANCEZ JUQU'À LA PROCHAINE GARE. SI ELLE N'APPARTIENT À PERSONNE, vous pouvez l'acheter à la banque. SI ELLE APPARTIENT À UN JOUEUR, payez au propriétaire le double du loyer auquel il a droit. SI VOUS PASSEZ PAR LA CASE DÉPART, RECEVEZ 200M.")
    {
    }

    public override void ApplyEffect(Player player)
    {
        Debug.Log($"Effet de carte : {Description}");
        int nextStationPosition = FindNextStation(player.CurrentPosition);

        if (nextStationPosition < player.CurrentPosition) 
        {
            player.Money += 200;
            Debug.Log($"{player.Name} passe par la case Départ et reçoit 200M. Nouveau solde : {player.Money}M.");
        }

        player.MoveToPosition(nextStationPosition);
        HandleStation(player, nextStationPosition);
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

    private void HandleStation(Player player, int stationPosition)
    {
        Tile station = Board.GetTile(stationPosition);

        if (station.Owner == null)
        {
            Debug.Log($"La gare à la position {stationPosition} n'appartient à personne. Vous pouvez l'acheter.");
        }
        else if (station.Owner != player)
        {
            int rent = station.Rent * 2; // Double du loyer
            player.Money -= rent;
            station.Owner.Money += rent;
            Debug.Log($"{player.Name} paie {rent}M au propriétaire {station.Owner.Name}. Nouveau solde : {player.Money}M.");
        }
        else
        {
            Debug.Log($"{player.Name} possède déjà cette gare.");
        }
    }
}
public class AdvanceToStationCard : ChanceTile
{
    public AdvanceToStationCard()
        : base("Avancez jusqu'à la prochaine gare. Si elle n'appartient à personne, vous pouvez l'acheter à la banque. Si elle appartient à un joueur, payez au propriétaire le double du loyer auquel il a droit. Si vous passez par la case Départ, recevez 200M.")
    {
    }

    public override void ApplyEffect(Player player)
    {
        Debug.Log($"Effet de carte : {Description}");
        int nextStationPosition = FindNextStation(player.CurrentPosition);

        if (nextStationPosition < player.CurrentPosition) 
        {
            player.Money += 200;
            Debug.Log($"{player.Name} passe par la case Départ et reçoit 200M. Nouveau solde : {player.Money}M.");
        }

        player.MoveToPosition(nextStationPosition);
        HandleStation(player, nextStationPosition);
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

    private void HandleStation(Player player, int stationPosition)
    {
        Tile station = Board.GetTile(stationPosition);

        if (station.Owner == null)
        {
            Debug.Log($"La gare à la position {stationPosition} n'appartient à personne. Vous pouvez l'acheter.");
        }
        else if (station.Owner != player)
        {
            int rent = station.Rent * 2; // Double du loyer
            player.Money -= rent;
            station.Owner.Money += rent;
            Debug.Log($"{player.Name} paie {rent}M au propriétaire {station.Owner.Name}. Nouveau solde : {player.Money}M.");
        }
        else
        {
            Debug.Log($"{player.Name} possède déjà cette gare.");
        }
    }
}
public class AdvanceToStationCard : ChanceTile
{
    public AdvanceToStationCard()
        : base("Avancez jusqu'à la prochaine gare. Si elle n'appartient à personne, vous pouvez l'acheter à la banque. Si elle appartient à un joueur, payez au propriétaire le double du loyer auquel il a droit. Si vous passez par la case Départ, recevez 200M.")
    {
    }

    public override void ApplyEffect(Player player)
    {
        Debug.Log($"Effet de carte : {Description}");

        int nextStationPosition = FindNextStation(player.CurrentPosition);

        // Si le joueur passe par la case Départ
        if (nextStationPosition < player.CurrentPosition)
        {
            player.Money += 200;
            Debug.Log($"{player.Name} passe par la case Départ et reçoit 200M. Nouveau solde : {player.Money}M.");
        }

        player.MoveToPosition(nextStationPosition);
        HandleStationOwnership(player, nextStationPosition);
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

    private void HandleStationOwnership(Player player, int stationPosition)
    {
        bool isOwned = UnityEngine.Random.value > 0.5f; // Simuler si la gare est possédée
        if (!isOwned)
        {
            Debug.Log($"La gare à la position {stationPosition} n'appartient à personne. Vous pouvez l'acheter.");
        }
        else
        {
            int rent = CalculateStationRent() * 2; // Double du loyer
            player.Money -= rent;
            Debug.Log($"{player.Name} paie {rent}M au propriétaire. Nouveau solde : {player.Money}M.");
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

    public override void ApplyEffect(Player player)
    {
        Debug.Log($"Effet de carte : {Description}");
        player.Money += 50;
        Debug.Log($"{player.Name} reçoit un dividende de 50M. Nouveau solde : {player.Money}M.");
    }
}
public class AdvanceToUtilityCard : ChanceTile
{
    public AdvanceToUtilityCard() 
        : base("Avancez jusqu'au prochain service public. S'il n'appartient à personne, vous pouvez l'acheter à la banque. S'il appartient à un joueur, lancez les dés et payez 10x le montant du résultat au propriétaire. Si vous passez par la case Départ, recevez 200M.")
    {
    }

    public override void ApplyEffect(Player player)
    {
        Debug.Log($"Effet de carte : {Description}");

        int nextUtilityPosition = FindNextUtility(player.CurrentPosition);
        if (nextUtilityPosition < player.CurrentPosition) // Passe par la case Départ
        {
            player.Money += 200;
            Debug.Log($"{player.Name} passe par la case Départ et reçoit 200M. Nouveau solde : {player.Money}M.");
        }

        player.MoveToPosition(nextUtilityPosition);
        HandleUtilityOwnership(player, nextUtilityPosition);
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

    private void HandleUtilityOwnership(Player player, int utilityPosition)
    {
        bool isOwned = UnityEngine.Random.value > 0.5f;
        if (!isOwned)
        {
            Debug.Log($"Le service public à la position {utilityPosition} n'appartient à personne. Vous pouvez l'acheter.");
        }
        else
        {
            int diceResult = UnityEngine.Random.Range(1, 7) + UnityEngine.Random.Range(1, 7);
            int rent = 10 * diceResult;
            player.Money -= rent;
            Debug.Log($"{player.Name} paie {rent}M au propriétaire. Nouveau solde : {player.Money}M.");
        }
    }
}
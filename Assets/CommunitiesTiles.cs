
using System.Collections.Generic;
using UnityEngine;

public class CommunitiesTiles
{
    
}

public class AdoptPuppyCard : CommunityTile
{
    public AdoptPuppyCard()
        : base("OUAF, OUAF ! VOUS ADOPTEZ UN CHIOT DANS UN REFUGE !\nVOUS ÊTES LIBÉRÉ DE PRISON.\nConservez cette carte jusqu'à ce qu'elle soit utilisée, échangée ou vendue.")
    {
    }

    public override void ApplyEffect(Player player)
    {
        Debug.Log($"Effet de carte : {Description}");

        // Le joueur est libéré de prison
        player.ReleaseFromJail();

        Debug.Log($"{player.Name} a adopté un chiot et est libéré de prison !");
    }
}
public class HelpNeighborCard : CommunityTile
{
    public HelpNeighborCard()
        : base("VOUS AIDEZ VOTRE VOISINE À PORTER SES COURSES. ELLE VOUS PRÉPARE UN REPAS POUR VOUS REMERCIER !\nRECEVEZ 20M.")
    {
    }

    public override void ApplyEffect(Player player)
    {
        Debug.Log($"Effet de carte : {Description}");

        // Le joueur reçoit 20M
        player.ReceiveMoney(20);

        Debug.Log($"{player.Name} a aidé sa voisine et a reçu 20M !");
    }
}
public class LoudMusicCard : CommunityTile
{
    public LoudMusicCard()
        : base("VOUS ÉCOUTEZ DE LA MUSIQUE À FOND TARD DANS LA NUIT ?\nVOS VOISINS N'APPRÉCIENT PAS.\nALLEZ EN PRISON. ALLEZ TOUT DROIT EN PRISON.\nNE PASSEZ PAS PAR LA CASE DÉPART, NE RECEVEZ PAS 200M.")
    {
    }

    public override void ApplyEffect(Player player)
    {
        Debug.Log($"Effet de carte : {Description}");

        // Le joueur va directement en prison
        player.MoveToPrison();

        Debug.Log($"{player.Name} a été envoyé en prison pour avoir écouté de la musique trop fort !");
    }
}

public class MarathonForHospitalCard : CommunityTile
{
    public MarathonForHospitalCard()
        : base("ALORS QUE VOUS NE PENSAIEZ PAS POUVOIR FAIRE UN MÈTRE DE PLUS, VOUS FRANCHISSEZ LA LIGNE D'ARRIVÉE DU MARATHON, ET VOUS RÉCOLTEZ DES FONDS POUR L'HÔPITAL LOCAL !\n\nAVANCEZ JUSQU'À LA CASE DÉPART.\nRECEVEZ 200M.")
    {
    }

    public override void ApplyEffect(Player player)
    {
        Debug.Log($"Effet de carte : {Description}");

        // Avancer jusqu'à la case Départ
        player.MoveToStart();
        
        // Ajouter 200M au joueur
        player.Money += 200;

        Debug.Log($"{player.Name} a avancé jusqu'à la case Départ et a reçu 200M.");
    }
}
public class BloodDonationCard : CommunityTile
{
    public BloodDonationCard()
        : base("VOUS DONNEZ VOTRE SANG. IL Y'A DES GÂTEAUX GRATUITS !\n\nRECEVEZ 10M.")
    {
    }

    public override void ApplyEffect(Player player)
    {
        Debug.Log($"Effet de carte : {Description}");

        // Ajouter 10M au joueur
        player.Money += 10;

        Debug.Log($"{player.Name} a reçu 10M pour avoir donné son sang.");
    }
}
public class AnimalShelterDonationCard : CommunityTile
{
    public AnimalShelterDonationCard()
        : base("VOS AMIS À FOURRURE DU REFUGE ANIMALIER VOUS SERONT RECONNAISSANTS POUR VOTRE DON.\n\nPAYEZ 50M.")
    {
    }

    public override void ApplyEffect(Player player)
    {
        Debug.Log($"Effet de carte : {Description}");

        // Retirer 50M du joueur
        player.Money -= 50;

        Debug.Log($"{player.Name} a payé 50M en don au refuge animalier.");
    }
}
public class ChattingWithElderNeighborCard : CommunityTile
{
    public ChattingWithElderNeighborCard()
        : base("VOUS PRENEZ LE TEMPS CHAQUE SEMAINE DE BAVARDER AVEC VOTRE VOISIN ÂGÉ. VOUS AVEZ ENTENDU DES HISTOIRES ÉTONNANTES !\n\nRECEVEZ 100M.")
    {
    }

    public override void ApplyEffect(Player player)
    {
        Debug.Log($"Effet de carte : {Description}");

        // Ajouter 100M au joueur
        player.Money += 100;

        Debug.Log($"{player.Name} a reçu 100M pour avoir bavardé avec son voisin âgé.");
    }
}
public class PedestrianPathCleanupCard : CommunityTile
{
    public PedestrianPathCleanupCard()
        : base("VOUS ORGANISEZ UN GROUPE POUR NETTOYER LE PARCOURS PIÉTON DE VOTRE VILLE.\n\nRECEVEZ 50M.")
    {
    }

    public override void ApplyEffect(Player player)
    {
        Debug.Log($"Effet de carte : {Description}");

        // Ajouter 50M au joueur
        player.Money += 50;

        Debug.Log($"{player.Name} a reçu 50M pour avoir organisé le nettoyage du parcours piéton.");
    }
}

public class HospitalPlayCard : CommunityTile
{
    public HospitalPlayCard()
        : base("VOUS PASSEZ LA JOURNÉE À JOUER AVEC LES ENFANTS À L'HÔPITAL LOCAL.\n\nRECEVEZ 100M.")
    {
    }

    public override void ApplyEffect(Player player)
    {
        Debug.Log($"Effet de carte : {Description}");

        // Ajouter 100M au joueur
        player.Money += 100;

        Debug.Log($"{player.Name} a reçu 100M pour avoir joué avec les enfants à l'hôpital local.");
    }
}
public class GardenCleanupCard : CommunityTile
{
    public GardenCleanupCard()
        : base("VOUS AIDEZ VOS VOISINS À NETTOYER LEUR JARDIN APRÈS UNE GROSSE TEMPÊTE.\n\nRECEVEZ 200M.")
    {
    }

    public override void ApplyEffect(Player player)
    {
        Debug.Log($"Effet de carte : {Description}");

        // Ajouter 200M au joueur
        player.Money += 200;

        Debug.Log($"{player.Name} a reçu 200M pour avoir aidé ses voisins à nettoyer leur jardin.");
    }
}
public class BakeSalePurchaseCard : CommunityTile
{
    public BakeSalePurchaseCard()
        : base("VOUS AVEZ ACHETÉ DES PETITS GÂTEAUX À LA VENTE DE PÂTISSERIES DE L'ÉCOLE. MIAM !\n\nPAYEZ 50M.")
    {
    }

    public override void ApplyEffect(Player player)
    {
        Debug.Log($"Effet de carte : {Description}");

        // Déduire 50M au joueur
        player.Money -= 50;

        Debug.Log($"{player.Name} a payé 50M pour des petits gâteaux délicieux.");
    }
}
public class CharityCarWashCard : CommunityTile
{
    public CharityCarWashCard()
        : base("VOUS ALLEZ AU LAVE-AUTO CARITATIF DE L'ÉCOLE DU QUARTIER, MAIS VOUS OUBLIEZ DE REMONTER VOS VITRES !\n\nPAYER 100M.")
    {
    }

    public override void ApplyEffect(Player player)
    {
        Debug.Log($"Effet de carte : {Description}");

        // Déduire 100M au joueur
        player.Money -= 100;

        Debug.Log($"{player.Name} a payé 100M après un passage au lave-auto caritatif.");
    }
}
public class HousingImprovementCard : CommunityTile
{
    public HousingImprovementCard()
        : base("VOUS AURIEZ DÛ VOUS PORTER VOLONTAIRE POUR CE PROJET D'AMÉLIORATION DE L'HABITAT. CELA VOUS AURAIT PERMIS D'ACQUÉRIR DE NOUVELLES COMPÉTENCES UTILES !\n\nPOUR CHAQUE MAISON QUE VOUS POSSÉDEZ, PAYEZ 40M.\nPOUR CHAQUE HOTEL QUE VOUS POSSÉDEZ, PAYEZ 115M.")
    {
    }

    public override void ApplyEffect(Player player)
    {
        Debug.Log($"Effet de carte : {Description}");

        int housesOwned = player.Properties.Sum(property => property.Houses);
        int hotelsOwned = player.Properties.Sum(property => property.Hotels);

        int totalCost = (housesOwned * 40) + (hotelsOwned * 115);

        // Déduire le coût total du joueur
        player.Money -= totalCost;

        Debug.Log($"{player.Name} possède {housesOwned} maisons et {hotelsOwned} hôtels.");
        Debug.Log($"{player.Name} doit payer un total de {totalCost}M pour le projet d'amélioration de l'habitat.");
    }
}
public class BakeSaleCard : CommunityTile
{
    public BakeSaleCard()
        : base("VOUS ORGANISEZ UNE VENTE DE PÂTISSERIES POUR L'ÉCOLE DE VOTRE QUARTIER.\n\nRECEVEZ 25M.")
    {
    }

    public override void ApplyEffect(Player player)
    {
        Debug.Log($"Effet de carte : {Description}");

        // Ajouter 25M au joueur
        player.Money += 25;

        Debug.Log($"{player.Name} reçoit 25M pour avoir organisé une vente de pâtisseries.");
    }
}
public class NeighborhoodPartyCard : CommunityTile
{
    public NeighborhoodPartyCard()
        : base("VOUS ORGANISEZ UNE FÊTE DE QUARTIER POUR QUE LES HABITANTS DE VOTRE RUE PUISSENT FAIRE CONNAISSANCE ENTRE EUX.\n\nRECEVEZ 10M DE CHAQUE JOUEUR.")
    {
    }

    public override void ApplyEffect(Player player)
    {
        Debug.Log($"Effet de carte : {Description}");

        // Récupérer la liste de tous les joueurs
        var allPlayers = GameManager.Instance.GetAllPlayers();
        int totalReceived = 0;

        foreach (var otherPlayer in allPlayers)
        {
            if (otherPlayer != player)
            {
                otherPlayer.Money -= 10;
                player.Money += 10;
                totalReceived += 10;

                Debug.Log($"{otherPlayer.Name} paie 10M à {player.Name} pour la fête de quartier.");
            }
        }

        Debug.Log($"{player.Name} a reçu un total de {totalReceived}M des autres joueurs pour la fête.");
    }
}
public class PlaygroundDonationCard : CommunityTile
{
    public PlaygroundDonationCard()
        : base("VOUS AVEZ AIDÉ À CONSTRUIRE UN NOUVEAU TERRAIN DE JEUX POUR L'ÉCOLE. VOUS ESSAYEZ LE TOBOGAN !\n\nRECEVEZ 100M.")
    {
    }

    public override void ApplyEffect(Player player)
    {
        Debug.Log($"Effet de carte : {Description}");

        // Ajouter 100M au joueur
        player.Money += 100;

        Debug.Log($"{player.Name} reçoit 100M pour avoir aidé à construire un terrain de jeux.");
    }
}

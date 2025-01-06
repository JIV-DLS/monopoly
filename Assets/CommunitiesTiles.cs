
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommunitiesTiles
{
    
}

public class AdoptPuppyCard : CommunityCard
{
    public AdoptPuppyCard(MonopolyGameManager tileMonopolyGameManager)
        : base(tileMonopolyGameManager, "OUAF, OUAF ! VOUS ADOPTEZ UN CHIOT DANS UN REFUGE !\nVOUS ÊTES LIBÉRÉ DE PRISON.\nConservez cette carte jusqu'à ce qu'elle soit utilisée, échangée ou vendue.")
    {
    }

    
    public override IEnumerator TriggerEffect(MonopolyPlayer monopolyPlayer)
    {
        Debug.Log($"Effet de carte : {description}");
        
        monopolyGameManager.SetGameTextEventsText($"{monopolyPlayer} a la possibilité de sortir en prison si il y va.");
        yield return monopolyGameManager.GiveCommunityCardToPlayerToAdoptAPuppyCard(monopolyPlayer);
        yield return new WaitForSeconds(.5f);
    }
}
public class HelpNeighborCard : CommunityCard
{
    public HelpNeighborCard()
        : base("VOUS AIDEZ VOTRE VOISINE À PORTER SES COURSES. ELLE VOUS PRÉPARE UN REPAS POUR VOUS REMERCIER !\nRECEVEZ 20M.")
    {
    }

    public override IEnumerator TriggerEffect(MonopolyPlayer MonopolyPlayer)
    {
        Debug.Log($"Effet de carte : {description}");

        // Le joueur reçoit 20M
        MonopolyPlayer.ReceiveMoney(20);

        Debug.Log($"{MonopolyPlayer.Name} a aidé sa voisine et a reçu 20M !");
    }
}
public class LoudMusicCard : CommunityCard
{
    public LoudMusicCard()
        : base("VOUS ÉCOUTEZ DE LA MUSIQUE À FOND TARD DANS LA NUIT ?\nVOS VOISINS N'APPRÉCIENT PAS.\nALLEZ EN PRISON. ALLEZ TOUT DROIT EN PRISON.\nNE PASSEZ PAS PAR LA CASE DÉPART, NE RECEVEZ PAS 200M.")
    {
    }

    public override IEnumerator TriggerEffect(MonopolyPlayer MonopolyPlayer)
    {
        Debug.Log($"Effet de carte : {description}");

        // Le joueur va directement en prison
        MonopolyPlayer.MoveToPrison();

        Debug.Log($"{MonopolyPlayer.Name} a été envoyé en prison pour avoir écouté de la musique trop fort !");
    }
}

public class MarathonForHospitalCard : CommunityCard
{
    public MarathonForHospitalCard()
        : base("ALORS QUE VOUS NE PENSAIEZ PAS POUVOIR FAIRE UN MÈTRE DE PLUS, VOUS FRANCHISSEZ LA LIGNE D'ARRIVÉE DU MARATHON, ET VOUS RÉCOLTEZ DES FONDS POUR L'HÔPITAL LOCAL !\n\nAVANCEZ JUSQU'À LA CASE DÉPART.\nRECEVEZ 200M.")
    {
    }

    public override IEnumerator TriggerEffect(MonopolyPlayer MonopolyPlayer)
    {
        Debug.Log($"Effet de carte : {description}");

        // Avancer jusqu'à la case Départ
        MonopolyPlayer.MoveToStart();
        
        // Ajouter 200M au joueur
        MonopolyPlayer.Money += 200;

        Debug.Log($"{MonopolyPlayer.Name} a avancé jusqu'à la case Départ et a reçu 200M.");
    }
}
public class BloodDonationCard : CommunityCard
{
    public BloodDonationCard()
        : base("VOUS DONNEZ VOTRE SANG. IL Y'A DES GÂTEAUX GRATUITS !\n\nRECEVEZ 10M.")
    {
    }

    public override IEnumerator TriggerEffect(MonopolyPlayer MonopolyPlayer)
    {
        Debug.Log($"Effet de carte : {description}");

        // Ajouter 10M au joueur
        MonopolyPlayer.Money += 10;

        Debug.Log($"{MonopolyPlayer.Name} a reçu 10M pour avoir donné son sang.");
    }
}
public class AnimalShelterDonationCard : CommunityCard
{
    public AnimalShelterDonationCard()
        : base("VOS AMIS À FOURRURE DU REFUGE ANIMALIER VOUS SERONT RECONNAISSANTS POUR VOTRE DON.\n\nPAYEZ 50M.")
    {
    }

    public override IEnumerator TriggerEffect(MonopolyPlayer MonopolyPlayer)
    {
        Debug.Log($"Effet de carte : {description}");

        // Retirer 50M du joueur
        MonopolyPlayer.Money -= 50;

        Debug.Log($"{MonopolyPlayer.Name} a payé 50M en don au refuge animalier.");
    }
}
public class ChattingWithElderNeighborCard : CommunityCard
{
    public ChattingWithElderNeighborCard()
        : base("VOUS PRENEZ LE TEMPS CHAQUE SEMAINE DE BAVARDER AVEC VOTRE VOISIN ÂGÉ. VOUS AVEZ ENTENDU DES HISTOIRES ÉTONNANTES !\n\nRECEVEZ 100M.")
    {
    }

    public override IEnumerator TriggerEffect(MonopolyPlayer MonopolyPlayer)
    {
        Debug.Log($"Effet de carte : {description}");

        // Ajouter 100M au joueur
        MonopolyPlayer.Money += 100;

        Debug.Log($"{MonopolyPlayer.Name} a reçu 100M pour avoir bavardé avec son voisin âgé.");
    }
}
public class PedestrianPathCleanupCard : CommunityCard
{
    public PedestrianPathCleanupCard()
        : base("VOUS ORGANISEZ UN GROUPE POUR NETTOYER LE PARCOURS PIÉTON DE VOTRE VILLE.\n\nRECEVEZ 50M.")
    {
    }

    public override IEnumerator TriggerEffect(MonopolyPlayer MonopolyPlayer)
    {
        Debug.Log($"Effet de carte : {description}");

        // Ajouter 50M au joueur
        MonopolyPlayer.Money += 50;

        Debug.Log($"{MonopolyPlayer.Name} a reçu 50M pour avoir organisé le nettoyage du parcours piéton.");
    }
}

public class HospitalPlayCard : CommunityCard
{
    public HospitalPlayCard()
        : base("VOUS PASSEZ LA JOURNÉE À JOUER AVEC LES ENFANTS À L'HÔPITAL LOCAL.\n\nRECEVEZ 100M.")
    {
    }

    public override IEnumerator TriggerEffect(MonopolyPlayer MonopolyPlayer)
    {
        Debug.Log($"Effet de carte : {description}");

        // Ajouter 100M au joueur
        MonopolyPlayer.Money += 100;

        Debug.Log($"{MonopolyPlayer.Name} a reçu 100M pour avoir joué avec les enfants à l'hôpital local.");
    }
}
public class GardenCleanupCard : CommunityCard
{
    public GardenCleanupCard()
        : base("VOUS AIDEZ VOS VOISINS À NETTOYER LEUR JARDIN APRÈS UNE GROSSE TEMPÊTE.\n\nRECEVEZ 200M.")
    {
    }

    public override IEnumerator TriggerEffect(MonopolyPlayer MonopolyPlayer)
    {
        Debug.Log($"Effet de carte : {description}");

        // Ajouter 200M au joueur
        MonopolyPlayer.Money += 200;

        Debug.Log($"{MonopolyPlayer.Name} a reçu 200M pour avoir aidé ses voisins à nettoyer leur jardin.");
    }
}
public class BakeSalePurchaseCard : CommunityCard
{
    public BakeSalePurchaseCard()
        : base("VOUS AVEZ ACHETÉ DES PETITS GÂTEAUX À LA VENTE DE PÂTISSERIES DE L'ÉCOLE. MIAM !\n\nPAYEZ 50M.")
    {
    }

    public override IEnumerator TriggerEffect(MonopolyPlayer MonopolyPlayer)
    {
        Debug.Log($"Effet de carte : {description}");

        // Déduire 50M au joueur
        MonopolyPlayer.Money -= 50;

        Debug.Log($"{MonopolyPlayer.Name} a payé 50M pour des petits gâteaux délicieux.");
    }
}
public class CharityCarWashCard : CommunityCard
{
    public CharityCarWashCard()
        : base("VOUS ALLEZ AU LAVE-AUTO CARITATIF DE L'ÉCOLE DU QUARTIER, MAIS VOUS OUBLIEZ DE REMONTER VOS VITRES !\n\nPAYER 100M.")
    {
    }

    public override IEnumerator TriggerEffect(MonopolyPlayer MonopolyPlayer)
    {
        Debug.Log($"Effet de carte : {description}");

        // Déduire 100M au joueur
        MonopolyPlayer.Money -= 100;

        Debug.Log($"{MonopolyPlayer.Name} a payé 100M après un passage au lave-auto caritatif.");
    }
}
public class HousingImprovementCard : CommunityCard
{
    public HousingImprovementCard()
        : base("VOUS AURIEZ DÛ VOUS PORTER VOLONTAIRE POUR CE PROJET D'AMÉLIORATION DE L'HABITAT. CELA VOUS AURAIT PERMIS D'ACQUÉRIR DE NOUVELLES COMPÉTENCES UTILES !\n\nPOUR CHAQUE MAISON QUE VOUS POSSÉDEZ, PAYEZ 40M.\nPOUR CHAQUE HOTEL QUE VOUS POSSÉDEZ, PAYEZ 115M.")
    {
    }

    public override IEnumerator TriggerEffect(MonopolyPlayer MonopolyPlayer)
    {
        Debug.Log($"Effet de carte : {description}");

        int housesOwned = MonopolyPlayer.Properties.Sum(property => property.Houses);
        int hotelsOwned = MonopolyPlayer.Properties.Sum(property => property.Hotels);

        int totalCost = (housesOwned * 40) + (hotelsOwned * 115);

        // Déduire le coût total du joueur
        MonopolyPlayer.Money -= totalCost;

        Debug.Log($"{MonopolyPlayer.Name} possède {housesOwned} maisons et {hotelsOwned} hôtels.");
        Debug.Log($"{MonopolyPlayer.Name} doit payer un total de {totalCost}M pour le projet d'amélioration de l'habitat.");
    }
}
public class BakeSaleCard : CommunityCard
{
    public BakeSaleCard()
        : base("VOUS ORGANISEZ UNE VENTE DE PÂTISSERIES POUR L'ÉCOLE DE VOTRE QUARTIER.\n\nRECEVEZ 25M.")
    {
    }

    public override IEnumerator TriggerEffect(MonopolyPlayer MonopolyPlayer)
    {
        Debug.Log($"Effet de carte : {description}");

        // Ajouter 25M au joueur
        MonopolyPlayer.Money += 25;

        Debug.Log($"{MonopolyPlayer.Name} reçoit 25M pour avoir organisé une vente de pâtisseries.");
    }
}
public class NeighborhoodPartyCard : CommunityCard
{
    public NeighborhoodPartyCard()
        : base("VOUS ORGANISEZ UNE FÊTE DE QUARTIER POUR QUE LES HABITANTS DE VOTRE RUE PUISSENT FAIRE CONNAISSANCE ENTRE EUX.\n\nRECEVEZ 10M DE CHAQUE JOUEUR.")
    {
    }

    public override IEnumerator TriggerEffect(MonopolyPlayer MonopolyPlayer)
    {
        Debug.Log($"Effet de carte : {description}");

        // Récupérer la liste de tous les joueurs
        var allMonopolyPlayers = GameManager.Instance.GetAllMonopolyPlayers();
        int totalReceived = 0;

        foreach (var otherMonopolyPlayer in allMonopolyPlayers)
        {
            if (otherMonopolyPlayer != MonopolyPlayer)
            {
                otherMonopolyPlayer.Money -= 10;
                MonopolyPlayer.Money += 10;
                totalReceived += 10;

                Debug.Log($"{otherMonopolyPlayer.Name} paie 10M à {MonopolyPlayer.Name} pour la fête de quartier.");
            }
        }

        Debug.Log($"{MonopolyPlayer.Name} a reçu un total de {totalReceived}M des autres joueurs pour la fête.");
    }
}
public class PlaygroundDonationCard : CommunityCard
{
    public PlaygroundDonationCard()
        : base("VOUS AVEZ AIDÉ À CONSTRUIRE UN NOUVEAU TERRAIN DE JEUX POUR L'ÉCOLE. VOUS ESSAYEZ LE TOBOGAN !\n\nRECEVEZ 100M.")
    {
    }

    public override IEnumerator TriggerEffect(MonopolyPlayer MonopolyPlayer)
    {
        Debug.Log($"Effet de carte : {description}");

        // Ajouter 100M au joueur
        MonopolyPlayer.Money += 100;

        Debug.Log($"{MonopolyPlayer.Name} reçoit 100M pour avoir aidé à construire un terrain de jeux.");
    }
}

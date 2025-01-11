
using System.Collections;
using Monopoly;
using UnityEngine;


public class AdoptPuppyCard : CommunityCard
{
    public AdoptPuppyCard(MonopolyGameManager tileMonopolyGameManager)
        : base( tileMonopolyGameManager, "OUAF, OUAF ! VOUS ADOPTEZ UN CHIOT DANS UN REFUGE !\nVOUS ÊTES LIBÉRÉ DE PRISON.\nConservez cette carte jusqu'à ce qu'elle soit utilisée, échangée ou vendue.")
    {
    }

    
    protected override IEnumerator TriggerEffect(MonopolyPlayer monopolyPlayer)
    {
        Debug.Log($"Effet de carte : {description}");
        
        monopolyGameManager.SetGameTextEventsText($"{monopolyPlayer.name} a la possibilité de sortir en prison si il y va.");
        yield return monopolyGameManager.GiveCommunityCardToPlayerToAdoptAPuppyCard(monopolyPlayer);
        yield return new WaitForSeconds(.5f);
    }
}
public class HelpNeighborCard : CommunityCard
{
    public HelpNeighborCard(MonopolyGameManager monopolyGameManager)
        : base( monopolyGameManager, "VOUS AIDEZ VOTRE VOISINE À PORTER SES COURSES. ELLE VOUS PRÉPARE UN REPAS POUR VOUS REMERCIER !\nRECEVEZ 20M.")
    {
    }

    protected override IEnumerator TriggerEffect(MonopolyPlayer monopolyPlayer)
    {
        monopolyPlayer.HaveWon(20);
        monopolyGameManager.SetGameTextEventsText(
            $"La banque verse 20M à {monopolyPlayer.name}.");
        yield return new WaitForSeconds(1f);
    }
}
public class LoudMusicCard : CommunityCard
{
    public LoudMusicCard(MonopolyGameManager monopolyGameManager)
        : base( monopolyGameManager, "VOUS ÉCOUTEZ DE LA MUSIQUE À FOND TARD DANS LA NUIT ?\nVOS VOISINS N'APPRÉCIENT PAS.\nALLEZ EN PRISON. ALLEZ TOUT DROIT EN PRISON.\nNE PASSEZ PAS PAR LA CASE DÉPART, NE RECEVEZ PAS 200M.")
    {
    }

    protected override IEnumerator TriggerEffect(MonopolyPlayer monopolyPlayer)
    {
        monopolyGameManager.SetGameTextEventsText($"{monopolyPlayer.name} doit allez en prison.");
        yield return monopolyGameManager.PutPlayerIntoPrison(monopolyPlayer);
        yield return new WaitForSeconds(.5f);
    }
}

public class MarathonForHospitalCard : CommunityCard
{
    public MarathonForHospitalCard(MonopolyGameManager monopolyGameManager)
        : base( monopolyGameManager, "ALORS QUE VOUS NE PENSAIEZ PAS POUVOIR FAIRE UN MÈTRE DE PLUS, VOUS FRANCHISSEZ LA LIGNE D'ARRIVÉE DU MARATHON, ET VOUS RÉCOLTEZ DES FONDS POUR L'HÔPITAL LOCAL !\n\nAVANCEZ JUSQU'À LA CASE DÉPART.\nRECEVEZ 200M.")
    {
    }

    protected override IEnumerator TriggerEffect(MonopolyPlayer monopolyPlayer)
    {
        monopolyGameManager.SetGameTextEventsText($"{monopolyPlayer.name} doit se déplacer à la case départ.");
        yield return monopolyGameManager.MoveAPlayerToStartTile(monopolyPlayer);
        yield return new WaitForSeconds(.5f);
    }
}
public class BloodDonationCard : CommunityCard
{
    public BloodDonationCard(MonopolyGameManager monopolyGameManager)
        : base( monopolyGameManager, "VOUS DONNEZ VOTRE SANG. IL Y'A DES GÂTEAUX GRATUITS !\n\nRECEVEZ 10M.")
    {
    }

    protected override IEnumerator TriggerEffect(MonopolyPlayer monopolyPlayer)
    {
        monopolyPlayer.HaveWon(10);
        monopolyGameManager.SetGameTextEventsText(
            $"La banque verse 10M à {monopolyPlayer.name}.");
        yield return new WaitForSeconds(1f);
    }
}
public class AnimalShelterDonationCard : CommunityCard
{
    public AnimalShelterDonationCard(MonopolyGameManager monopolyGameManager)
        : base( monopolyGameManager, "VOS AMIS À FOURRURE DU REFUGE ANIMALIER VOUS SERONT RECONNAISSANTS POUR VOTRE DON.\n\nPAYEZ 50M.")
    {
    }

    protected override IEnumerator TriggerEffect(MonopolyPlayer monopolyPlayer)
    {
        monopolyGameManager.SetGameTextEventsText(
            $"{monopolyPlayer.name} doit payer 50M à la banque.");
        yield return monopolyGameManager.PlayerMustPayToBank(monopolyPlayer, 50);
        yield return new WaitForSeconds(1f);
    }
}
public class ChattingWithElderNeighborCard : CommunityCard
{
    public ChattingWithElderNeighborCard(MonopolyGameManager monopolyGameManager)
        : base( monopolyGameManager, "VOUS PRENEZ LE TEMPS CHAQUE SEMAINE DE BAVARDER AVEC VOTRE VOISIN ÂGÉ. VOUS AVEZ ENTENDU DES HISTOIRES ÉTONNANTES !\n\nRECEVEZ 100M.")
    {
    }

    protected override IEnumerator TriggerEffect(MonopolyPlayer monopolyPlayer)
    {
        monopolyPlayer.HaveWon(100);
        monopolyGameManager.SetGameTextEventsText(
            $"La banque verse 100M à {monopolyPlayer.name}.");
        yield return new WaitForSeconds(1f);
    }
}
public class PedestrianPathCleanupCard : CommunityCard
{
    public PedestrianPathCleanupCard(MonopolyGameManager monopolyGameManager)
        : base( monopolyGameManager, "VOUS ORGANISEZ UN GROUPE POUR NETTOYER LE PARCOURS PIÉTON DE VOTRE VILLE.\n\nRECEVEZ 50M.")
    {
    }

    protected override IEnumerator TriggerEffect(MonopolyPlayer monopolyPlayer)
    {
        monopolyPlayer.HaveWon(50);
        monopolyGameManager.SetGameTextEventsText(
            $"La banque verse 50M à {monopolyPlayer.name}.");
        yield return new WaitForSeconds(1f);
    }
}

public class HospitalPlayCard : CommunityCard
{
    public HospitalPlayCard(MonopolyGameManager monopolyGameManager)
        : base( monopolyGameManager, "VOUS PASSEZ LA JOURNÉE À JOUER AVEC LES ENFANTS À L'HÔPITAL LOCAL.\n\nRECEVEZ 100M.")
    {
    }

    protected override IEnumerator TriggerEffect(MonopolyPlayer monopolyPlayer)
    {
        monopolyPlayer.HaveWon(100);
        monopolyGameManager.SetGameTextEventsText(
            $"La banque verse 100M à {monopolyPlayer.name}.");
        yield return new WaitForSeconds(1f);
    }
}
public class GardenCleanupCard : CommunityCard
{
    public GardenCleanupCard(MonopolyGameManager monopolyGameManager)
        : base( monopolyGameManager, "VOUS AIDEZ VOS VOISINS À NETTOYER LEUR JARDIN APRÈS UNE GROSSE TEMPÊTE.\n\nRECEVEZ 200M.")
    {
    }

    protected override IEnumerator TriggerEffect(MonopolyPlayer monopolyPlayer)
    {
        monopolyPlayer.HaveWon(200);
        monopolyGameManager.SetGameTextEventsText(
            $"La banque verse 200M à {monopolyPlayer.name}.");
        yield return new WaitForSeconds(1f);
    }
}
public class BakeSalePurchaseCard : CommunityCard
{
    public BakeSalePurchaseCard(MonopolyGameManager monopolyGameManager)
        : base( monopolyGameManager, "VOUS AVEZ ACHETÉ DES PETITS GÂTEAUX À LA VENTE DE PÂTISSERIES DE L'ÉCOLE. MIAM !\n\nPAYEZ 50M.")
    {
    }

    protected override IEnumerator TriggerEffect(MonopolyPlayer monopolyPlayer)
    {
        monopolyGameManager.SetGameTextEventsText(
            $"{monopolyPlayer.name} doit payer 50M à la banque.");
        yield return monopolyGameManager.PlayerMustPayToBank(monopolyPlayer, 50);
        yield return new WaitForSeconds(1f);
    }
}
public class CharityCarWashCard : CommunityCard
{
    public CharityCarWashCard(MonopolyGameManager monopolyGameManager)
        : base( monopolyGameManager, "VOUS ALLEZ AU LAVE-AUTO CARITATIF DE L'ÉCOLE DU QUARTIER, MAIS VOUS OUBLIEZ DE REMONTER VOS VITRES !\n\nPAYER 100M.")
    {
    }

    protected override IEnumerator TriggerEffect(MonopolyPlayer monopolyPlayer)
    {
        
        monopolyGameManager.SetGameTextEventsText(
            $"{monopolyPlayer.name} doit payer 100M à la banque.");
        yield return monopolyGameManager.PlayerMustPayToBank(monopolyPlayer, 100);
        yield return new WaitForSeconds(1f);
    }
}
public class HousingImprovementCard : CommunityCard
{
    public HousingImprovementCard(MonopolyGameManager monopolyGameManager)
        : base( monopolyGameManager, "VOUS AURIEZ DÛ VOUS PORTER VOLONTAIRE POUR CE PROJET D'AMÉLIORATION DE L'HABITAT. CELA VOUS AURAIT PERMIS D'ACQUÉRIR DE NOUVELLES COMPÉTENCES UTILES !\n\nPOUR CHAQUE MAISON QUE VOUS POSSÉDEZ, PAYEZ 40M.\nPOUR CHAQUE HOTEL QUE VOUS POSSÉDEZ, PAYEZ 115M.")
    {
    }

    protected override IEnumerator TriggerEffect(MonopolyPlayer monopolyPlayer)
    {
        int houseRepairCost = 40;
        int hotelRepairCost = 115;
        int totalRepairCost;
        int housesNumber = monopolyPlayer.GetAllHousesNumber();
        int hotelsNumber = monopolyPlayer.GetAllHotelsNumber();
        if (housesNumber > 0 && hotelsNumber > 0)
        {
            monopolyGameManager.SetGameTextEventsText($"{monopolyPlayer.name} detient {housesNumber} maisons et {hotelsNumber} hôtels.");
        }
        else if (housesNumber > 0)
        {
            monopolyGameManager.SetGameTextEventsText($"{monopolyPlayer.name} detient {housesNumber} maisons.");
        }
        else if (hotelsNumber > 0)
        {
            monopolyGameManager.SetGameTextEventsText($"{monopolyPlayer.name} detient {hotelsNumber} hôtels.");
        }
        else
        {
            monopolyGameManager.SetGameTextEventsText($"{monopolyPlayer.name} ne detient aucune maison ni hôtel.");
        }
        
        yield return new WaitForSeconds(1.5f);
        
        if (housesNumber > 0 || hotelsNumber > 0)
        {
            totalRepairCost = houseRepairCost * housesNumber + hotelRepairCost * hotelsNumber;
            monopolyGameManager.SetGameTextEventsText($"{monopolyPlayer.name} doit payer {totalRepairCost}M à la banque.");
            yield return new WaitForSeconds(.5f);
            yield return monopolyGameManager.PlayerMustPayToBank(monopolyPlayer, totalRepairCost);
            yield return new WaitForSeconds(1.5f);
        }
    }
}
public class BakeSaleCard : CommunityCard
{
    public BakeSaleCard(MonopolyGameManager monopolyGameManager)
        : base( monopolyGameManager, "VOUS ORGANISEZ UNE VENTE DE PÂTISSERIES POUR L'ÉCOLE DE VOTRE QUARTIER.\n\nRECEVEZ 25M.")
    {
    }

    protected override IEnumerator TriggerEffect(MonopolyPlayer monopolyPlayer)
    {
        monopolyPlayer.HaveWon(25);
        monopolyGameManager.SetGameTextEventsText(
            $"La banque verse 25M à {monopolyPlayer.name}.");
        yield return new WaitForSeconds(1f);
    }
}
public class NeighborhoodPartyCard : CommunityCard
{
    public NeighborhoodPartyCard(MonopolyGameManager monopolyGameManager)
        : base( monopolyGameManager, "VOUS ORGANISEZ UNE FÊTE DE QUARTIER POUR QUE LES HABITANTS DE VOTRE RUE PUISSENT FAIRE CONNAISSANCE ENTRE EUX.\n\nRECEVEZ 10M DE CHAQUE JOUEUR.")
    {
    }

    protected override IEnumerator TriggerEffect(MonopolyPlayer monopolyPlayer)
    {
        monopolyGameManager.SetGameTextEventsText(
            $"Chaque joueur doit payer 10M à {monopolyPlayer.name}.");
        yield return monopolyGameManager.AllPlayersPayToPlayer(monopolyPlayer, 10);
        yield return new WaitForSeconds(1f);
    }
}
public class PlaygroundDonationCard : CommunityCard
{
    public PlaygroundDonationCard(MonopolyGameManager monopolyGameManager)
        : base( monopolyGameManager, "VOUS AVEZ AIDÉ À CONSTRUIRE UN NOUVEAU TERRAIN DE JEUX POUR L'ÉCOLE. VOUS ESSAYEZ LE TOBOGAN !\n\nRECEVEZ 100M.")
    {
    }

    protected override IEnumerator TriggerEffect(MonopolyPlayer monopolyPlayer)
    {
        monopolyPlayer.HaveWon(100);
        monopolyGameManager.SetGameTextEventsText(
            $"La banque verse 100M à {monopolyPlayer.name}.");
        yield return new WaitForSeconds(1f);
    }
}

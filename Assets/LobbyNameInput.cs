using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;

public class LobbyNameInput : MonoBehaviour
{
    public ManageRoom ManageRoom;
    public List<ButtonHandler> buttonsHandler;
    public List<BaseTextHandler> textsHandler;
    [SerializeField] private TMP_InputField inputField; // Drag TMP_InputField here in Inspector
    [SerializeField]
    private List<string> nicknameList = new List<string>
    {
        "MaîtreDuMonopole", "RoiDesRues", "LeStratège", "ChanceuxDuDé", "ReineDeLaFortune", 
        "CapitaineDesGares", "SeigneurDesPropriétés", "ExpertEnImmobilier", "GrandBanquier", 
        "MonarqueDeLaRicheesse", "MagnatDuCommerce", "ArchitecteDeLaVictoire", 
        "InvestisseurFou", "BaronDesHôtels", "LeParieur", "ConquérantDesTerrains", 
        "LeMarchand", "AcheteurSauvage", "PropriétaireUltime", "Spéculateur", 
        "ReineDuCarreau", "PrinceDesJardins", "ÉconomisteEnChef", "BanquierSansLimites", 
        "MaîtreDesTransactions", "CollectionneurDePropriétés", "LeProfiteur", 
        "EmpereurDesQuartiers", "ReineDeLaRue", "ExpertDuMonopole", "FouDuHôtel", 
        "GénieDesInvestissements", "LeCalculateur", "L'Astucieux", "RoiDeL'échange", 
        "ChampionDuCapital", "PropriétaireEnChef", "SageDuCommerce", "LeMarchandDeRêves", 
        "DominateurDeLaChance", "AsDeL'immobilier", "RoiDesContrats", "VendeurD'élite", 
        "MaîtreDeL'enchère", "CollectionneurDeRues", "ReineDesDéfis", "RoiDesJetons", 
        "SeigneurDesHôtels", "ArchitecteDeLaVille", "MagnatDesTerrains", "ChevalierDuCarreau", 
        "LeMaîtreDesCartes", "MarchandDeFortune", "DameDesTransactions", 
        "MagicienDuMonopole", "CavalierDeLaRicheesse", "ReineDesQuartiers", 
        "EmpereurDeLaChance", "DominateurDuPlateau", "HérosDuDé", "LeVisionnaire", 
        "LeSage", "PrinceDesHôtels", "ReineDuCommerce", "ChampionDeLaRue", 
        "SultanDuMonopole", "LeMaîtreDesAffaires", "LeFinancier", "L'InvestisseurMystique", 
        "GrandSpéculateur", "PropriétaireRoyal", "ReineDesAffaires", "RoiDesQuartiers", 
        "VendeurDeLuxe", "ÉconomisteLégendaire", "LeConquérant", "MaîtreDesGares", 
        "RoiDesRivaux", "ChevalierDuCapital", "SeigneurDesTransactions", 
        "L'Intouchable", "ReineDuPlateau", "ChampionDesDéfis", "BaronDeLaRue", 
        "MaîtreDeLaBanque", "LeGrandPropriétaire", "EmpereurDuCommerce", 
        "RoiDesAffaires", "PrinceDeLaRue", "MaîtreDesTerrains", "ReineDeL'hôtel", 
        "DominateurDesEnchères", "GrandStratège", "LeMillionnaire", "GéantDesQuartiers"
    }; // Liste de surnoms inspirés par Monopoly
    private void Awake()
    {
        if (inputField == null)
        {
            Debug.LogError("TMP_InputField is not assigned in the Inspector.");
            return;
        }
        buttonsHandler.Select((button, index) => new { button, index }) // Attach the index to each item
            .ToList().ForEach(item =>inputField.onValueChanged.AddListener(inputStr => 
            item.button.SetInteractableText(
                textsHandler[item.index].GetText().Trim().Length > 3 && inputStr.Trim().Length > 3 ? "XXX" : ""
            )));
        inputField.onValueChanged.AddListener(ManageRoom.UpdateNickname);
        
        SetRandomNickname();
    }
    public void SetRandomNickname()
    {
        int randomIndex = Random.Range(0, nicknameList.Count);
        inputField.text = nicknameList[randomIndex];
    }
}

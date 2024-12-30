using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayersUIManager : MonoBehaviour
{
    public Transform tabParent; // Parent for tabs
    public Transform contentParent; // Parent for tab content (should be a vertical scroll view)
    public Font defaultFont; // Assign a font for dynamically created text

    // Sample data for players (replace with your actual game data)
    private List<PlayerData> players = new List<PlayerData>();

    private void Start()
    {
        // Example data, replace with actual game logic
        players.Add(new PlayerData("Player 1", 1500, 0, "Park Lane", new List<string> { "Card 1", "Card 2" }, "5, 6", "Throw Dice"));
        players.Add(new PlayerData("Player 2", 1200, 1, "Mayfair", new List<string> { "Card A", "Card B" }, "3, 4", "Pay Rent"));

        GenerateTabs();
    }

    private void GenerateTabs()
    {
        foreach (var player in players)
        {
            // Create Tab Button
            GameObject tabButton = CreateButton(player.Name, tabParent);
            tabButton.GetComponent<Button>().onClick.AddListener(() => ShowPlayerInfo(player));

            // Create Player Tab Content
            GameObject playerTabContent = new GameObject(player.Name + " Content");
            playerTabContent.transform.SetParent(contentParent);
            playerTabContent.AddComponent<VerticalLayoutGroup>();
            playerTabContent.SetActive(false);

            // Fill Player Data
            CreateText($"Money: ${player.Money}", playerTabContent.transform);
            CreateText($"Jail Count: {player.JailCount}", playerTabContent.transform);
            CreateText($"Current Place: {player.CurrentPlace}", playerTabContent.transform);
            CreateText($"Last Thrown: {player.LastThrown}", playerTabContent.transform);
            CreateText($"Actions: {player.PossibleActions}", playerTabContent.transform);

            // Create Vertical Scroll Area for Cards
            GameObject scrollArea = CreateScrollArea(playerTabContent.transform);
            foreach (string card in player.CardsInPossession)
            {
                CreateText(card, scrollArea.transform);
            }
        }
    }

    private GameObject CreateButton(string text, Transform parent)
    {
        GameObject buttonObj = new GameObject(text + " Button");
        buttonObj.transform.SetParent(parent);

        Button button = buttonObj.AddComponent<Button>();
        buttonObj.AddComponent<Image>(); // Required for Button

        GameObject buttonText = CreateText(text, buttonObj.transform);
        buttonText.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;

        return buttonObj;
    }

    private GameObject CreateText(string content, Transform parent)
    {
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(parent);

        Text text = textObj.AddComponent<Text>();
        text.font = defaultFont;
        text.text = content;
        text.fontSize = 14;
        text.color = Color.black;
        text.alignment = TextAnchor.MiddleLeft;

        RectTransform rect = text.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(200, 30);

        return textObj;
    }

    private GameObject CreateScrollArea(Transform parent)
    {
        GameObject scrollObj = new GameObject("ScrollArea");
        scrollObj.transform.SetParent(parent);

        ScrollRect scrollRect = scrollObj.AddComponent<ScrollRect>();
        scrollObj.AddComponent<Image>(); // Background for Scroll Area

        GameObject contentObj = new GameObject("Content");
        contentObj.transform.SetParent(scrollObj.transform);
        contentObj.AddComponent<VerticalLayoutGroup>();

        scrollRect.content = contentObj.GetComponent<RectTransform>();
        return contentObj;
    }

    private void ShowPlayerInfo(PlayerData player)
    {
        // Hide all contents first
        foreach (Transform child in contentParent)
        {
            child.gameObject.SetActive(false);
        }

        // Show selected player content
        Transform selectedContent = contentParent.Find(player.Name + " Content");
        if (selectedContent != null)
        {
            selectedContent.gameObject.SetActive(true);
        }
    }
}

[System.Serializable]
public class PlayerData
{
    public string Name;
    public int Money;
    public int JailCount;
    public string CurrentPlace;
    public List<string> CardsInPossession;
    public string LastThrown;
    public string PossibleActions;

    public PlayerData(string name, int money, int jailCount, string currentPlace, List<string> cardsInPossession, string lastThrown, string possibleActions)
    {
        Name = name;
        Money = money;
        JailCount = jailCount;
        CurrentPlace = currentPlace;
        CardsInPossession = cardsInPossession;
        LastThrown = lastThrown;
        PossibleActions = possibleActions;
    }
}
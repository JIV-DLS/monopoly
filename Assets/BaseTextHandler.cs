using System;
using Monopoly;
using UnityEngine;
using TMPro;

public interface IInitComponent
{
    public void Init();
    public bool IsAlreadyInitialized();
}

public class MonoBehaviourWithInitComponent : MonoBehaviour, IInitComponent
{
    private bool _isInitialized = false;
    public void Init()
    {
        if (IsAlreadyInitialized())
        {
            return;
        }

        _isInitialized = true;
        OtherInit();
    }

    public virtual void OtherInit()
    {
        
    }

    public bool IsAlreadyInitialized()
    {
        return _isInitialized;
    }
}
public class BaseTextHandler : MonoBehaviourWithInitComponent
{
    protected TextMeshProUGUI textComponent;

    // Public variable to define the maximum number of lines
    public int maxLines = -1; // Default to -1 (no limit)

    public void Enable()
    {
        gameObject.SetActive(true);
    }

    protected virtual void Awake()
    {
        Init();
    }

    public override void OtherInit()
    {
        base.OtherInit();
        // Get the Text component
        textComponent = GetComponentInChildren<TextMeshProUGUI>(true);

        if (textComponent == null)
        {
            Debug.LogError("Text component not found!");
        }
    }


    // Virtual method to set text (can be overridden in child classes)
    public virtual void SetText(string newText)
    {
        if (textComponent != null)
        {
            textComponent.text = TruncateToMaxLines(newText);
        }
    }

    // Virtual method to append text
    public virtual void AppendText(string additionalText)
    {
        if (textComponent != null)
        {
            if (textComponent.text.Length > 1)
            {
                additionalText = "\n\n" + additionalText;
            }
            string combinedText = textComponent.text + additionalText;
            textComponent.text = TruncateToMaxLines(combinedText);
        }
    }

    // Virtual method to clear text
    public virtual void ClearText()
    {
        if (textComponent != null)
        {
            textComponent.text = string.Empty;
        }
    }

    // Helper method to truncate the text to the specified maximum number of lines
    private string _TruncateToMaxLines(string text)
    {
        if (maxLines < 1 || textComponent == null)
        {
            // No limit on lines
            return text;
        }

        // Set the text temporarily to calculate line count
        textComponent.text = text;

        // Use TMP's internal line info to determine visible lines
        textComponent.ForceMeshUpdate(); // Ensure the mesh is updated
        int lineCount = textComponent.textInfo.lineCount;

        if (lineCount <= maxLines)
        {
            return text; // Text fits within the max lines
        }

        // Extract the portion of the text that fits within maxLines
        string truncatedText = "";
        for (int i = 0; i < maxLines; i++)
        {
            var lineInfo = textComponent.textInfo.lineInfo[i];
            truncatedText += text.Substring(lineInfo.firstCharacterIndex, lineInfo.characterCount);
        }

        return truncatedText.TrimEnd(); // Return the truncated text without trailing spaces
    }
    private string TruncateToMaxLines(string text)
    {
        if (maxLines < 1 )//|| textComponent == null
        {
            // No limit on lines
            return text;
        }

        // Set the text temporarily to calculate line count
        textComponent.text = text;

        // Force update to get accurate text info
        // textComponent.ForceMeshUpdate();
        int lineCount = textComponent.textInfo.lineCount;

        if (lineCount <= maxLines)
        {
            return text; // Text fits within the max lines
        }

        // Extract the portion of the text that fits within maxLines, from the end
        string truncatedText = "";
        for (int i = lineCount - maxLines; i < lineCount; i++)
        {
            var lineInfo = textComponent.textInfo.lineInfo[i];
            truncatedText += text.Substring(lineInfo.firstCharacterIndex, lineInfo.characterCount);
        }

        return truncatedText.TrimEnd(); // Return the truncated text without trailing spaces
    }
}

public class BaseTextHandlerWithMonopolyPlayer : BaseTextHandler
{
    
    protected MonopolyPlayer SelfMadePlayer;

    public void SetSelfMadePlayer(MonopolyPlayer selfMadePlayer)
    {
        SelfMadePlayer = selfMadePlayer;
    }
}
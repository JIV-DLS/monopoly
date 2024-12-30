using UnityEngine;
using TMPro;

public class BaseTextHandler : MonoBehaviour
{
    protected TextMeshProUGUI textComponent;

    protected virtual void Awake()
    {
        // Get the Text component
        textComponent = GetComponent<TextMeshProUGUI>();

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
            textComponent.text = newText;
        }
    }

    // Virtual method to append text
    public virtual void AppendText(string additionalText)
    {
        if (textComponent != null)
        {
            textComponent.text += additionalText;
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
}

public class BaseTextHandlerWithSelfMadePlayer : BaseTextHandler
{
    
    protected SelfmadePlayer selfmadePlayer;

    public void SetSelfMadePlayer(SelfmadePlayer selfmadePlayer)
    {
        this.selfmadePlayer = selfmadePlayer;
    }
}
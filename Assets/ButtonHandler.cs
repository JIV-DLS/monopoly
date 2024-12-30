using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonHandler : MonoBehaviour
{
    protected Button button;
    private TextMeshProUGUI buttonText;

    protected virtual void Awake()
    {
        // Get the Button component
        button = GetComponent<Button>();

        if (button == null)
        {
            Debug.LogError("Button component not found!");
            return;
        }

        // Add a listener to handle button clicks
        button.onClick.AddListener(OnButtonClick);
        
        

        // Get the Text component of the Button (if it exists)
        buttonText = button.GetComponentInChildren<TextMeshProUGUI>();

        if (buttonText == null)
        {
            Debug.LogError("Text component not found as a child of the Button!");
        }

        // Add a listener to handle button clicks
        button.onClick.AddListener(OnButtonClick);
    }

    protected virtual void OnDestroy()
    {
        // Remove the listener to avoid memory leaks
        if (button != null)
        {
            button.onClick.RemoveListener(OnButtonClick);
        }
    }

    // Virtual method to handle button click (to be overridden in child classes)
    protected virtual void OnButtonClick()
    {
        Debug.Log("Base button clicked!");
    }

    // Example method to toggle the button's interactable property
    public void SetButtonInteractable(bool isInteractable)
    {
        if (button != null)
        {
            button.interactable = isInteractable;
        }
    }

    // Method to set the button's text
    public void SetButtonText(string text)
    {
        if (buttonText != null)
        {
            buttonText.text = text;
        }
        else
        {
            Debug.LogWarning("Button text component is not assigned!");
        }
    }
}


public class ButtonHandlerWithSelfMadePlayer : ButtonHandler
{
    
    protected SelfmadePlayer selfmadePlayer;

    public void SetSelfMadePlayer(SelfmadePlayer selfmadePlayer)
    {
        this.selfmadePlayer = selfmadePlayer;
    }
}
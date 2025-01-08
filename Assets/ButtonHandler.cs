using UnityEngine;
using TMPro;using UnityEngine.UIElements;using Button = UnityEngine.UI.Button;

public interface IClickableButtonHandler{
    public void OnClick();
}
public class ButtonHandler : MonoBehaviourWithInitComponent
{
    private Button _button;
    private TextMeshProUGUI _buttonText;
    public IClickableButtonHandler Handler;
    
    protected virtual void Awake()
    {
        Init();
    }

    protected virtual void OnDestroy()
    {
        // Remove the listener to avoid memory leaks
        if (_button != null)
        {
            _button.onClick.RemoveListener(OnButtonClick);
        }
    }

    // Virtual method to handle button click (to be overridden in child classes)
    protected virtual void OnButtonClick()
    {
        if (Handler != null)
        {
            Handler.OnClick();
        }
        else
        {
            Debug.Log("Base button clicked!");
        }
    }

    // Example method to toggle the button's interactable property
    public void SetButtonInteractable(bool isInteractable)
    {
        if (!ReferenceEquals(_button, null))
        {
            _button.interactable = isInteractable;
        }
    }

    // Method to set the button's text
    public void SetButtonText(string text)
    {
        if (!ReferenceEquals(_buttonText, null))
        {
            _buttonText.text = text;
        }
        else
        {
            // Debug.LogWarning("Button text component is not assigned!");
        }
    }

    public override void OtherInit()
    {
        
        // Get the Button component
        _button = GetComponent<Button>();

        if (_button == null)
        {
            Debug.LogError("Button component not found!");
            return;
        }
        // Add a listener to handle button clicks
        _button.onClick.AddListener(OnButtonClick);
        
        

        // Get the Text component of the Button (if it exists)
        _buttonText = _button.GetComponentInChildren<TextMeshProUGUI>();

        if (_buttonText == null)
        {
            Debug.LogWarning("Text component not found as a child of the Button!");
        }

    }
}


public class ButtonHandlerWithMonopolyPlayer : ButtonHandler
{
    
    protected MonopolyPlayer MonopolyPlayer;

    public void SetSelfMadePlayer(MonopolyPlayer selfMadePlayer)
    {
        MonopolyPlayer = selfMadePlayer;
    }
}
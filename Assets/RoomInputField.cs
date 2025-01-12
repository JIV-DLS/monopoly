using UnityEngine;
using TMPro;
using UnityEngine.Serialization;

public class RoomInputField : MonoBehaviour
{
    public ButtonHandler buttonHandler;
    [SerializeField] private TMP_InputField inputField; // Drag TMP_InputField here in Inspector

    private void Start()
    {
        if (inputField == null)
        {
            Debug.LogError("TMP_InputField is not assigned in the Inspector.");
            return;
        }

        // Example: Print the initial text
        Debug.Log("Initial Text: " + inputField.text);

        // Subscribe to text changes
        inputField.onValueChanged.AddListener(buttonHandler.SetInteractableText);
    }

}

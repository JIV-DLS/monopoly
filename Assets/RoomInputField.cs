using UnityEngine;
using TMPro;
using UnityEngine.Serialization;

public class RoomInputField : MonoBehaviour
{
    public ButtonHandler buttonHandler;
    public BaseTextHandler textHandler;

    [SerializeField] private TMP_InputField inputField; // Drag TMP_InputField here in Inspector

    private void Awake()
    {
        if (inputField == null)
        {
            Debug.LogError("TMP_InputField is not assigned in the Inspector.");
            return;
        }

        // Example: Print the initial text
        Debug.Log("Initial Text: " + inputField.text);

        // Subscribe to text changes
        inputField.onValueChanged.AddListener(inputStr =>buttonHandler.SetInteractableText(
            textHandler.GetText().Trim().Length > 3 && inputStr.Trim().Length > 3 ? "XXX" : ""
        ));
    }

}

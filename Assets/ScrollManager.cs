using UnityEngine;
using UnityEngine.UI;

public class ScrollManager : MonoBehaviour
{
    private AutoHeightBaseTextHandler text; // Base class to handle both Vertical and Horizontal Layout Groups

    void Awake()
    {
        // Find a LayoutGroup in the current GameObject's hierarchy
        text = GetComponentInChildren<AutoHeightBaseTextHandler>();
        if (text == null)
        {
            Debug.LogError("No BaseTextHandler text found in children.");
        }
    }

    public void AddTextItem(string textContent)
    {
        text.AppendText(textContent);
    }
}
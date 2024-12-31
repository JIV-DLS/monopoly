using UnityEngine;
using UnityEngine.UI;

public class LayoutManager : MonoBehaviour
{
    private LayoutGroup layoutGroupParent; // Base class to handle both Vertical and Horizontal Layout Groups
    public BaseTextHandler textPrefab; // Prefab to duplicate

    void Awake()
    {
        // Find a LayoutGroup in the current GameObject's hierarchy
        layoutGroupParent = GetComponentInChildren<LayoutGroup>();
        if (layoutGroupParent == null)
        {
            Debug.LogError("No LayoutGroup (Vertical or Horizontal) found in the children of this GameObject.");
        }
    }

    public void AddTextItem(string textContent)
    {
        if (textPrefab == null || layoutGroupParent == null)
        {
            Debug.LogError("TextPrefab or LayoutGroupParent is not set correctly!");
            return;
        }

        // Instantiate the prefab as a child of the layout group
        BaseTextHandler textComponent = Instantiate(textPrefab, layoutGroupParent.transform);
        textComponent.Enable();
        // Set the text content
        if (textComponent != null)
        {
            textComponent.SetText(textContent);
        }
        else
        {
            Debug.LogError("The prefab does not have a Text component!");
        }
    }
}
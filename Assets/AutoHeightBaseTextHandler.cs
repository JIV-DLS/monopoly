using UnityEngine;
using TMPro;

public class AutoHeightBaseTextHandler : BaseTextHandler
{
    // Optional padding to add around the text
    [SerializeField]
    private float verticalPadding = 10f;

    protected override void Awake()
    {
        base.Awake();

        // Ensure the RectTransform is available
        if (textComponent != null && textComponent.rectTransform == null)
        {
            Debug.LogError("RectTransform for TextMeshProUGUI component is missing!");
        }
    }

    public override void SetText(string newText)
    {
        base.SetText(newText);
        AdjustHeight();
    }

    public override void AppendText(string additionalText)
    {
        base.AppendText(additionalText);
        AdjustHeight();
    }

    private void AdjustHeight()
    {
        if (textComponent != null)
        {
            // Get the preferred height of the text
            float preferredHeight = textComponent.preferredHeight;

            // Access the RectTransform to modify its size
            RectTransform rectTransform = textComponent.rectTransform;

            if (rectTransform != null && rectTransform.sizeDelta.y < preferredHeight + verticalPadding)
            {
                // Adjust the height to fit the content plus optional padding
                Vector2 newSize = rectTransform.sizeDelta;
                newSize.y = preferredHeight + verticalPadding;
                rectTransform.sizeDelta = newSize;
            }
        }
    }
}
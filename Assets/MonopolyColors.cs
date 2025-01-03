using UnityEngine;

public static class MonopolyColors
{
    // Enum for property color groups
    public enum PropertyColor
    {
        Brown,
        LightBlue,
        Pink,
        Orange,
        Red,
        Yellow,
        Green,
        DarkBlue,
        None // For railroads or utilities
    }

    // Static method to map enum values to Color
    public static Color GetColor(PropertyColor color)
    {
        switch (color)
        {
            case PropertyColor.Brown:
                return new Color(177f / 255f, 101f / 255f, 63f / 255f, 1f); // Brown
            case PropertyColor.LightBlue:
                return new Color(197f / 255f, 255f / 255f, 255f / 255f, 1f); // Light Blue
            case PropertyColor.Pink:
                return new Color(255f / 255f, 71f / 255f, 184f / 255f, 1f); // Pink
            case PropertyColor.Orange:
                return new Color(255f / 255f, 175f / 255f, 38f/ 255f, 1f); // Orange
            case PropertyColor.Red:
                return new Color(255f / 255f, 35f/ 255f, 47f/ 255f, 1f); // Red
            case PropertyColor.Yellow:
                return new Color(255f / 255f, 255f / 255f, 2f/ 255f, 1f); // Yellow
            case PropertyColor.Green:
                return new Color(37f/255f, 214f / 255f, 108f/255f, 1f); // Green
            case PropertyColor.DarkBlue:
                return new Color(0f, 135f / 255f, 230f / 255f, 1f); // Dark Blue
            case PropertyColor.None:
                return Color.gray; // Default for railroads/utilities
            default:
                return Color.white; // Fallback color
        }
    }
}

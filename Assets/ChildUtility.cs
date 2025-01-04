
using UnityEngine;

public static class ChildUtility
{
    /// <summary>
    /// Finds a child GameObject by name and gets the specified component from it.
    /// </summary>
    /// <typeparam name="T">Type of the component to get (e.g., a script).</typeparam>
    /// <param name="parent">The parent Transform to search under.</param>
    /// <param name="childName">The name of the child GameObject.</param>
    /// <returns>The component of type T if found, or null if not found.</returns>
    public static T GetChildComponentByName<T>(Transform parent, string childName) where T : Component
    {
        if (parent == null)
        {
            Debug.LogWarning("Parent transform is null.");
            return null;
        }

        // Recursive method to find the child by name
        Transform FindChildRecursive(Transform currentParent, string targetName)
        {
            foreach (Transform child in currentParent)
            {
                if (child.name == targetName)
                    return child;

                Transform found = FindChildRecursive(child, targetName);
                if (found != null)
                    return found;
            }

            return null; // Not found
        }

        Transform child = FindChildRecursive(parent, childName);

        if (child == null)
        {
            Debug.LogWarning($"Child '{childName}' not found recursively under parent '{parent.name}'.");
            return null;
        }

        T component = child.GetComponent<T>();

        if (component == null)
        {
            Debug.LogWarning($"Component of type {typeof(T).Name} not found on child '{childName}'.");
        }

        return component;
    }

}

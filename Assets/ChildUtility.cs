
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

        Transform child = parent.Find(childName);

        if (child == null)
        {
            Debug.LogWarning($"Child '{childName}' not found under parent '{parent.name}'.");
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

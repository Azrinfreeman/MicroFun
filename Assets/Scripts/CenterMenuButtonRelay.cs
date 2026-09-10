using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Lets a redesigned menu button invoke the serialized listeners of a retained
/// legacy button, preserving scene navigation without duplicating callback data.
/// </summary>
public class CenterMenuButtonRelay : MonoBehaviour
{
    [SerializeField] private Button sourceButton;

    public void InvokeSource()
    {
        if (sourceButton == null)
        {
            Debug.LogWarning(name + " has no source button assigned.", this);
            return;
        }

        sourceButton.onClick.Invoke();
    }
}

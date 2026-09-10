using UnityEngine;
using UnityEngine.UI;

public class AlgaGeneratedButtonRoute : MonoBehaviour
{
    public int action;

    void Awake()
    {
        var button = GetComponent<Button>();
        if (button == null) return;
        button.onClick.AddListener(HandleClick);
    }

    void HandleClick()
    {
        var panel = FindAlgaPanel();
        if (panel == null) return;

        if (action == 0)
        {
            var dialog = panel.transform.Find("DialogParent");
            if (dialog != null) dialog.gameObject.SetActive(true);
            return;
        }

        var alga = panel.transform.parent;
        if (alga != null) alga.gameObject.SetActive(false);
        var all = Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var candidate in all)
        {
            if (candidate.name == "Center" && candidate.transform.parent != null && candidate.transform.parent.name == "Canvas")
            {
                candidate.SetActive(true);
                break;
            }
        }
    }

    Transform FindAlgaPanel()
    {
        var current = transform;
        while (current != null)
        {
            if (current.name == "1" && current.parent != null && current.parent.name == "Alga") return current;
            current = current.parent;
        }
        return null;
    }
}

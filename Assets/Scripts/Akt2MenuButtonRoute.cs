using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class Akt2MenuButtonRoute : MonoBehaviour
{
    [SerializeField] private int legacyButtonIndex;
    [SerializeField] private bool hideCenterBeforeInvoke;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(InvokeLegacyRoute);
    }

    private void OnDestroy()
    {
        var button = GetComponent<Button>();
        if (button != null) button.onClick.RemoveListener(InvokeLegacyRoute);
    }

    private void InvokeLegacyRoute()
    {
        var canvas = GameObject.Find("Canvas");
        if (canvas == null) return;

        var center = canvas.transform.Find("Center");
        if (center == null) return;

        var legacyMenu = center.Find("GameObject");
        if (legacyMenu == null) return;

        var buttons = legacyMenu.GetComponentsInChildren<Button>(true);
        if (legacyButtonIndex < 0 || legacyButtonIndex >= buttons.Length) return;

        if (hideCenterBeforeInvoke)
            center.gameObject.SetActive(false);

        buttons[legacyButtonIndex].onClick.Invoke();
    }
}
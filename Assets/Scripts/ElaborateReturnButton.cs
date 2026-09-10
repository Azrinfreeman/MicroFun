using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Returns from elaborate1 to a specific panel in the akt1 scene.
/// Works whether elaborate1 was opened additively or previewed by itself.
/// </summary>
[RequireComponent(typeof(Button))]
public class ElaborateReturnButton : MonoBehaviour
{
    public enum ReturnDestination
    {
        SubMenu,
        ActivityHome
    }

    [SerializeField] private ReturnDestination destination;

    private Button button;
    private static ReturnDestination pendingDestination;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(Navigate);
    }

    private void OnDestroy()
    {
        if (button != null) button.onClick.RemoveListener(Navigate);
    }

    private void Navigate()
    {
        if (button != null) button.interactable = false;

        Scene activityScene = SceneManager.GetSceneByName("akt1");
        if (activityScene.IsValid() && activityScene.isLoaded)
        {
            ConfigureActivityScene(activityScene, destination);
            if (gameObject.scene != activityScene)
                SceneManager.UnloadSceneAsync(gameObject.scene);
            return;
        }

        pendingDestination = destination;
        SceneManager.sceneLoaded -= HandleActivitySceneLoaded;
        SceneManager.sceneLoaded += HandleActivitySceneLoaded;
        SceneManager.LoadScene("akt1", LoadSceneMode.Single);
    }

    private static void HandleActivitySceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "akt1") return;

        SceneManager.sceneLoaded -= HandleActivitySceneLoaded;
        ConfigureActivityScene(scene, pendingDestination);
    }

    private static void ConfigureActivityScene(Scene activityScene, ReturnDestination target)
    {
        GameObject canvas = null;

        foreach (GameObject root in activityScene.GetRootGameObjects())
        {
            if (root.name == "Canvas") canvas = root;
            if (root.name == "Main Camera" || root.name == "EventSystem")
                root.SetActive(true);
        }

        if (canvas == null) return;

        canvas.SetActive(true);
        string[] panels =
        {
            "LandingPage", "SubMenu", "Pengenalan", "Definisi",
            "Jenis", "Fungi", "Bakteria"
        };

        foreach (string panelName in panels)
        {
            Transform panel = canvas.transform.Find(panelName);
            if (panel != null) panel.gameObject.SetActive(false);
        }

        string targetPanel = target == ReturnDestination.SubMenu
            ? "SubMenu"
            : "LandingPage";

        Transform destinationPanel = canvas.transform.Find(targetPanel);
        if (destinationPanel != null) destinationPanel.gameObject.SetActive(true);
    }
}
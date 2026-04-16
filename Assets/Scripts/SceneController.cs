using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Drag the Canvas from your current (old) scene here.")]
    public GameObject oldCanvas;
    public GameObject oldCamera;

    public GameObject oldEventSystem;

    // We store the name of the added scene so we can safely unload it later
    private string addedSceneName;

    public void AddScene(string sceneName)
    {
        addedSceneName = sceneName;

        // 1. Hide the old canvas before loading the new scene
        if (oldCanvas != null)
        {
            oldCanvas.SetActive(false);
            oldCamera.SetActive(false);
            oldEventSystem.SetActive(false);
        }

        // 2. Load the new scene additively
        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
    }

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void UnloadScene()
    {
        // 1. Unload the new scene we added earlier
        if (!string.IsNullOrEmpty(addedSceneName))
        {
            SceneManager.UnloadSceneAsync(addedSceneName);
            addedSceneName = null;
        }

        // 2. Show the old canvas again
        if (oldCanvas != null)
        {
            oldCanvas.SetActive(true);
            oldCamera.SetActive(true);
            oldEventSystem.SetActive(true);
        }
    }
}

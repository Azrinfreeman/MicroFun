using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


/// <summary>
/// Returns from an additive learning page, or loads the fallback page when this
/// scene is being previewed on its own.
/// </summary>
[RequireComponent(typeof(Button))]
public class BackController : MonoBehaviour
{
    [SerializeField] private string fallbackSceneName = "akt1";

    private Button button;
    private bool isNavigating;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(ReturnToPreviousPage);
    }

    private void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(ReturnToPreviousPage);
        }
    }

    private void ReturnToPreviousPage()
    {
        if (isNavigating)
        {
            return;
        }

        isNavigating = true;
        button.interactable = false;

        if (SceneManager.sceneCount > 1 && gameObject.scene.isLoaded)
        {
            var sceneCtrl = FindObjectOfType<SceneController>();
            if (sceneCtrl != null)
            {
                sceneCtrl.UnloadScene();
            }
            else
            {
                SceneManager.UnloadSceneAsync(gameObject.scene);
            }
            return;
        }

        SceneManager.LoadScene(fallbackSceneName, LoadSceneMode.Single);
    }
}

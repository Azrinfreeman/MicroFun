using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Attaching this to a button will load the configured target scene once.
/// </summary>
[RequireComponent(typeof(Button))]
public class NextController : MonoBehaviour
{
    [SerializeField] private string targetSceneName;

    private Button button;
    private bool isNavigating;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(LoadNextPage);
    }

    private void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(LoadNextPage);
        }
    }

    private void LoadNextPage()
    {
        if (isNavigating || string.IsNullOrWhiteSpace(targetSceneName))
        {
            return;
        }

        isNavigating = true;
        button.interactable = false;
        SceneManager.LoadScene(targetSceneName, LoadSceneMode.Single);
    }
}

using TMPro;
using UnityEngine;

public class PembentanganSectionController : MonoBehaviour
{
    [SerializeField] private GameObject topicPromptPanel;
    [SerializeField] private TextMeshProUGUI topicPromptText;

    public void Configure(GameObject panel, TextMeshProUGUI promptText)
    {
        topicPromptPanel = panel;
        topicPromptText = promptText;
    }

    public void ShowCiri()
    {
        ShowPrompt("Bentangkan ciri-ciri mikroorganisma yang dipilih.");
    }

    public void ShowContoh()
    {
        ShowPrompt("Berikan contoh mikroorganisma yang dipilih.");
    }

    public void ShowKepentingan()
    {
        ShowPrompt("Terangkan kepentingan mikroorganisma tersebut.");
    }

    public void HidePrompt()
    {
        if (topicPromptPanel != null)
        {
            topicPromptPanel.SetActive(false);
        }
    }

    private void ShowPrompt(string message)
    {
        if (topicPromptText != null)
        {
            topicPromptText.text = message;
        }

        if (topicPromptPanel != null)
        {
            topicPromptPanel.SetActive(true);
        }
    }
}

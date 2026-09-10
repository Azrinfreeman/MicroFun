using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogController : MonoBehaviour
{
    public List<string> dialogList;
    private TextMeshProUGUI textDisplay;
    public Button nextButton;
    public Button exitButton;
    public float typingSpeed = 0.05f;

    private string fullText;
    private string currentText = "";
    public int currentDialogIndex = 0;
    public bool PressToStart = false;

    // Track the coroutine to prevent multiple instances running
    private Coroutine typingCoroutine;

    // Awake is better for finding references
    void Awake()
    {
        nextButton = transform.Find("textDialog").GetChild(0).GetComponent<Button>();
        exitButton = transform.Find("ExitButton").GetComponent<Button>();
        textDisplay = transform.Find("textDialog").GetComponent<TextMeshProUGUI>();

        nextButton.onClick.AddListener(NextDialog);
    }

    // This runs EVERY time the object is set to Active
    void OnEnable()
    {
        // Reset or continue from the last index when the object is re-enabled
        if (dialogList != null && dialogList.Count > 0)
        {
            exitButton.transform.gameObject.SetActive(false);
            StartTypewriter(dialogList[currentDialogIndex]);
        }
    }

    public void StartTypewriter(string textToType)
    {
        if (PressToStart == false)
        {
            fullText = textToType;

            // If a routine is already running, stop it first
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);

            typingCoroutine = StartCoroutine(ShowText());
        }
    }

    IEnumerator ShowText()
    {
        nextButton.transform.gameObject.SetActive(false);
        textDisplay.text = ""; // Clear text at the start

        for (int i = 0; i <= fullText.Length; i++)
        {
            currentText = fullText.Substring(0, i);
            textDisplay.text = currentText;
            yield return new WaitForSeconds(typingSpeed);
        }

        // Logic for buttons after typing finishes
        if (currentDialogIndex == dialogList.Count - 1)
        {
            nextButton.transform.gameObject.SetActive(false);
            exitButton.transform.gameObject.SetActive(true);
        }
        else
        {
            nextButton.transform.gameObject.SetActive(true);
        }

        typingCoroutine = null; // Clear reference
    }

    public void NextDialog()
    {
        currentDialogIndex++;
        if (currentDialogIndex < dialogList.Count)
        {
            StartTypewriter(dialogList[currentDialogIndex]);
        }
    }

    public void StartingWrite()
    {
        PressToStart = false;
        StartTypewriter(dialogList[currentDialogIndex]);
    }
}

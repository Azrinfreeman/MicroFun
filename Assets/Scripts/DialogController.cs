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

    // If using an Arabic fixer plugin, you'd process the string first
    private string fullText;
    private string currentText = "";

    public int currentDialogIndex = 0;

    public bool PressToStart = false;

    public void StartTypewriter(string textToType)
    {
        if (PressToStart == false)
        {
            fullText = textToType;
            StartCoroutine(ShowText());
        }
    }

    IEnumerator ShowText()
    {
        nextButton.transform.gameObject.SetActive(false);
        for (int i = 0; i <= fullText.Length; i++)
        {
            // Substring gets the characters one by one
            currentText = fullText.Substring(0, i);
            textDisplay.text = currentText;

            // Wait for the delay
            yield return new WaitForSeconds(typingSpeed);
        }
        if (currentDialogIndex == dialogList.Count - 1)
        {
            nextButton.transform.gameObject.SetActive(false);
            exitButton.transform.gameObject.SetActive(true);
        }
        else
        {
            nextButton.transform.gameObject.SetActive(true);
        }
    }

    public void NextDialog()
    {
        currentText = "";
        currentDialogIndex++;
        if (currentDialogIndex < dialogList.Count)
        {
            StartTypewriter(dialogList[currentDialogIndex]);
        }
        else { }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentDialogIndex = 0;
        nextButton = transform.Find("textDialog").GetChild(0).GetComponent<Button>();
        exitButton = transform.Find("ExitButton").GetComponent<Button>();
        textDisplay = transform.Find("textDialog").GetComponent<TextMeshProUGUI>();
        exitButton.transform.gameObject.SetActive(false);
        nextButton.onClick.AddListener(NextDialog);

        StartTypewriter(dialogList[currentDialogIndex]); // Start with the first dialog in the list
    }

    public void StartingWrite()
    {
        PressToStart = false;
        StartTypewriter(dialogList[currentDialogIndex]);
    }

    // Update is called once per frame
    void Update() { }
}

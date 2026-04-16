using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class QuizController : MonoBehaviour
{
    public static QuizController instance;
    public static System.Random rnd = new System.Random();

    public static System.Random rnd1 = new System.Random();

    //questioon number is used to know which question is currently shown, so we can check if the answer is correct
    public int questionNumber;

    //button number is used to know which button was the answer, so we can check if the answer is correct
    public int buttonNumber;

    public int numberCount;

    public int totalQuestions;

    public Transform questionText;
    public Transform WinPanel;

    public List<Transform> buttonAnswers;
    public List<int> randomAnswer;
    public TextMeshProUGUI indicatorText;
    public AudioSource wrong;
    public AudioSource correct;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [System.Serializable]
    public class Question
    {
        public string questionText;
        public List<string> choices;
        public int correctChoicesIndex;

        public Question(string questionText, List<string> choices, int correctChoicesIndex)
        {
            this.questionText = questionText;
            this.choices = choices;
            this.correctChoicesIndex = correctChoicesIndex;
        }
    }

    public List<Question> questions = new List<Question>();

    public void showQuestion()
    {
        if (questions.Count > 0)
        {
            StartCoroutine(showQuest());
        }
        else
        {
            WinPanel.transform.gameObject.SetActive(true);
        }
    }

    IEnumerator showQuest()
    { //get random question number
        questionNumber = rnd.Next(0, questions.Count);
        //set question text and choices
        questionText.GetComponent<TMPro.TextMeshProUGUI>().text = questions[
            questionNumber
        ].questionText;

        //clear the button of any text and function
        for (int a = 0; a < 4; a++)
        {
            buttonAnswers[a].GetComponent<Button>().onClick.RemoveAllListeners();
            buttonAnswers[a]
                .GetComponent<Button>()
                .transform.GetChild(0)
                .GetComponent<TextMeshProUGUI>()
                .text = "";
            buttonAnswers[a].GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            //            Debug.Log("Clear button");
        }
        EnableAllButtons();
        //buttonTotal is used to generate random buttonNumber
        int buttonTotal = 0;
        //generate random button number
        buttonNumber = questions[questionNumber].correctChoicesIndex;

        //apply the right button along with text

        for (int i = 0; i < buttonAnswers.Count; i++)
        {
            //if buttons are more than choices, hide them, else show them and set text
            if (i > questions[questionNumber].choices.Count - 1)
            {
                buttonAnswers[i].gameObject.SetActive(false);
            }
            else
            {
                if (buttonNumber == i)
                {
                    buttonAnswers[buttonNumber]
                        .GetComponent<Button>()
                        .onClick.AddListener(() => CorrectButtonFunction(buttonNumber));
                    buttonAnswers[i].GetComponentInChildren<TMPro.TextMeshProUGUI>().text =
                        questions[questionNumber].choices[i];
                }

                buttonTotal++;
            }
        }

        //apply the wrong button
        for (int i = 0; i < buttonTotal; i++)
        {
            if (i != buttonNumber)
            {
                //the list
                int chooseRandom = rnd.Next(0, questions[questionNumber].choices.Count);

                //if list is empty (first time)
                if (randomAnswer.Count == 0)
                {
                    while (chooseRandom == questionNumber)
                    {
                        chooseRandom = rnd.Next(0, questions[questionNumber].choices.Count);
                    }
                    randomAnswer.Add(chooseRandom);
                }
                else
                {
                    // check if randomnumbers are duplicated in list
                    for (int l = 0; l < randomAnswer.Count; l++)
                    {
                        //                        Debug.Log("Check");
                        while (randomAnswer[l] == chooseRandom)
                        {
                            //          Debug.Log("Duplicated");
                            //generate new number if chooseRandom already in the list
                            chooseRandom = rnd.Next(0, questions[questionNumber].choices.Count);
                        }
                    }
                    //check if answers is already selected from the list
                    while (chooseRandom == questionNumber)
                    {
                        Debug.Log("Duplicated");
                        //generate new number if chooseRandom already in the list
                        chooseRandom = rnd.Next(0, questions[questionNumber].choices.Count);
                    }

                    //

                    randomAnswer.Add(chooseRandom);
                }
                buttonAnswers[i].GetComponent<Button>().onClick.RemoveAllListeners();
                buttonAnswers[i]
                    .GetComponent<Button>()
                    .onClick.AddListener(() => IncorrectButtonFunction());
                buttonAnswers[i].GetComponentInChildren<TMPro.TextMeshProUGUI>().text = questions[
                    questionNumber
                ].choices[i];
            }
        }
        numberCount++;
        indicatorText.text = numberCount + " / " + totalQuestions;

        //assign button
        yield return new WaitForSeconds(1);
    }

    IEnumerator incorrectAnswer()
    {
        DisableAllButtons();
        if (GameObject.Find("wrong").GetComponent<AudioSource>().isPlaying)
        {
            GameObject.Find("wrong").GetComponent<AudioSource>().Stop();
        }
        if (!GameObject.Find("wrong").GetComponent<AudioSource>().isPlaying)
        {
            GameObject.Find("wrong").GetComponent<AudioSource>().Play();
            // GameObject.Find("aww").GetComponent<AudioSource>().Play();
        }
        buttonAnswers[buttonNumber].GetComponent<Image>().color = new Color32(54, 217, 42, 255);
        // cross.gameObject.SetActive(true);
        questions.RemoveAt(questionNumber);
        yield return new WaitForSeconds(2f);
        // cross.gameObject.SetActive(false);

        //repeat
        RepeatQuestion();
    }

    public void CorrectButtonFunction(int buttonNum)
    {
        StartCoroutine(roundCollected(buttonNum));
    }

    public void IncorrectButtonFunction()
    {
        StartCoroutine(incorrectAnswer());
    }

    IEnumerator roundCollected(int buttonNum)
    {
        ScoreController.instance.AddScore(1);
        DisableAllButtons();
        if (GameObject.Find("rewarded").GetComponent<AudioSource>().isPlaying)
        {
            GameObject.Find("rewarded").GetComponent<AudioSource>().Stop();
        }
        if (!GameObject.Find("rewarded").GetComponent<AudioSource>().isPlaying)
        {
            GameObject.Find("rewarded").GetComponent<AudioSource>().Play();
            GameObject.Find("yay").GetComponent<AudioSource>().Play();
        }
        buttonAnswers[buttonNum].GetComponent<Image>().color = new Color32(54, 217, 42, 255);
        questions.RemoveAt(questionNumber);
        //post question
        //ScoreController.instance.AddScore(10);
        //tick.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        //tick.gameObject.SetActive(false);
        //EnableAllButtons();
        yield return new WaitForSeconds(0.3f);

        //repeat
        RepeatQuestion();
    }

    private void RepeatQuestion()
    {
        Debug.Log("repeat");

        showQuestion();
    }

    private void DisableAllButtons()
    {
        for (int a = 0; a < 4; a++)
        {
            buttonAnswers[a].GetComponent<Button>().enabled = false;
            buttonAnswers[a].GetComponent<Image>().color = new Color32(77, 99, 243, 255);
            //            Debug.Log("Clear button");
        }
    }

    void EnableAllButtons()
    {
        for (int a = 0; a < 4; a++)
        {
            buttonAnswers[a].GetComponent<Button>().enabled = true;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        totalQuestions = questions.Count;
        indicatorText.text = numberCount + " / " + totalQuestions;
        showQuestion();
    }

    // Update is called once per frame
    void Update() { }
}

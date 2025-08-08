using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.Collections;
public class QuizManager : MonoBehaviour
{
    public  TextMeshProUGUI questionText;
    public Button[] optionButtons;
    private List<Question> questions;
    private int currentQuestion = 0;
    private int score = 0;

    public Slider timerSlider;
    public float totalTime = 60f;
    private Coroutine timerCoroutine;
    public GameObject correctFeedbackPrefab;
    public GameObject incorrectFeedbackPrefab;
    public Transform feedbackSpawnPoint; 
    public GameObject certificate;
    public TextMeshProUGUI nameTXT;

    private void Start()
    {
        StartCoroutine(APIManager.Instance.FetchQuestions(OnQuestionsLoaded));
        timerSlider.maxValue = totalTime;
        timerSlider.value = totalTime;       
        timerCoroutine = StartCoroutine(StartTimer()); 
        certificate.SetActive(false);
    }

    void OnQuestionsLoaded(List<Question> loadedQuestions)
    {
        questions = loadedQuestions;
        DisplayQuestion();
    }

    void DisplayQuestion()
    {
        if (currentQuestion >= questions.Count)
        {
            SubmitFinalScore();
            return;
        }

        Question q = questions[currentQuestion];
        questionText.text = q.question;
        optionButtons[0].GetComponentInChildren<TextMeshProUGUI>().text = q.option_a;
        optionButtons[1].GetComponentInChildren<TextMeshProUGUI>().text = q.option_b;
        optionButtons[2].GetComponentInChildren<TextMeshProUGUI>().text = q.option_c;
        optionButtons[3].GetComponentInChildren<TextMeshProUGUI>().text = q.option_d;

        for (int i = 0; i < optionButtons.Length; i++)
        {
            int index = i;
            optionButtons[i].onClick.RemoveAllListeners();
            optionButtons[i].onClick.AddListener(() => OnOptionSelected(index));
        }
    }

    void OnOptionSelected(int index)
    {
        if (currentQuestion >= questions.Count)
            return;

        Question q = questions[currentQuestion];
        string selectedOption = index switch
        {
            0 => "A",
            1 => "B",
            2 => "C",
            3 => "D",
            _ => ""
        };

   

        bool isCorrect = selectedOption == q.correct_option;
        if (isCorrect)
        {
            score++;
            Debug.Log("Correct!");
            Destroy(Instantiate(correctFeedbackPrefab, feedbackSpawnPoint), 0.25f);

        }
        else
        {
            Debug.Log(" Incorrect");
            Destroy(Instantiate(incorrectFeedbackPrefab, feedbackSpawnPoint), 0.25f);

        }

        currentQuestion++;

        if (currentQuestion < questions.Count)
            DisplayQuestion();
        else
            SubmitFinalScore();
    }



    void SubmitFinalScore()
    {
        if (timerCoroutine != null)
            StopCoroutine(timerCoroutine);

        Debug.Log("Final Score: " + score);
        StartCoroutine(APIManager.Instance.SubmitFinalScore(score));
        nameTXT.text = "Name: "+APIManager.Instance.student_name;
        certificate.SetActive(true);
    }

    IEnumerator StartTimer()
    {
        float timeLeft = totalTime;

        while (timeLeft > 0f)
        {
            timeLeft -= Time.deltaTime;
            timerSlider.value= timeLeft;
            yield return null;
        }

        Debug.Log("⏰ Time's up!");
        SubmitFinalScore(); 
    }

}

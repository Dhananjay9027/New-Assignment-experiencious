using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class APIManager : MonoBehaviour
{
    public static APIManager Instance;
    private string baseURL = "http://localhost:8000"; // change to your deployed API
    public int student_id;
    public string student_name;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Prevent duplicates
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // ✅ Persist across scenes
    }

    public IEnumerator FetchQuestions(System.Action<List<Question>> callback)
    {
        UnityWebRequest request = UnityWebRequest.Get($"{baseURL}/questions");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = "{\"questions\":" + request.downloadHandler.text + "}";
            QuestionList list = JsonUtility.FromJson<QuestionList>(json);
            callback(list.questions);
        }
        else
        {
            Debug.LogError("Failed to fetch questions: " + request.error);
        }
    }

    

    [System.Serializable]
    public class AnswerData
    {
        public int student_id;
        public int question_id;
        public string selected_option;
        public bool is_correct;
    }

    [System.Serializable]
    public class QuestionList
    {
        public List<Question> questions;
    }
    public IEnumerator SubmitFinalScore(int score)
    {
        // Create the data to send
        ScoreSubmission data = new ScoreSubmission
        {
            student_id = student_id,  
            score = score
        };

        string json = JsonUtility.ToJson(data);

        // Setup the POST request
        UnityWebRequest req = new UnityWebRequest($"{baseURL}/submit-score", "POST");
        byte[] body = System.Text.Encoding.UTF8.GetBytes(json);
        req.uploadHandler = new UploadHandlerRaw(body);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        // Send the request and wait for response
        yield return req.SendWebRequest();

        // Handle the result
        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(" Score submission failed: " + req.error);
        }
        else
        {
            string result = req.downloadHandler.text;
            Debug.Log(" Score submission successful! Response: " + result);

            // Optionally, parse response if needed
            // var response = JsonUtility.FromJson<ScoreResponse>(result);
            // Debug.Log("Status: " + response.status);
        }
    }


    [System.Serializable]
    public class ScoreSubmission
    {
        public int student_id;
        public int score;
    }


}

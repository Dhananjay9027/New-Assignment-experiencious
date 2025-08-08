using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.Collections;
using System.Security.Cryptography;

public class RegistrationManager : MonoBehaviour
{
    public TMP_InputField inputName;
    public TMP_InputField inputClass;
    public TMP_InputField inputMobile;
    public TMP_InputField inputEmail;
    public TMP_Text statusText;
    public Button Register;
    private string apiUrl = "http://localhost:8000/login-or-register";
   

    private void Start()
    {
        Register.onClick.AddListener(OnRegisterClick);
    }

    [System.Serializable]
    public class RegistrationData
    {
        public string name;
        public string class_name;
        public string mobile;
        public string email;
    }

    [System.Serializable]
    public class RegistrationResponse
    {
        public string status;
        public int student_id;
        public string message; 
    }

    public void OnRegisterClick()
    {
        if (ValidateForm())
        {
            RegistrationData data = new RegistrationData
            {
                name = inputName.text,
                class_name = inputClass.text,
                mobile = inputMobile.text,
                email = inputEmail.text
            };

            StartCoroutine(SendRegistration(data));
        }
    }

    bool ValidateForm()
    {
        if (string.IsNullOrEmpty(inputName.text) ||
            string.IsNullOrEmpty(inputClass.text) ||
            string.IsNullOrEmpty(inputMobile.text) ||
            string.IsNullOrEmpty(inputEmail.text))
        {
            statusText.text = " Please fill in all fields.";
            return false;
        }

        /
        return true;
    }

    IEnumerator SendRegistration(RegistrationData data)
    {
        string json = JsonUtility.ToJson(data);
        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        byte[] body = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(body);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            statusText.text = " Registration failed: " + request.error;
        }
        else
        {
            string result = request.downloadHandler.text;
            RegistrationResponse response = JsonUtility.FromJson<RegistrationResponse>(result);
            statusText.text = response.message == "login"

    ? $" Welcome back! Student ID: {response.student_id}"
    : $" Registered! Student ID: {response.student_id}";
            APIManager.Instance.student_name=data.name;
            APIManager.Instance.student_id = response.student_id;
            yield return new WaitForSeconds(1);
            UnityEngine.SceneManagement.SceneManager.LoadScene("QuizScene");
        }
    }

}



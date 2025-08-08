[System.Serializable]
public class Question
{
    public int id;
    public string category;
    public string question;
    public string option_a;
    public string option_b;
    public string option_c;
    public string option_d;
    public string correct_option; // not sent to player, but useful for testing
}

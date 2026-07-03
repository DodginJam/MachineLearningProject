using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreUI : MonoBehaviour
{
    [field: SerializeField]
    public TextMeshProUGUI PlayerScoreDisplay
    {  get; private set; }

    [field: SerializeField]
    public TextMeshProUGUI AgentScoreDisplay
    { get; private set; }

    [field: SerializeField]
    public TrackingAgentGameManager GameManager
    { get; private set; }

    [field: SerializeField]
    public TextMeshProUGUI WinnerDisplay
    { get; private set; }

    [field: SerializeField]
    public TextMeshProUGUI TimerDisplay
    { get; private set; }

    public int TimerDisplayValue
    { get; private set; }

    private void OnEnable()
    {
        GameManager.UpdatePlayerUI += UpdatePlayerDisplay;
        GameManager.UpdateAgentUI += UpdateAgentDisplay;
        GameManager.DisplayWinnerUI += DisplayWinnerUI;
        GameManager.UpdateTimer += UpdateTimer;
    }

    private void OnDisable()
    {
        GameManager.UpdatePlayerUI -= UpdatePlayerDisplay;
        GameManager.UpdateAgentUI -= UpdateAgentDisplay;
        GameManager.DisplayWinnerUI -= DisplayWinnerUI;
    }

    public void UpdatePlayerDisplay(int newDisplay)
    {
        PlayerScoreDisplay.text = newDisplay.ToString();
    }

    public void UpdateAgentDisplay(int newDisplay)
    {
        AgentScoreDisplay.text = newDisplay.ToString();
    }

    public void DisplayWinnerUI(int winnerID)
    {
        string winnerString = winnerID == 0 ? "Player Wins" : winnerID == 1 ? "Agent Wins" : "Draw - No Winner";
        WinnerDisplay.text = winnerString;

        Invoke(nameof(ReturnToMenu), 5f);
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void UpdateTimer(float newTime)
    {
        int roundedTime = Mathf.FloorToInt(newTime);

        if (roundedTime != TimerDisplayValue)
        {
            TimerDisplayValue = roundedTime;
            TimerDisplay.text = roundedTime.ToString();
        }
    }
}

using ProjectEnums;
using System;
using UnityEngine;

public class TrackingAgentGameManager : MonoBehaviour
{
    [field: SerializeField]
    public float TimerToEndOfGame
    { get; private set; } = 30;

    public int PlayerScore
    { get; private set; } = 0;

    public int AgentScore
    { get; private set; } = 0;


    [field: SerializeField]
    public CameraController Player
    { get; private set; }

    [field: SerializeField]
    public TrackAndFireAgent Agent
    { get; private set; }

    [field: SerializeField]
    public ScoreUI UIDisplay
    { get; private set; }

    public event Action<int> UpdatePlayerUI;
    public event Action<int> UpdateAgentUI;

    public event Action<int> DisplayWinnerUI;

    public event Action<float> UpdateTimer;

    private void OnEnable()
    {
        Player.UpdateScore += UpdatePlayerScore;
        Agent.UpdateScore += UpdateAgentScore;
    }

    private void OnDisable()
    {
        Player.UpdateScore -= UpdatePlayerScore;
        Agent.UpdateScore -= UpdateAgentScore;
    }

    void EndGame()
    {
        Agent.gameObject.SetActive(false);
        Player.PlayerControllerOwner.gameObject.SetActive(false);

        // Player win is 0, agent win is 1 and draw is 2.
        int winnerID = PlayerScore > AgentScore ? 0 : AgentScore > PlayerScore ? 1 : 2;
        DisplayWinnerUI?.Invoke(winnerID);
    }

    private void Update()
    {
        TimerToEndOfGame -= Time.deltaTime;

        if (TimerToEndOfGame <= 0)
        {
            EndGame();
        }
        else
        {
            UpdateTimer?.Invoke(TimerToEndOfGame);
        }
    }

    public void UpdatePlayerScore(TargetType targetType)
    {
        int scoreToAdd = targetType == TargetType.Enemy ? 1 : -1;
        PlayerScore += scoreToAdd;
        UpdatePlayerUI?.Invoke(PlayerScore);
    }

    public void UpdateAgentScore(TargetType targetType)
    {
        int scoreToAdd = targetType == TargetType.Enemy ? 1 : -1;
        AgentScore += scoreToAdd;
        UpdateAgentUI?.Invoke(AgentScore);
    }
}

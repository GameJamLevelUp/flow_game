using UnityEngine;
using Dan.Main;
using TMPro;
using System;

public class PopulateLeaderboard : MonoBehaviour
{
    public GameObject entryPrefab;
    public Transform entryContainer;
    public TMP_InputField nameInputField; // Reference to the TMP_InputField for the player's name
    public float playerScore; // Score to be submitted

    void Start()
    {
        FetchAndPopulateLeaderboard();
    }

    private void FetchAndPopulateLeaderboard()
    {
        Leaderboards.flow_game_leaderboard.GetEntries((Dan.Models.Entry[] scores) =>
        {
            foreach (Transform child in entryContainer)
            {
                Destroy(child.gameObject);
            }

            for (int i = 0; i < scores.Length; i++)
            {
                GameObject newEntry = Instantiate(entryPrefab, entryContainer);
                LeaderboardEntry entry = newEntry.GetComponent<LeaderboardEntry>();
                entry.SetEntryData(i + 1, scores[i].Username, scores[i].Score);
            }
        });
    }

    public TextMeshProUGUI highscoreText;
     public void SaveHighScore(float playerScore)
    {
        string playerName;
        if (nameInputField != null)
        {
            playerName = nameInputField.text;
            Debug.Log(playerName);
        }
        else
        {
            Debug.LogError("NameInputField is not assigned.");
            return;
        }

        // Ensure playerName is not empty before uploading
        if (!string.IsNullOrEmpty(playerName))
        {
            int scoreInMeters = ConvertScoreToMeters(highscoreText.text);
            int newScore = (int)playerScore;
            int updatedScore = scoreInMeters + newScore;

            Leaderboards.flow_game_leaderboard.UploadNewEntry(playerName, updatedScore, (bool success) =>
            {
                if (success)
                {
                    Debug.Log($"High score {updatedScore} uploaded successfully.");
                    // Optionally, you can call FetchAndPopulateLeaderboard() again to refresh the leaderboard
                    FetchAndPopulateLeaderboard();
                }
                else
                {
                    Debug.LogError("Failed to upload high score.");
                }
            });
        }
        else
        {
            Debug.LogError("Player name is empty. Cannot upload high score.");
        }
    }

    // Convert highscoreText from kilometers to meters and parse it
    private int ConvertScoreToMeters(string scoreText)
    {
        // Remove the " km" part and parse the value
        scoreText = scoreText.Replace(" km", "").Trim();
        if (float.TryParse(scoreText, out float kilometers))
        {
            return Mathf.RoundToInt(kilometers * 1000f); // Convert kilometers to meters
        }
        Debug.LogError("Failed to parse high score text.");
        return 0; // Return 0 if parsing fails
    }
}

using System;
using UnityEngine;
using System.IO;
using TMPro;

public static class HighScoreManager
{
    private static readonly string filePath = "highscore.txt";

    // Save the high score to a text file
    public static void SaveHighScore(float score)
    {
        try
        {
            File.WriteAllText(filePath, score.ToString());
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save high score: {e.Message}");
        }
    }

    // Retrieve the high score from the text file
    public static float GetHighScore()
    {
        try
        {
            if (File.Exists(filePath))
            {
                string scoreText = File.ReadAllText(filePath);
                if (float.TryParse(scoreText, out float highScore))
                {
                    return highScore;
                }
                else
                {
                    Debug.LogError("Failed to parse high score from file.");
                    return 0;
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to retrieve high score: {e.Message}");
        }
        return 0; // Default to 0 if no high score is found
    }

    // Check if the current score is higher than the saved high score and save it if true
    public static bool TrySaveHighScore(float score, TextMeshProUGUI highScoreText)
    {
        float currentHighScore = 0;
        try 
        {
            currentHighScore = GetHighScore();
        }
        catch (Exception e)
        {
            return false;
        }

        float dividedDistance = Mathf.Max(score, currentHighScore) / 1000f;

        string scoreText = $"{dividedDistance:F2} km";
        
        highScoreText.text = $"HIGH SCORE: {scoreText}";
        if (score > currentHighScore)
        {
            highScoreText.text = "NEW HIGH SCORE";
            SaveHighScore(score);
            return true;
        }

        return false;
    }
}

using System;
using System.IO;
using TMPro;
using UnityEngine;

public class UsernameManager : MonoBehaviour
{
    private static readonly string usernameFilePath = "username.txt";

    // Reference to the TMP_InputField for username input
    public TMP_InputField usernameInputField;

    // Method to save the username from the TMP_InputField
    public void SaveUsernameFromInput()
    {
        if (usernameInputField != null)
        {
            string username = usernameInputField.text;
            SaveUsername(username);

            transform.GetComponent<DisplayLeaderboardState>().UpdateState();
        }
        else
        {
            Debug.LogError("UsernameInputField is not assigned.");
        }
    }

    // Save the username to a text file
    public void SaveUsername(string username)
    {
        try
        {
            File.WriteAllText(usernameFilePath, username);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save username: {e.Message}");
        }
    }

    // Retrieve the username from the text file
    public string GetUsername()
    {
        try
        {
            if (File.Exists(usernameFilePath))
            {
                return File.ReadAllText(usernameFilePath);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to retrieve username: {e.Message}");
        }
        return string.Empty; // Return an empty string if no username is found
    }

    // Optionally, you can set the TMP_InputField text from the saved username
    public void LoadUsernameIntoInputField()
    {
        if (usernameInputField != null)
        {
            string username = GetUsername();
            usernameInputField.text = username;
        }
        else
        {
            Debug.LogError("UsernameInputField is not assigned.");
        }
    }
}

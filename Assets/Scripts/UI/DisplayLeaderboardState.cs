using TMPro;
using UnityEngine;

public class DisplayLeaderboardState : MonoBehaviour
{
    public GameObject content;       // Reference to the content GameObject
    public GameObject nameInput; // Reference to the TMP_InputField for entering a name

    private UsernameManager usernameManager; // Reference to the UsernameManager component

    void Start()
    {
        UpdateState();
    }

    public void UpdateState() {
        usernameManager = FindObjectOfType<UsernameManager>();

        if (usernameManager == null)
        {
            Debug.LogError("UsernameManager not found in the scene.");
            return;
        }

        // Check if a username is set
        string username = usernameManager.GetUsername();
        
        if (string.IsNullOrEmpty(username))
        {
            // No username found, show the nameInput and hide content
            nameInput.gameObject.SetActive(true);

            content.SetActive(false);
        }
        else
        {
            // Username found, show the content and hide nameInput
            nameInput.gameObject.SetActive(false);
            nameInput.GetComponent<TMP_InputField>().text = username;
            content.SetActive(true);
        }
    }
}

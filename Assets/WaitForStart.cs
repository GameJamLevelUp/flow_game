using UnityEngine;
using System.Collections;

public class WaitForStart : MonoBehaviour
{
    public GameObject gameplayManager;
    public GameObject startPanel;

    void Start()
    {
        // Start the coroutine to wait for the right mouse click
        StartCoroutine(WaitForRightClick());
    }

    IEnumerator WaitForRightClick()
    {
        // Pause the game or disable gameplay mechanics
        if (gameplayManager != null)
        {
            gameplayManager.SetActive(false);
        }

        if (startPanel != null)
        {
            startPanel.SetActive(true);
        }


        // Wait until the player left-clicks
        while (!Input.GetMouseButtonDown(0))
        {
            yield return null; // Wait for the next frame
        }

        if (startPanel != null)
        {
            startPanel.SetActive(false);
        }

        // Enable gameplay mechanics
        if (gameplayManager != null)
        {
            gameplayManager.SetActive(true);
        }

    }
}
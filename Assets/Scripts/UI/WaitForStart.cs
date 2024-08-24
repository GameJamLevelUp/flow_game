using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WaitForStart : MonoBehaviour
{
    public GameObject[] gameplayManagers;
    public GameObject startPanel;

    void Start()
    {
        // Start the coroutine to wait for the right mouse click
        StartCoroutine(WaitForStartClick());
    }

    IEnumerator WaitForStartClick()
    {
        // Pause the game or disable gameplay mechanics
        if (gameplayManagers != null)
        {
            foreach (GameObject manager in gameplayManagers)
            {
                manager.SetActive(false);
            }
        }

        if (startPanel != null)
        {
            startPanel.SetActive(true);
        }

        while (startPanel.activeSelf)
        {
            yield return null; // Wait for the next frame
        }




        // Enable gameplay mechanics
        if (gameplayManagers != null)
        {
            foreach (GameObject manager in gameplayManagers)
            {
                manager.SetActive(true);
            }
        }
    }

}
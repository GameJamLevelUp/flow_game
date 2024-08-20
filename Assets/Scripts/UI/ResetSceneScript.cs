using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetSceneScript : MonoBehaviour
{
    // Call this method to reset the current scene
    public void ResetScene()
    {
        // Get the active scene and reload it
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}

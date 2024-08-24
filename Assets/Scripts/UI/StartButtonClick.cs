using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StartButtonClick : MonoBehaviour
{
    public GameObject startPanel;

    public void HidePanel()
    {
        if (startPanel != null)
        {
            startPanel.SetActive(false);
        }
    }


}
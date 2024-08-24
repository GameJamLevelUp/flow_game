using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TimeController : MonoBehaviour
{

    public float targetTimeScaleUI = 1.0f; // Target time scale
    public float targetTimeScaleEffects = 1.0f; // Target time scale
    private float timeScaleSpeed = 5.0f;  // Speed of transition
    public bool hasDied = false;
    void Update()
    {
        if (!hasDied) 
        {
            float target = Mathf.Min(targetTimeScaleUI, targetTimeScaleEffects);
            Time.timeScale = Mathf.Lerp(Time.timeScale, target, Time.deltaTime * timeScaleSpeed);
        }
    }
}

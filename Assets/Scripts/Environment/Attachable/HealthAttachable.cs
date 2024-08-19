using UnityEngine;

public class HealthAttachable : Attachable
{

    // Override the OnComplete method from Attachable
    public override void OnComplete()
    {
        Debug.Log("ADD healthj");
        GameObject.FindAnyObjectByType<GameUI>().ReceiveHealth();
    }
}

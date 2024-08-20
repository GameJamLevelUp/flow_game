using UnityEngine;

public class HealthAttachable : Attachable
{

    // Override the OnComplete method from Attachable
    public override void OnComplete()
    {
        GameObject.FindAnyObjectByType<GameUI>().ReceiveHealth();
    }
}

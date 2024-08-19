using UnityEngine;
using Dan.Main; // This should be accessible if the namespace is correct

public class TestNamespace : MonoBehaviour
{
    void Start()
    {
        Debug.Log(Leaderboards.flow_game_leaderboard);
    }
}

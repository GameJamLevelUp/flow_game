using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardEntry : MonoBehaviour
{
    public TextMeshProUGUI indexText;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI distanceText;

    // Method to set the leaderboard entry data
    public void SetEntryData(int index, string name, float distanceInMeters)
    {
        // Set the index with a ". " at the end
        indexText.text = index.ToString() + ".";

        // Set the name
        nameText.text = name;

        // Convert the distance from meters to kilometers and format it
        float distanceInKm = distanceInMeters / 1000f;
        distanceText.text = distanceInKm.ToString("0.00") + " km";
    }
}

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ShowStats : MonoBehaviour {

    public GameObject[] PlayerText = null;

    public string [] positions = null;

	// Use this for initialization
	void Start () {
        var pp = God.main.GetComponent<RaceGod>().playerPositions;
        var ps = God.main.GetComponent<RaceGod>().stats;
        var wp = God.main.GetComponent<RaceGod>().winningPlayer;

        PlayerText[wp].GetComponent<Text>().color = new Color(0.0f, 1f, 0.0f);

        for (var i = 0; i< 4; i++) {
            var pt = PlayerText [i].GetComponent<Text>();
            pt.text += "\n";
            // Is this what square they are on, or the position that they are in the race?
            //pt.text += positions[pp[i]] + "\n";
            pt.text += "\n";
            pt.text += "Boosts: " + ps [i].boostsUsed + "\n";
            pt.text += "kcal: " + Mathf.Floor(ps [i].caloriesBurned*1000) + "\n";
            pt.text += "Snorted: " + ps [i].cokeConsumed + "\n";
            pt.text += "Collected: " + ps [i].cokeFromHeaven + "\n";
            pt.text += "Regen'd: " + ps [i].cokeRegenerated + "\n";
            pt.text += "Damage: " + ps [i].damageCaused + "\n";
            // For some reason, this value can be: 1.255e+09
            // If you fix that bug, please uncomment this line
            //pt.text += "Distance: " + (ps [i].distanceTraveled < 1 ? 0 : Mathf.Round(ps [i].distanceTraveled)) + "\n";
            pt.text += "Garbage: " + ps [i].garbageStrewn + "\n";
            pt.text += "Messages: " + ps [i].messagesReceived + "\n";
            pt.text += "Collisions: " + ps [i].thingsCollidedWith + "\n";
            pt.text += "Switches: " + ps [i].timesLanesSwitched + "\n";
            pt.text += "Running: " + God.FormatTime(ps [i].timeSpentRunning) + "\n";
            pt.text += "Standing: " + God.FormatTime(ps [i].timesStoodStill) + "\n";

        }

    }
	
	// Update is called once per frame
	void Update () {
	
	}
}

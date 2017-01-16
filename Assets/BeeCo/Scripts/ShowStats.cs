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

        foreach( int i in pp ) {
            var pt = PlayerText [i].GetComponent<Text>();
            pt.text += "\n";
            // Is this what square they are on, or the position that they are in the race?
            //pt.text += positions[pp[i]] + "\n";
            pt.text += "\n";

            pt.text += "Boosts Used\n";
            pt.text += God.FormatNumber( ps[i].boostsUsed ) + "\n";

            pt.text += "kcals Burned\n";
            pt.text += God.FormatNumber( Mathf.Floor(ps [i].caloriesBurned*1000) ) + "\n";

            pt.text += "Coke Snorted\n";
            pt.text += God.FormatNumber( ps[i].cokeConsumed ) + "\n";

            pt.text += "Coke From Sky\n";
            pt.text += God.FormatNumber( ps[i].cokeFromHeaven ) + "\n";

            pt.text += "Coke Regen'd\n";
            pt.text += God.FormatNumber( ps[i].cokeRegenerated ) + "\n";

            pt.text += "Damage Caused\n";
            pt.text += God.FormatMoney( ps[i].damageCaused ) + "\n";

            pt.text += "Dist Traveled\n";
            pt.text += God.FormatNumber( ps[i].distanceTraveled ) + "\n";

            pt.text += "Garbage Strewn\n";
            pt.text += God.FormatNumber( ps[i].garbageStrewn ) + "\n";

            pt.text += "Chat Messages\n";
            pt.text += God.FormatNumber( ps[i].messagesReceived ) + "\n";

            pt.text += "Collisions\n";
            pt.text += God.FormatNumber( ps[i].thingsCollidedWith ) + "\n";

            pt.text += "Lane Switch Academy\n";
            pt.text += God.FormatNumber( ps[i].timesLanesSwitched ) + "\n";

            pt.text += "Run Time\n";
            pt.text += God.FormatTime( ps[i].timeSpentRunning ) + "\n";

            pt.text += "Times Stood Still\n";
            pt.text += God.FormatNumber( ps[i].timesStoodStill ) + "\n";
        }
    }
}

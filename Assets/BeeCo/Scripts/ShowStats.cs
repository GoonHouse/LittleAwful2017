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

        for( int i = 0; i < 4; i++ ) {
            var pt = PlayerText [i].GetComponent<Text>();

            // find the correct stats for this player
            var pss = new BirdStats();
            var ppp = -1;
            var k = 0;
            foreach( int j in pp ) {
                if( j == i ) {
                    pss = ps[k];
                    ppp = pp.Count - k;
                    break;
                }
                k++;
            }

            pt.text += "\n\nPosition\n";
            // Is this what square they are on, or the position that they are in the race?
            pt.text += ppp + "\n";

            pt.text += "Boosts Used\n";
            pt.text += God.FormatNumber( pss.boostsUsed ) + "\n";

            pt.text += "kcals Burned\n";
            pt.text += God.FormatNumber( Mathf.Floor( pss.caloriesBurned*1000) ) + "\n";

            pt.text += "Coke Snorted\n";
            pt.text += God.FormatNumber( pss.cokeConsumed ) + "\n";

            pt.text += "Coke From Sky\n";
            pt.text += God.FormatNumber( pss.cokeFromHeaven ) + "\n";

            pt.text += "Coke Regen'd\n";
            pt.text += God.FormatNumber( pss.cokeRegenerated ) + "\n";

            pt.text += "Damage Caused\n";
            pt.text += God.FormatMoney( pss.damageCaused ) + "\n";

            pt.text += "Dist Traveled\n";
            pt.text += God.FormatNumber( pss.distanceTraveled ) + "\n";

            pt.text += "Garbage Strewn\n";
            pt.text += God.FormatNumber( pss.garbageStrewn ) + "\n";

            pt.text += "Chat Messages\n";
            pt.text += God.FormatNumber( pss.messagesReceived ) + "\n";

            pt.text += "Collisions\n";
            pt.text += God.FormatNumber( pss.thingsCollidedWith ) + "\n";

            pt.text += "Lane Switch Academy\n";
            pt.text += God.FormatNumber( pss.timesLanesSwitched ) + "\n";

            pt.text += "Run Time\n";
            pt.text += God.FormatTime( pss.timeSpentRunning ) + "\n";

            pt.text += "Times Stood Still\n";
            pt.text += God.FormatNumber( pss.timesStoodStill ) + "\n";
        }
    }
}

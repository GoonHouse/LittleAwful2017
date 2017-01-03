using UnityEngine;
using System.Collections;

public class RaceBird : MonoBehaviour {
    public RaceBrain brain;

    public int lane;
    public int position;

    public Vector2 moveDist = new Vector2(14.5f, 13.7f);

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
        if ( brain.BrainLeft() ) {
            CanMove(lane - 1, position);
        } else if ( brain.BrainRight() ) {
            CanMove(lane + 1, position);
        }


        if ( brain.BrainGo() ) {
            CanMove(lane, position + 1);
        } else if ( brain.BrainStop() ) {
            CanMove(lane, position - 1);
        }
    }

    // burp
    bool CanMove(int moveLane, int movePosition) {
        var t = GetComponent<Transform>();
        var pos = t.position;
        var mvec = new Vector2(lane - moveLane,  movePosition - position);
        lane = moveLane;
        position = movePosition;
        pos.x += mvec.y * moveDist.x;
        pos.z += mvec.x * moveDist.y;
        t.position = pos;
        return true;
    }
}

using UnityEngine;
using System.Collections;

public class RaceBird : MonoBehaviour {
    public RaceBrain brain;

    public int lane;
    public int position;

    public Vector2 moveDist = new Vector2(14.5f, 13.7f);

    public Vector2 minExtents = new Vector2(1, 0);
    public Vector2 maxExtents = new Vector2(4, 7);

    // Use this for initialization
    void Start () {
        SetPosition(lane, position);

    }
	
	// Update is called once per frame
	void Update () {
        if ( brain.BrainLeft() ) {
            MoveIfPossible(lane - 1, position);
        } else if ( brain.BrainRight() ) {
            MoveIfPossible(lane + 1, position);
        }


        if ( brain.BrainGo() ) {
            MoveIfPossible(lane, position + 1);
        } else if ( brain.BrainStop() ) {
            MoveIfPossible(lane, position - 1);
        }
    }

    void SetPosition(int newLane, int newPosition) {
        var rg = God.main.GetComponent<RaceGod>();
        rg.grid.Add(newLane.ToString() + "_" + newPosition.ToString(), this.gameObject);
        this.lane = newLane;
        this.position = newPosition;
    }

    void ClearPosition(int newLane, int newPosition) {
        var rg = God.main.GetComponent<RaceGod>();
        rg.grid.Remove(newLane.ToString() + "_" + newPosition.ToString());
    }

    bool CanMove(int moveLane, int movePosition) {
        var rg = God.main.GetComponent<RaceGod>();
        bool canMove = true;
        
        // is it occupied?
        GameObject doesExist;
        rg.grid.TryGetValue(moveLane.ToString() + "_" + movePosition.ToString(), out doesExist);
        canMove = (canMove && (doesExist == null));

        // is it within boundaries?
        canMove = (canMove && ( moveLane >= minExtents.x && moveLane <= maxExtents.x ));
        canMove = (canMove && ( movePosition >= minExtents.y && movePosition <= maxExtents.y));

        return canMove;
    }

    bool MoveIfPossible(int moveLane, int movePosition) {
        if( CanMove(moveLane, movePosition)) {
            var t = GetComponent<Transform>();
            var pos = t.position;
            var mvec = new Vector2(lane - moveLane, movePosition - position);
            ClearPosition(lane, position);
            lane = moveLane;
            position = movePosition;
            pos.x += mvec.y * moveDist.x;
            pos.z += mvec.x * moveDist.y;
            t.position = pos;
            SetPosition(lane, position);
            return true;
        } else {
            return false;
        }
    }
}

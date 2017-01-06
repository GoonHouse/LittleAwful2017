using UnityEngine;
using System.Collections;

public class RaceBird : MonoBehaviour {
    public RaceBrain brain;

    public Vector2 gridPosition = new Vector2(0, 0);

    public float laneSwitchTime = 0.5f;

    public bool alive = true;

    // Use this for initialization
    void Start () {
        SetPosition();
    }
	
	// Update is called once per frame
	void Update () {
        if ( brain.BrainLeft() ) {
            MoveIfPossible(0, 1);
        } else if ( brain.BrainRight() ) {
            MoveIfPossible(0, -1);
        }


        if ( brain.BrainGo() ) {
            MoveIfPossible(1, 0);
        } else if ( brain.BrainStop() ) {
            MoveIfPossible(-1, 0);
        }
    }

    void SetPosition() {
        var rg = God.main.GetComponent<RaceGod>();
        rg.grid.Add(gridPosition.x + "_" + gridPosition.y, this.gameObject);
    }

    void ClearPosition() {
        var rg = God.main.GetComponent<RaceGod>();
        rg.grid.Remove(gridPosition.x + "_" + gridPosition.y);
    }

    bool CanMove( Vector2 newPos ) {
        var rg = God.main.GetComponent<RaceGod>();
        bool canMove = true;
        
        
        // is it occupied?
        GameObject doesExist;
        rg.grid.TryGetValue(
            newPos.x + "_" + newPos.y,
            out doesExist
        );

        canMove = (canMove && (doesExist == null));

        // is it within boundaries?
        canMove = ( canMove && ( newPos.x >= 0.0f && newPos.x < rg.worldDimensions.x ) );
        canMove = ( canMove && ( newPos.y >= 0.0f && newPos.y < rg.worldDimensions.y ) );

        return canMove;
    }

    void ForceMove(int relX, int relY) {
        var newPos = new Vector2(
            gridPosition.x + relX,
            gridPosition.y + relY
        );

    }

    bool MoveIfPossible(int relX, int relY) {
        var newPos = new Vector2(
            gridPosition.x + relX,
            gridPosition.y + relY
        );
        if( CanMove( newPos ) ) {
            ClearPosition();

            // find where we need to tween to
            var rg = God.main.GetComponent<RaceGod>();
            GameObject targetLocation;
            var trygetmove = newPos.x + "_" + newPos.y;
            rg.spawnedGrid.TryGetValue(
                newPos.x + "_" + newPos.y,
                out targetLocation
            );

            if( !targetLocation ) {
                Debug.Log( this.name + " tried to move to: " + trygetmove );
            }

            // set our tween target to a position + an offset
            var loc = targetLocation.transform.position;
            loc.y += 10.0f;
            GetComponent<Tweener>().SetTarget( loc, Quaternion.identity, laneSwitchTime);

            // physically relocate ourselves
            gridPosition = newPos;
            SetPosition();
            return true;
        } else {
            return false;
        }
    }

    void OnTriggerEnter( Collider other ) {
        Debug.Log( gameObject.name + " just touched a " + other.gameObject.name );
    }
}

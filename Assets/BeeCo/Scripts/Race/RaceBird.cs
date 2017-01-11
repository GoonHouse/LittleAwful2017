using UnityEngine;
using System.Collections;

public class RaceBird : MonoBehaviour {
    private RaceGod rg;

    public RaceBrain brain;

    public Vector2 gridPosition = new Vector2(0, 0);

    public float laneSwitchTime = 0.5f;

    // whether or not the bird has been busted
    public bool alive = true;

    // the maximum amount of coke the bird has obtained
    public int cokeMax = 5;
    // how much coke the bird has to spend
    public int cokeCurrent = 5;

    // the last point in time coke was used
    public float timeLastUsedCoke;
    // how much time must pass before coke is restored
    public float timeToRecoverCoke = 5.0f;
    // once we are allowed to generate coke, how much time per unit to restore?
    public float timePerCokeRecovery = 1.0f;

    // when we last generated a coke
    public float timeLastGeneratedCoke;

    // Use this for initialization
    void Start () {
        rg = God.main.GetComponent<RaceGod>();
        //SetPosition();
    }
	
	// Update is called once per frame
	void Update () {
        switch( rg.raceState ) {
            case RaceState.PreHungry:
                // ???
                break;
            case RaceState.Hungry:
                // check if we have run out of time
                // check if all marbles consumed
                break;
            case RaceState.PostHungry:
                break;
            case RaceState.PreRace:
                break;
            case RaceState.Race:
                InterpretBrainForRace();
                CokeUpdate();
                break;
            case RaceState.PostRace:
                // winner emerges, tween them into the sunset
                break;
            default:
                break;
        }
    }

    void InterpretBrainForRace() {
        if( brain.BrainLeft() ) {
            MoveIfPossible( 0, 1 );
        } else if( brain.BrainRight() ) {
            MoveIfPossible( 0, -1 );
        }


        if( brain.BrainGo() ) {
            CokeBoost();
        } else if( brain.BrainStop() ) {
            MoveIfPossible( -1, 0 );
        }
    }

    void CokeUpdate() {
        var missingCoke = cokeMax - cokeCurrent;
        if( Time.time > ( timeLastUsedCoke + timeToRecoverCoke ) ) {
            if( Time.time > ( timeLastGeneratedCoke + timePerCokeRecovery ) && missingCoke > 0 ) {
                timeLastGeneratedCoke = Time.time;
                OnGetCoke();
            }
        }
    }

    void OnGetCoke() {
        if( cokeCurrent < cokeMax ) {
            cokeCurrent += 1;
        }
    }

    bool CokeBoost() {
        if( cokeCurrent > 0 && MoveIfPossible( 1, 0 ) ) {
            cokeCurrent -= 1;
            timeLastUsedCoke = Time.time;
            return true;
        } else {
            return false;
        }
    }

    void SetPosition() {
        rg.grid.Add(gridPosition.x + "_" + gridPosition.y, this.gameObject);
    }

    void ClearPosition() {
        rg.grid.Remove(gridPosition.x + "_" + gridPosition.y);
    }

    bool CanMove( Vector2 newPos ) {
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

    public void ForceMoveTo( int absX, int absY, float time = -1.0f ) {
        var newPos = new Vector2(
            absX,
            absY
        );
        MoveTo( newPos, time );
    }

    public void ForceMove(int relX, int relY) {
        var newPos = new Vector2(
            gridPosition.x + relX,
            gridPosition.y + relY
        );
        MoveTo( newPos );
    }

    bool MoveIfPossible(int relX, int relY) {
        var newPos = new Vector2(
            gridPosition.x + relX,
            gridPosition.y + relY
        );
        if( CanMove( newPos ) ) {
            MoveTo( newPos );
            return true;
        } else {
            return false;
        }
    }

    void MoveTo( Vector2 newPos, float time = -1.0f ) {
        if( time < 0.0f ) {
            time = laneSwitchTime;
        }
        ClearPosition();

        GameObject targetLocation;
        var trygetmove = newPos.x + "_" + newPos.y;
        rg.spawnedGrid.TryGetValue(
            newPos.x + "_" + newPos.y,
            out targetLocation
        );

        if( !targetLocation ) {
            Debug.Log( this.name + " tried to move to: " + trygetmove );
        }

        targetLocation = targetLocation.transform.Find( "TweenAnchor" ).gameObject;

        // set our tween target to a position + an offset
        //var loc = targetLocation.transform.position;
        //loc.y += 10.0f;
        GetComponent<Tweener>().SetTarget( targetLocation, time );

        // physically relocate ourselves
        gridPosition = newPos;
        SetPosition();
    }

    void OnTriggerEnter( Collider other ) {
        if( other.gameObject.tag == "ForceBack" ) {
            if( ! MoveIfPossible( -1, 0 ) ) {
                Debug.Log( gameObject.name + " could not move backwards from hitting a " + other.gameObject.name );
            }
        }
    }
}

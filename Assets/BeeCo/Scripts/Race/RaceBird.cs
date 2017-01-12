using UnityEngine;
using System.Collections;

public class RaceBird : MonoBehaviour {
    private RaceGod rg;
    public PlayerGUI myGUI;
    public RaceBrain brain;

    public GameObject objectSpeech;
    public GameObject objectBusted;

    public Vector2 gridPosition = new Vector2(0, 0);

    public float laneSwitchTime = 0.5f;

    public int playerID;

    // whether or not the bird has been busted
    public bool alive = true;

    // the maximum amount of coke the bird has obtained
    public int cokeMax = 5;
    // how much coke the bird has to spend
    public int cokeCurrent = 5;

    public float timeToSwoonce = 0.5f;

    // the last point in time coke was used
    public float timeLastUsedCoke;
    // how much time must pass before coke is restored
    public float timeToRecoverCoke = 5.0f;
    // once we are allowed to generate coke, how much time per unit to restore?
    public float timePerCokeRecovery = 1.0f;

    // when we last generated a coke
    public float timeLastGeneratedCoke;

    private GameObject head;
    private GameObject headStartPosition;
    private Animator anims;
    private GameObject anchorBusted;
    private GameObject anchorSpeech;

    public bool isTargeting = false;
    public Marble targetMarble;

    // Use this for initialization
    void Start () {
        rg = God.main.GetComponent<RaceGod>();

        // find our head
        head = gameObject.transform.FindAllChildren( "Head" ).gameObject;
        head.AddComponent<Tweener>();
        anims = gameObject.GetComponentInChildren<Animator>();

        anchorBusted = gameObject.transform.FindAllChildren( "BustedAnchor" ).gameObject;
        anchorSpeech = gameObject.transform.FindAllChildren( "SpeechAnchor" ).gameObject;

        // create an anchor for our head
        var headMate = head.transform.parent.gameObject;
        headStartPosition = new GameObject();
        headStartPosition.transform.SetParent( headMate.transform );
        headStartPosition.transform.localPosition = head.transform.localPosition;
        headStartPosition.transform.localRotation = head.transform.localRotation;

        cokeCurrent = cokeMax;
        myGUI.score = cokeMax;
        myGUI.score_max = cokeMax;
        myGUI.current_score = cokeMax;
    }

    public void SetBrainFromInt( int brainType ) {
        switch( brainType ) {
            case 2: // Human
                brain = gameObject.AddComponent<RaceBrainHuman>();
                break;
            case 1: // AI
                brain = gameObject.AddComponent<RaceBrainComputer>();
                break;
            case 0: // twitch
            default:
                brain = gameObject.AddComponent<RaceBrainTwitch>();
                break;

        }
    }
	
	// Update is called once per frame
	void Update () {
        if( Input.GetKey( KeyCode.G ) ) {
            var bus = God.SpawnChild( objectBusted, anchorBusted );
        }
        switch( rg.raceState ) {
            case RaceState.PreHungry:
                // ???
                break;
            case RaceState.Hungry:
                // check if we have run out of time
                // check if all marbles consumed
                InterpretBrainForHungry();
                TargetUpdate();
                break;
            case RaceState.PostHungry:
                RetractHead();
                anims.SetBool( "IsRunning", true );
                break;
            case RaceState.PreRace:
                RetractHead();
                anims.SetBool( "IsRunning", true );
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
        UpdateHUD();
    }

    void InterpretBrainForHungry() {
        if( brain.BrainGo() && !targetMarble && !isTargeting ) {
            var cokes = GameObject.FindGameObjectsWithTag( "Edible" );
            if( cokes.Length > 0 ) {
                var closest = GetClosest( cokes );
                isTargeting = true;
                targetMarble = closest.GetComponent<Marble>();
                var tw = head.GetComponent<Tweener>();
                tw.SetTarget( closest, timeToSwoonce );
                anims.SetBool( "MouthIsOpen", true );
                tw.onDone += RetractHead;
                // head.transform.position = closest.transform.position;
            }
        }
    }

    void RetractHead() {
        var tw = head.GetComponent<Tweener>();
        tw.onDone -= RetractHead;
        if( targetMarble && isTargeting ) {
            targetMarble.transform.SetParent( head.transform );
            targetMarble.eater = this;
            targetMarble.tag = "Untagged";
            anims.SetBool( "MouthIsOpen", false );
            tw.onDone += EatCoke;
        }

        isTargeting = false;

        tw.SetTarget( headStartPosition, timeToSwoonce );
    }

    void EatCoke() {
        cokeMax += (int) targetMarble.value;
        cokeCurrent = cokeMax;
        // Debug.Log( gameObject.name + " is eating a coke: " + targetMarble.gameObject );
        rg.marblesActive--;
        Destroy( targetMarble.gameObject );
        var tw = head.GetComponent<Tweener>();
        tw.onDone -= EatCoke;
    }

    public void TwitchMessage( string msg, string user ) {
        Debug.Log( gameObject.name + "has twitch message <" + user + ">: " + msg );
    }

    GameObject GetClosest( GameObject[] pool ) {
        GameObject tMin = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;
        foreach( GameObject t in pool ) {
            float dist = Vector3.Distance( t.transform.position, currentPos );
            if( dist < minDist ) {
                tMin = t;
                minDist = dist;
            }
        }
        return tMin;
    }

    Transform GetClosest( Transform[] pool ) {
        Transform tMin = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;
        foreach( Transform t in pool ) {
            float dist = Vector3.Distance( t.position, currentPos );
            if( dist < minDist ) {
                tMin = t;
                minDist = dist;
            }
        }
        return tMin;
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

    void TargetUpdate() {
        if( targetMarble && targetMarble.eater != null && isTargeting ) {
            targetMarble = null;
            isTargeting = false;
            RetractHead();
        } else if( !targetMarble && isTargeting ) {
            RetractHead();
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

    public void SetPosition() {
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

    void UpdateHUD() {
        myGUI.score_max = cokeMax;
        myGUI.score = cokeCurrent;
    }

    void OnTriggerEnter( Collider other ) {
        if( other.gameObject.tag == "ForceBack" ) {
            if( ! MoveIfPossible( -1, 0 ) ) {
                Debug.Log( gameObject.name + " could not move backwards from hitting a " + other.gameObject.name );
            }
        }
    }
}

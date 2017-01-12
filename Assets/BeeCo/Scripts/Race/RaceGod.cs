using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum RaceState {
    NoRace,
    PreHungry,
    Hungry,
    PostHungry,
    PreRace,
    Race,
    PostRace
}

public class RaceGod : MonoBehaviour {
    public RaceState raceState = RaceState.NoRace;

    public GameObject debugMarker;
    public GameObject gridObject;
    public GameObject chunkAnchor;
    public GameObject cokeObject;

    public Dictionary<string, GameObject> grid = new Dictionary<string, GameObject>();
    public Dictionary<string, GameObject> spawnedGrid = new Dictionary<string, GameObject>();
    public int playersAlive;
    public float startTime;

    public float spawnDistance = 3.0f;
    public float spawnProtectionDistance = 7.0f;

    // how long the strips of road are
    public float worldChunkWidth = 100.0f;
    public int worldChunkOffset = -3;
    public Vector2 worldUnitDims = new Vector2(12, 10);
    public Vector2 worldDimensions = new Vector2(7, 4);
    public Vector2 worldMargins = new Vector2(2, 1);
    public Vector2 worldOffset;
    public string  worldSpawnTarget = "World/TileSpawns";

    public Transform world;
    public Transform motion;

    public int spawnWaveCount = 0;
    public float hazardMoveSpeed = 2.0f;

    public List<GameObject> players = new List<GameObject>();
    public List<GameObject> twitchPlayers = new List<GameObject>();
    public List<GameObject> playerGUIs = new List<GameObject>();

    public GameObject policeLight;
    public Text hudTime;
    public GameObject cameraRaceAnchor;
    public GameObject cokeSpawnAnchor;

    public float timeForBirdsRelocate = 3.0f;

    public float timePreRaceStart;
    public float timePreRaceDuration = 3.0f;

    // hungry game logic
    public float timeHungryStart;
    public float timeHungryDuration = 60.0f;

    public int marblesTotal = 0;
    public int marblesMinActive = 13;
    public int marblesActive = 0;
    public int marblesRoundTotal = 60;

    public float timeLastSpawnedMarble;
    public float timeSpawnMarbleDelay = 0.5f;

    public List<string> ircCommands = new List<string>{ "eat", "left", "right", "go", "back" };

    /*
     * intensity = determined by: ( Time.time - startTime )
     * 
     * spawn manager needs to ramp up "pressure" and let it off by spawning.
     *      how hazardous a configuration is, is based on intensity + rng boundaries
     * after a certain amount of time, we need to make the chance of spawning things 100%
     */
    void Awake() {
        SceneManager.sceneLoaded += TheLevelWasLoaded;
    }

    void TheLevelWasLoaded(Scene scene, LoadSceneMode loadSceneMode) {
        if( scene.name == "Race" ) {
            // shortcut transforms
            world = GameObject.Find( "World" ).transform;
            motion = GameObject.Find( "Motion/Lanes" ).transform;

            FindInstanceThings();

            // determine world size
            // TODO: the road prefab does not consider lane width & isn't variable
            worldOffset = new Vector2(
                worldUnitDims.x / 2.0f,
                ( worldUnitDims.y / 2.0f ) - ( ( worldDimensions.y / 2.0f ) * ( worldUnitDims.y + worldMargins.y ) )
            );
            CreateWorld();

            //SpawnDebugMarker();

            // spawn road chunks all the way up until the reference object
            // UpdateWorldPosition();
            // UpdateMotion();
            UpdateSpawn();

            raceState = RaceState.PreHungry;
        }
    }

    void FindInstanceThings() {
        policeLight = world.transform.Find( "PoliceLight" ).gameObject;
        hudTime = GameObject.Find( "Canvas/TimeLabel/Time" ).GetComponent<Text>();
        cameraRaceAnchor = world.transform.Find( "CameraAnchors/CameraRaceAnchor" ).gameObject;
        cokeSpawnAnchor = world.transform.Find( "CokeSpawnAnchor" ).gameObject;

        var guis = Camera.main.gameObject.GetComponentsInChildren<PlayerGUI>();
        foreach( PlayerGUI gui in guis ) {
            playerGUIs.Add( gui.gameObject );
        }

        var theSave = God.main.GetComponent<SaveData>().loadedSave;
        // @TODO: Announce Twitch Rules
        // twitch.SendMsg( "benis" );

        var pid = 0;
        var raceBirds = world.transform.Find( "BirdAnchors" ).gameObject.GetComponentsInChildren<RaceBird>();
        foreach( RaceBird rb in raceBirds ) {
            rb.playerID = pid;
            rb.SetBrainFromInt( theSave.playerUse[pid] );
            if( theSave.playerUse[pid] == 0 ) {
                twitchPlayers.Add( rb.gameObject );
            }
            players.Add( rb.gameObject );
            pid++;
        }

        if( twitchPlayers.Count > 0 ) {
            var twitch = God.main.GetComponent<TwitchIRC>();
            twitch.oauth = theSave.ircPass;
            twitch.nickName = theSave.ircNick;
            twitch.channelName = theSave.ircChannel;
            twitch.server = theSave.ircServer;
            twitch.port = theSave.ircPort;

            twitch.messageRecievedEvent.AddListener( MessageHandler );

            twitch.StartIRC();
        }
    }

    void MessageHandler( string msg, string user ) {
        var parts = new List<string>( msg.Split( ' ' ) );
        var wasCommand = false;

        if( parts.Count >= 2 ) {
            foreach( string ircCommand in ircCommands ) {
                if( parts[0] == ircCommand ) {
                    int pdex = -1;
                    if( System.Int32.TryParse( parts[1], out pdex ) && pdex > 0 && pdex < players.Count ) {
                        var p = players[pdex - 1];
                        if( p ) {
                            wasCommand = true;
                            var brain = p.GetComponent<RaceBird>().brain;
                            brain.DoSignal( ircCommand );
                        }
                    }
                }
            }
        }

        if( !wasCommand ) {
            var pid = Random.Range( 0, twitchPlayers.Count - 1 );
            players[pid].GetComponent<RaceBird>().TwitchMessage( msg, user );
        }
    }

    // Use this for initialization
    void Start () {
        // TheLevelWasLoaded( SceneManager.GetActiveScene(), LoadSceneMode.Single );
    }

    // Update is called once per frame
    void Update () {
        switch( raceState ) {
            case RaceState.NoRace:
                // not doing anything important
                break;
            case RaceState.PreHungry:
                // everything that sets the scene up better goes here
                policeLight.SetActive( false );

                raceState = RaceState.Hungry;
                break;
            case RaceState.Hungry:
                // collect the hungry edge
                if( timeHungryStart <= 0.0f ) {
                    timeHungryStart = Time.time;
                }

                hudTime.text = God.FormatTime(( timeHungryStart + timeHungryDuration ) - Time.time);

                UpdateEnoughCoke();

                // check if we are still hungry by time
                if( Time.time > ( timeHungryStart + timeHungryDuration ) ) {
                    raceState = RaceState.PostHungry;
                }

                // check if all marbles consumed
                break;
            case RaceState.PostHungry:
                hudTime.text = "GET READY!";
                TransitionHungryToRace();
                break;
            case RaceState.PreRace:
                // we let update motion
                // UpdateMotion();
                if( timePreRaceStart <= 0.0f ) {
                    timePreRaceStart = Time.time;
                }

                hudTime.text = God.FormatTime( ( timePreRaceStart + timePreRaceDuration ) - Time.time );

                // check if we are still hungry by time
                if( Time.time > ( timePreRaceStart + timePreRaceDuration ) ) {
                    // disable HUD text
                    

                    raceState = RaceState.Race;
                }
                break;
            case RaceState.Race:
                // update the world position
                // update the track
                hudTime.text = "TO RUN!";

                UpdateWorldPosition();
                UpdateSpawn();
                break;
            case RaceState.PostRace:
                // winner emerges, tween them into the sunset
                break;
            default:
                Debug.Log( "groose is loose" );
                break;
        }
    }

    int MapPlayersToStartPosition(int id) {
        switch( id ) {
            case 0:
                return 0;
            case 1:
                return 2;
            case 2:
                return 3;
            case 3:
                return 5;
            default:
                return 1;
        }
    }

    void TransitionHungryToRace() {
        /*
        when hungry game ends and race begins, we must:
        1) reposition HUD elements
        2) despawn all stray marbles
        3) tween the birds into their appropriate race positions
        */

        // it's the cops
        policeLight.SetActive( true );

        // tween the birds into their appropriate race positions
        var playerID = 0;
        foreach( GameObject player in players ) {
            /*
            var tweener = player.GetComponent<Tweener>();
            var targetGridPos =  + "_0";
            GameObject gridPos;
            spawnedGrid.TryGetValue( targetGridPos, out gridPos );
            tweener.SetTarget( gridPos );
            */
            var rb = player.GetComponent<RaceBird>();
            rb.ForceMoveTo( 0, MapPlayersToStartPosition( playerID ), timeForBirdsRelocate );

            playerID++;
        }

        // move the camera
        var ct = Camera.main.gameObject.GetComponent<Tweener>();
        ct.SetTarget( cameraRaceAnchor, timeForBirdsRelocate );

        // set round state
        raceState = RaceState.PreRace;
    }

    void SpawnDebugMarker() {
        // set the distance reference object so we can spawn beyond the curvature of the shader
        GameObject unit = Instantiate( debugMarker, Vector3.zero, debugMarker.transform.rotation ) as GameObject;
        var pos = new Vector3(
            ( worldDimensions.x + spawnDistance ) * ( worldUnitDims.x + worldMargins.x ) + worldOffset.x,
            0,
            0
        );
        unit.name = "DebugMarker";
        unit.transform.parent = world;
        unit.transform.localPosition = pos;
    }

    void UpdateWorldPosition() {
        var pos = world.position;
        pos.x += hazardMoveSpeed;
        world.position = pos;
    }

    void UpdateSpawn() {
        var howManyToSpawn = (Mathf.CeilToInt( world.position.x / worldChunkWidth ) + spawnProtectionDistance) - spawnWaveCount;
        for( var i = 0; i <= howManyToSpawn; i++ ) {
            PopulateWave();
        }
    }

    void UpdateEnoughCoke() {
        if( marblesActive < marblesMinActive && marblesRoundTotal > 0 && Time.time > ( timeLastSpawnedMarble + timeSpawnMarbleDelay ) ) {
            var coke = God.SpawnChild( cokeObject, cokeSpawnAnchor );
            coke.name = "Coke #" + marblesRoundTotal;
            timeLastSpawnedMarble = Time.time;
            marblesRoundTotal--;
            marblesActive++;
        }

        if( marblesActive <= 0 && marblesRoundTotal <= 0 ) {
            raceState = RaceState.PostHungry;
        }
    }

    void PopulateWave() {
        var chunk = God.SpawnAt( chunkAnchor, new Vector3( (spawnWaveCount + worldChunkOffset) * worldChunkWidth, 0.0f, 0.0f ) );
        chunk.transform.SetParent( motion.transform );
        chunk.name = "Wave " + spawnWaveCount;

        // don't put hazards on an initial stretch
        if( spawnWaveCount + worldChunkOffset <= 0 ) {
            Destroy( chunk.transform.Find( "HazardStripAnchor" ).gameObject );
        }

        spawnWaveCount += 1;
    }

    void CreateWorld() {
        for( int x = 0; x < worldDimensions.x; x++ ) {
            for( int z = 0; z < worldDimensions.y; z++ ) {
                GameObject unit = Instantiate( gridObject, Vector3.zero, gridObject.transform.rotation ) as GameObject;
                var coordinate = x + "_" + z;
                var pos = new Vector3(
                    x * (worldUnitDims.x + worldMargins.x) + worldOffset.x,
                    0,
                    z * (worldUnitDims.y + worldMargins.y) + worldOffset.y
                );
                unit.name = coordinate + " " + unit.name;
                unit.transform.parent = GameObject.Find( worldSpawnTarget ).transform;
                unit.transform.localPosition = pos;
                spawnedGrid.Add(
                    coordinate,
                    unit
                );
            }
        }
    }
}

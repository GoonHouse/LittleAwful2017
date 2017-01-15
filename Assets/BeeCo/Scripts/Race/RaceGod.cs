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

[System.Serializable]
public struct ChatMessage {
    public ChatMessage( string msg, string user ) {
        this.user = user;
        this.msg = msg;
    }

    public string user;
    public string msg;
}

[System.Serializable]
public struct BirdStats {
    public int cokeConsumed; // how much coke has been consumed // X
    public int cokeRegenerated; // duh // X
    public int cokeFromHeaven; // bonus coke from the sky // X
    public int thingsCollidedWith; // how many things you've collided with // X
    public int timesLanesSwitched; // lane switch academy? (poops?)  // X
    public int timesStoodStill; // X
    public int boostsUsed; // X 
    public float timeSpentRunning; // X
    public float distanceTraveled; // how far you've gone -- winner gets the distance to the moon added // X
    public int messagesReceived;

    public float garbageStrewn; // things hit * (1/7) * Random.value(2*3);
    public float damageCaused;
    public float caloriesBurned;
}

public class RaceGod : MonoBehaviour {
    public RaceState raceState = RaceState.NoRace;

    public GameObject debugMarker;
    public GameObject gridObject;
    public GameObject chunkAnchor;
    public GameObject cokeObject;
    public GameObject coolHat;

    public Dictionary<string, GameObject> grid = new Dictionary<string, GameObject>();
    public Dictionary<string, GameObject> spawnedGrid = new Dictionary<string, GameObject>();
    public int playersAlive;
    public float startTime;

    public float spawnDistance = 3.0f;
    public float spawnProtectionDistance = 7.0f;

    // how long the strips of road are
    public float worldChunkWidth = 100.0f;
    public int worldChunkOffset = -3;
    public int worldHazardRange = 3;
    public Vector2 worldUnitDims = new Vector2(12, 10);
    public Vector2 worldDimensions = new Vector2(7, 4);
    public Vector2 worldMargins = new Vector2(2, 1);
    public Vector2 worldOffset;
    public GameObject worldSpawnTarget;
    public int worldSpawnSafetyDistance = 2;

    public Transform world;
    public Transform motion;

    public float intensity;
    public int spawnWaveCount = 0;
    public float hazardMoveSpeed = 1.0f;
    public float hazardIntensityFactor = 5.0f;

    public List<GameObject> players = new List<GameObject>();
    public List<GameObject> twitchPlayers = new List<GameObject>();
    public List<GameObject> playerGUIs = new List<GameObject>();

    public List<ChatMessage> chatQueue = new List<ChatMessage>();

    public GameObject policeLight;
    public Text hudTime;
    public GameObject cameraRaceAnchor;
    public GameObject cokeSpawnAnchors;
    public List<SpawnArea> cokeSpawners = new List<SpawnArea>();

    public float timeForBirdsRelocate = 3.0f;

    public float timePreRaceStart;
    public float timePreRaceDuration = 3.0f;

    public float timePostRaceStart;
    public float timePostRaceDuration = 3.0f;

    // hungry game logic
    public float timeHungryStart;
    public float timeHungryDuration = 60.0f;

    public float timeRaceStart;
    public float timeRaceDuration = 180.0f;

    public int marblesTotal = 0;
    public int marblesMinActive = 13;
    public int marblesActive = 0;
    public int marblesRoundTotal = 60;

    public float timeLastSpawnedMarble;
    public float timeSpawnMarbleDelay = 0.5f;

    public List<string> ircCommands = new List<string>{ "eat", "left", "right", "go", "back", "stop" };

    public List<BirdStats> stats = new List<BirdStats>();


    public List<int> playerPositions = new List<int>();
    public int winningPlayer;

    private string debugUser = "DickDaddy43";
    private List<string> debugMessages = new List<string>{
        "hey",
        "my dick does not work and that upsets me greatly just wanted you to know okay thanks guys I appreciate it thanks for visiting goodbye",
        "what do you mean irc\ndoesn't have newlines",
        "<b>it's funny because it says b</b>",
        "**get out** of my _house_"
    };
    private string ourScene = "";

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
        ourScene = scene.name;
        if( scene.name == "Race" ) {
            // shortcut transforms
            world = GameObject.Find( "World" ).transform;
            motion = GameObject.Find( "Motion/Lanes" ).transform;
            worldSpawnTarget = GameObject.Find( "World/TileSpawns" );

            FindInstanceThings();

            // determine world size
            // TODO: the road prefab does not consider lane width & isn't variable
            worldOffset = new Vector2(
                worldUnitDims.x / 2.0f,
                ( worldUnitDims.y / 2.0f ) - ( ( worldDimensions.y / 2.0f ) * ( worldUnitDims.y + worldMargins.y ) )
            );
            CreateWorld();
            worldSpawnTarget.SetActive( false );

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
        cokeSpawnAnchors = world.transform.Find( "CokeSpawnAnchors" ).gameObject;
        cokeSpawners = new List<SpawnArea>( cokeSpawnAnchors.GetComponentsInChildren<SpawnArea>() );

        var guis = Camera.main.gameObject.GetComponentsInChildren<PlayerGUI>();
        foreach( PlayerGUI gui in guis ) {
            playerGUIs.Add( gui.gameObject );
        }

        var theSave = God.main.GetComponent<SaveData>().loadedSave;
        // @TODO: Announce Twitch Rules
        // twitch.SendMsg( "benis" );

        var flashy = policeLight.GetComponent<Animator>();
        flashy.speed = theSave.flashingLights;

        var pid = 0;
        playersAlive = 0;
        var raceBirds = world.transform.Find( "BirdAnchors" ).gameObject.GetComponentsInChildren<RaceBird>();
        foreach( RaceBird rb in raceBirds ) {
            rb.playerID = pid;
            rb.SetBrainFromInt( theSave.playerUse[pid] );
            switch( theSave.playerUse[pid] ) {
                case 0:
                    twitchPlayers.Add( rb.gameObject );
                    break;
                case 2:
                    rb.gameObject.GetComponent<RaceBrainHuman>().playerID = (pid + 1);
                    break;
            }
            if( theSave.lastBirdToWin == pid ) {
                rb.winnerSize = theSave.lastBirdWinsConsecutive;
            } else {
                rb.winnerSize = 0;
            }
            players.Add( rb.gameObject );
            pid++;
            playersAlive++;
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

            twitch.SendMsg( "Hey fellow viewers! To make a Twitch controlled player do something, type \"command playerNumber\"!" );
            
        }
    }

    void MessageHandler( string msg, string user ) {
        var parts = new List<string>( msg.Split( ' ' ) );
        var wasCommand = false;
        var theSave = God.main.GetComponent<SaveData>().loadedSave;

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

        if( !wasCommand && user.ToLower() != theSave.ircNick.ToLower() ) {
            chatQueue.Add( new ChatMessage( msg, user ) );
        }
    }

    // Use this for initialization
    void Start () {
        // TheLevelWasLoaded( SceneManager.GetActiveScene(), LoadSceneMode.Single );
    }

    // Update is called once per frame
    void Update () {
        if( ourScene != "Race" ) {
            return;
        }
        UpdateTwitchChat();

        if( Input.GetKeyDown( KeyCode.Q ) ) {
            var msg = debugMessages[Random.Range( 0, debugMessages.Count - 1 )];
            MessageHandler( msg, debugUser );
        }
        if( Input.GetKeyDown( KeyCode.Y ) && raceState < RaceState.PostHungry ) {
            raceState = RaceState.PostHungry;
        }

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

                // UpdateEnoughCoke();

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
                    hudTime.text = "TO RUN!";
                    hudTime.transform.parent.GetComponent<Tweener>().SetTarget(
                        GameObject.Find( "TimeLabelAnchor" ),
                        timeForBirdsRelocate
                    );

                    raceState = RaceState.Race;
                }
                break;
            case RaceState.Race:
                if( timeRaceStart <= 0.0f ) {
                    timeRaceStart = Time.time;
                }
                // update the world position
                // update the track
                UpdateWorldPosition();
                UpdateSpawn();
                UpdatePlayersAlive();
                UpdateRaceSpeed();
                break;
            case RaceState.PostRace:
                // winner emerges, tween them into the sunset
                if( timePostRaceStart <= 0.0f ) {
                    timePostRaceStart = Time.time;
                }

                // check if we are still hungry by time
                if( Time.time > ( timePostRaceStart + timePostRaceDuration ) ) {
                    if( twitchPlayers.Count > 0 ) {
                        var twitch = God.main.GetComponent<TwitchIRC>();
                        twitch.SendMsg( "Holy wow, somebody won! Horray!" );
                    }

                    raceState = RaceState.NoRace;
                    SceneManager.LoadScene( "Moon" );
                }
                break;
            default:
                Debug.Log( "groose is loose" );
                break;
        }
    }

    void UpdateRaceSpeed() {
        var time = Mathf.Min( timeRaceDuration, Time.time - timeRaceStart );
        intensity = time / timeRaceDuration;

        hazardMoveSpeed = 1.0f + intensity * hazardIntensityFactor;
    }

    void UpdatePlayersAlive() {
        var alivePlayers = new List<RaceBird>();
        foreach( GameObject player in players ) {
            var rb = player.GetComponent<RaceBird>();
            if( rb.alive ) {
                alivePlayers.Add( rb );
            }
        }
        if( alivePlayers.Count <= 1 ) {
            alivePlayers[0].WinGame();
            var theSave = God.main.GetComponent<SaveData>().loadedSave;

            if( theSave.lastBirdToWin == alivePlayers[0].playerID ) {
                theSave.lastBirdWinsConsecutive++;
            } else {
                theSave.lastBirdWinsConsecutive = 0;
            }
            theSave.lastBirdToWin = alivePlayers[0].playerID;
            
            raceState = RaceState.PostRace;
        }
        playersAlive = alivePlayers.Count;
    }

    void UpdateTwitchChat() {
        if( twitchPlayers.Count > 0 && chatQueue.Count > 0 ) {
            var tps = new List<RaceBird>();
            foreach( GameObject twitch in twitchPlayers ) {
                var rb = twitch.GetComponent<RaceBird>();
                if( !rb.twitchMessage && rb.alive ) {
                    tps.Add( rb );
                }
            }

            if( tps.Count > 0 ) {
                tps.Shuffle();

                var tm = chatQueue[0];
                chatQueue.RemoveAt( 0 );
                var tb = tps[0];
                tb.TwitchMessage( tm.msg, tm.user );
            }
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
        policeLight.GetComponent<RandomNoises>().PlayAnySound();
        var pt = policeLight.GetComponent<Tweener>();
        pt.SetTarget( GameObject.Find( "PoliceLightDestAnchor" ), timeForBirdsRelocate );

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
            var posY = MapPlayersToStartPosition( playerID );
            
            rb.gridPosition = new Vector2( worldSpawnSafetyDistance, posY );
            rb.SetPosition();

            rb.ForceMoveTo( worldSpawnSafetyDistance, posY, timeForBirdsRelocate );

            playerID++;
        }

        foreach( SpawnArea spa in cokeSpawners ) {
            spa.shouldSpawn = false;
        }

        // destroy all the coke
        var cokes = GameObject.FindObjectsOfType<Marble>();
        foreach( Marble coke in cokes ) {
            Destroy( coke.gameObject );
        }

        // move the GUIs
        foreach( GameObject gui in playerGUIs ) {
            var target = gui.GetComponent<PlayerGUI>().tween_location;
            var twr = gui.GetComponent<Tweener>();
            twr.SetTarget( target, timeForBirdsRelocate );
        }

        // move the camera
        var ct = Camera.main.gameObject.GetComponent<Tweener>();
        ct.SetTarget( cameraRaceAnchor, timeForBirdsRelocate );

        // tell the chat how to play
        if( twitchPlayers.Count > 0 ) {
            var twitch = God.main.GetComponent<TwitchIRC>();
            twitch.SendMsg( "Controls: \"go\" or \"boost\" to use coke to boost, \"stop\" to stand still, and \"left/right\" to switch lanes!" );
        }

        // visualize the grid
        worldSpawnTarget.SetActive( true );

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
            
            var coke = cokeSpawners[0].SpawnA( cokeObject );
            coke.name = "Coke #" + marblesRoundTotal;
            coke.transform.localRotation = Random.rotation;

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
        if( spawnWaveCount + worldChunkOffset <= worldHazardRange ) {
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
                unit.transform.parent = worldSpawnTarget.transform;
                unit.transform.localPosition = pos;
                spawnedGrid.Add(
                    coordinate,
                    unit
                );
            }
        }
    }
}

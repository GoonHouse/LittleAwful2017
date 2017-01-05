using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RaceGod : MonoBehaviour {
    public GameObject debugMarker;
    public GameObject gridObject;
    public GameObject chunkAnchor;

    public Dictionary<string, GameObject> grid = new Dictionary<string, GameObject>();
    public Dictionary<string, GameObject> spawnedGrid = new Dictionary<string, GameObject>();
    public int playersAlive;
    public float startTime;

    public float spawnDistance = 3.0f;
    public float spawnProtectionDistance = 7.0f;

    // how long the strips of road are
    public float worldChunkWidth = 100.0f; 

    public Vector2 worldUnitDims = new Vector2(12, 10);
    public Vector2 worldDimensions = new Vector2(7, 4);
    public Vector2 worldMargins = new Vector2(2, 1);
    public Vector2 worldOffset;
    public string  worldSpawnTarget = "World/TileSpawns";

    public Transform world;
    public Transform motion;

    public int spawnWaveCount = 0;
    public float hazardMoveSpeed = 2.0f;

    /*
     * intensity = determined by: ( Time.time - startTime )
     * 
     * spawn manager needs to ramp up "pressure" and let it off by spawning.
     *      how hazardous a configuration is, is based on intensity + rng boundaries
     * after a certain amount of time, we need to make the chance of spawning things 100%
     */

    // Use this for initialization
    void Start () {
        // shortcut transforms
        world = GameObject.Find( "World" ).transform;
        motion = GameObject.Find( "Motion/Lanes" ).transform;

        // determine world size
        // TODO: the road prefab does not consider lane width & isn't variable
        worldOffset = new Vector2(
            worldUnitDims.x / 2.0f,
            ( worldUnitDims.y / 2.0f ) - ( ( worldDimensions.y / 2.0f ) * ( worldUnitDims.y + worldMargins.y ) )
        );
        CreateWorld();

        //SpawnDebugMarker();

        // spawn road chunks all the way up until the reference object
        UpdateMotion();
    }

    // Update is called once per frame
    void Update () {
        UpdateWorldPosition();
        // check if we need to spawn after we move the world
        UpdateMotion();
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

    // check if we need to spawn more waves
    void UpdateMotion() {
        // @TODO: simplify. really, this should only operate on the edge that howManyToSpawn > 0 but I complicated my math.
        var distanceSpawned = worldChunkWidth * spawnWaveCount;
        var distanceShouldSpawn = worldChunkWidth * ( spawnWaveCount - spawnProtectionDistance );
        var howManyToSpawn = Mathf.CeilToInt( ( distanceSpawned - world.position.x ) / worldChunkWidth );
        if( world.position.x >= distanceShouldSpawn ) {
            for( var i = 0; i <= howManyToSpawn; i++ ) {
                PopulateWave();
            }
        }
    }

    void PopulateWave() {
        var chunk = God.SpawnAt( chunkAnchor, new Vector3( spawnWaveCount * worldChunkWidth, 0.0f, 0.0f ) );
        chunk.transform.SetParent( motion.transform );
        chunk.name = "Wave " + spawnWaveCount;

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

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RaceGod : MonoBehaviour {
    public GameObject gridObject;
    public GameObject debugMarker;

    public Dictionary<string, GameObject> grid = new Dictionary<string, GameObject>();
    public Dictionary<string, GameObject> spawnedGrid = new Dictionary<string, GameObject>();
    public int playersAlive;
    public float startTime;

    public float spawnDistance = 3.0f;

    public Vector2 worldUnitDims = new Vector2(12, 10);
    public Vector2 worldDimensions = new Vector2(7, 4);
    public Vector2 worldMargins = new Vector2(2, 1);
    public Vector2 worldOffset;

    /*
     * intensity = determined by: ( Time.time - startTime )
     * 
     * spawn manager needs to ramp up "pressure" and let it off by spawning.
     *      how hazardous a configuration is, is based on intensity + rng boundaries
     * after a certain amount of time, we need to make the chance of spawning things 100%
     */

    // Use this for initialization
    void Start () {
        worldOffset = new Vector2(
            worldUnitDims.x / 2.0f,
            ( worldUnitDims.y / 2.0f ) - ( ( worldDimensions.y / 2.0f ) * ( worldUnitDims.y + worldMargins.y ) )
        );
        CreateWorld();

        // test debug object spawn
        GameObject unit = Instantiate( debugMarker, Vector3.zero, debugMarker.transform.rotation ) as GameObject;
        var pos = new Vector3(
            ( worldDimensions.x + spawnDistance ) * ( worldUnitDims.x + worldMargins.x ) + worldOffset.x,
            0,
            0
        );
        unit.name = "DebugWaveSpawnPoint";
        unit.transform.parent = GameObject.Find( "World" ).transform;
        unit.transform.localPosition = pos;
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void CreateWorld() {
        for( int x = 0; x < worldDimensions.x; x++ ) {
            for( int z = 0; z < worldDimensions.y; z++ ) {
                GameObject unit = Instantiate( gridObject, Vector3.zero, gridObject.transform.rotation ) as GameObject;
                var coordinate = x.ToString() + "_" + z.ToString();
                var pos = new Vector3(
                    x * (worldUnitDims.x + worldMargins.x) + worldOffset.x,
                    0,
                    z * (worldUnitDims.y + worldMargins.y) + worldOffset.y
                );
                unit.name = coordinate + " " + unit.name;
                unit.transform.parent = GameObject.Find("World").transform;
                unit.transform.localPosition = pos;
                spawnedGrid.Add(
                    coordinate,
                    unit
                );
            }
        }
    }
}

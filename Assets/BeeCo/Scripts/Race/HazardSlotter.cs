using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HazardSlotter : MonoBehaviour {
    public GameObject hazardBaseObject;
    public GameObject hazardObject;

    private RaceGod rg;

    // Use this for initialization
    void Start () {
        rg = God.main.GetComponent<RaceGod>();

        // == keep track of available positions
        //var positions = new List<GameObject>();

        //var maxSpots = rg.worldDimensions.y - FindObjectsOfType<RaceBird>().Length - 1;
        var spawns = GetSpawnList();
        for( int i = 0; i < spawns.Count - 1; i++ ) {
            var pos = new Vector3(
                0,
                0,
                spawns[i] * ( rg.worldUnitDims.y + rg.worldMargins.y ) + rg.worldOffset.y
            );
            
            var unit = God.SpawnChild( hazardBaseObject, gameObject );
            unit.transform.localPosition = pos;
            //positions.Add( unit );

            // place a hazard at every available position
            God.SpawnChild( hazardObject, unit );
        }
    }

    List<int> GetSpawnList() {
        // permute the indices of a table
        // ( e.g. 1, 2, 3, 4, 5, 6-> 3, 6, 4, 2, 1, 5)
        // then fill the first  k  with 1
        //       and the last  n-k with 0
        var shuffleDex = new List<int>();
        for( int i = 0; i < rg.worldDimensions.y; i++ ) {
            shuffleDex.Add( i );
        }
        shuffleDex.Shuffle();

        // @TODO: inform this range from living players and intensity
        int numToDrop = Random.Range( 2, 4 );
        shuffleDex.RemoveRange( 0, numToDrop );
        return shuffleDex;
    }
}

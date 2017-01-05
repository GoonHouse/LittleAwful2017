using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HazardSlotter : MonoBehaviour {
    public GameObject plateHazard;
    public GameObject plateNeat;

    private RaceGod rg;

    // Use this for initialization
    void Start () {
        rg = God.main.GetComponent<RaceGod>();

        // == keep track of available positions
        //var positions = new List<GameObject>();

        //var maxSpots = rg.worldDimensions.y - FindObjectsOfType<RaceBird>().Length - 1;
        var spawns = GetSpawnList();

        // @TODO: inform this range from living players and intensity
        // the intensity of the round should map such that:
        //      all are dropped frequently in the early game
        // but the player count ensures that players-1 always get dropped
        int numToDrop = Random.Range( 2, 4 );

        for( int i = 0; i < spawns.Count; i++ ) {
            var pos = new Vector3(
                0,
                0,
                spawns[i] * ( rg.worldUnitDims.y + rg.worldMargins.y ) + rg.worldOffset.y
            );

            GameObject unit;

            // instead of dropping, anything around this threshold is good
            if( i >= numToDrop ) {
                unit = God.SpawnChild( plateHazard, gameObject );
            } else {
                unit = God.SpawnChild( plateNeat, gameObject );
            }
            
            unit.transform.localPosition = pos;
            unit.name = i + " " + unit.name;
            //positions.Add( unit );
        }

        // @TODO: if we do not spawn anything here, add pressure to RaceGod
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

        return shuffleDex;
    }
}

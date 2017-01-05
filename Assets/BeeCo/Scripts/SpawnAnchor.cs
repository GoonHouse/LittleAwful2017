using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnAnchor : MonoBehaviour {

    public List<GameObject> possibleSpawns;
    public float chanceOfSpawn = 1.00f;
    public bool doScale = false;

    public void SpawnRandom() {
        if( Random.value <= chanceOfSpawn ) {
            var prefab = possibleSpawns[Random.Range( 0, possibleSpawns.Count )];
            var go = Instantiate( prefab, transform.position, transform.rotation ) as GameObject;
            go.transform.SetParent( transform, true );
            if( doScale ) {
                var sc = go.transform.localScale;
                var msc = transform.localScale;
                sc.x *= msc.x;
                sc.y *= msc.y;
                sc.z *= msc.z;
                go.transform.localScale = sc;
            }
        }
    }

    // Use this for initialization
    void Start() {
        SpawnRandom();
    }

    // Update is called once per frame
    void Update() {

    }
}
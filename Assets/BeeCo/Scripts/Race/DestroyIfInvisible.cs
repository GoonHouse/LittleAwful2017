using UnityEngine;
using System.Collections;

public class DestroyIfInvisible : MonoBehaviour {
    // Update is called once per frame
    RaceGod rg;

    void Start() {
        rg = God.main.GetComponent<RaceGod>();
    }
	void Update () {
        if( rg.world.position.x >= transform.position.x + ( rg.worldChunkWidth + ( rg.worldChunkWidth * Mathf.Abs( rg.worldChunkOffset ) ) ) ) {
            Destroy( gameObject );
        }
	}
}

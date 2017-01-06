using UnityEngine;
using System.Collections;

public class ScreamFactory : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter( Collider other ) {
        Debug.Log( "[scream factory] " + gameObject.name + " just touched a " + other.gameObject.name );
    }
}

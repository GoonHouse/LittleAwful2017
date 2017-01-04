using UnityEngine;
using System.Collections;

public class HazardMover : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        var t = transform.position;
        t.x -= God.main.GetComponent<RaceGod>().hazardMoveSpeed;
        transform.position = t;
	}
}

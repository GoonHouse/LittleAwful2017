using UnityEngine;
using System.Collections;

public class Tweener : MonoBehaviour {

    public Vector3 startPosition;
    public Quaternion startRotation;
    public float startTime;

    public Vector3 targetPosition;
    public Quaternion targetRotation;
    public float targetTime;

    public bool done = true;

    void Reset() {
        
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void UpdateTween() {

        if( !done ) {
            if( Time.time <= this.targetTime ) {
                var travelRatio = Time.time / this.targetTime;
                transform.position = Vector3.Lerp( startPosition, targetPosition, travelRatio );
                transform.rotation = Quaternion.Lerp( startRotation, targetRotation, travelRatio );
            } else {
                transform.position = targetPosition;
                transform.rotation = targetRotation;
                done = true;
            }
        }
    }

    public void SetTarget( Transform target, float time ) {
        SetTarget( target.position, target.rotation, time );
    }

    

    public void SetTarget( Vector3 newTargetPos, Quaternion newTargetRot, float time = 1.0f ) {
        // Set beginning positions.
        this.startPosition = transform.position;
        this.startRotation = transform.rotation;
        this.startTime = Time.time;
        
        // no rotation, set properly
        if( newTargetRot == null ) {
            newTargetRot = transform.rotation;
        }
        this.targetPosition = newTargetPos;
        this.targetRotation = newTargetRot;
        this.targetTime = this.startTime + time;

        done = false;
    }
}

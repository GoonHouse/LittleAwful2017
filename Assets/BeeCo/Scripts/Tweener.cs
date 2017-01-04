using UnityEngine;
using System.Collections;

public class Tweener : MonoBehaviour {
    public GameObject debugTarget;

    public float defaultTime;

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
        if( Input.GetKeyDown( KeyCode.Space ) ) {
            SetTarget( debugTarget );
        }

        UpdateTween();
	}

    void UpdateTween() {
        if( !done ) {
            if( Time.time <= this.targetTime ) {
                var travelRatio = ( Time.time - this.startTime ) / ( this.targetTime - this.startTime);
                transform.position = Vector3.Lerp( startPosition, targetPosition, travelRatio );
                transform.rotation = Quaternion.Lerp( startRotation, targetRotation, travelRatio );
            } else {
                transform.position = targetPosition;
                transform.rotation = targetRotation;
                done = true;
            }
        }
    }

    public void SetTarget( GameObject target, float time = 1.0f ) {
        SetTarget( target.transform.position, target.transform.rotation, time );
    }

    public void SetTarget( Transform target, float time = 1.0f ) {
        SetTarget( target.position, target.rotation, time );
    }

    public void SetTarget( Vector3 newTargetPos, Quaternion newTargetRot, float time = 1.0f ) {
        // Set beginning positions.
        this.startPosition = transform.position;
        this.startRotation = transform.rotation;
        this.startTime = Time.time;
        
        // no rotation, set properly
        /*
        if( newTargetRot ) {
            newTargetRot = transform.rotation;
        }
        */
        this.targetPosition = newTargetPos;
        this.targetRotation = newTargetRot;
        this.targetTime = this.startTime + time;

        done = false;
    }
}

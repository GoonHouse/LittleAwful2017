using UnityEngine;
using System.Collections;

public class Tweener : MonoBehaviour {
    public GameObject debugTarget;

    public EasingFunction.Ease easeType = EasingFunction.Ease.EaseInOutBack;
    private EasingFunction.Function easeFunc;

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
        easeFunc = EasingFunction.GetEasingFunction( easeType );
    }
	
	// Update is called once per frame
	void Update () {
        if( Input.GetKeyDown( KeyCode.Space ) ) {
            SetTarget( debugTarget );
        }

        UpdateTween();
	}

    Vector3 ApplyEase( Vector3 startVec, Vector3 targetVec, float ratio ) {
        return new Vector3(
            easeFunc( startVec.x, targetVec.x, ratio ),
            easeFunc( startVec.y, targetVec.y, ratio ),
            easeFunc( startVec.z, targetVec.z, ratio )
        );
    }

    Quaternion ApplyEase( Quaternion startQuat, Quaternion targetQuat, float ratio ) {
        return new Quaternion(
            easeFunc( startQuat.x, targetQuat.x, ratio ),
            easeFunc( startQuat.y, targetQuat.y, ratio ),
            easeFunc( startQuat.z, targetQuat.z, ratio ),
            easeFunc( startQuat.w, targetQuat.w, ratio )
        );
    }

    void UpdateTween() {
        if( !done ) {
            if( Time.time <= this.targetTime ) {
                var travelRatio = ( Time.time - this.startTime ) / ( this.targetTime - this.startTime);
                //travelRatio = Mathf.SmoothStep( 0.0f, 1.0f, travelRatio );

                /*
                var pos = transform.position;
                
                pos.x = LerpOutExpo( startPosition.x, targetPosition.x, travelRatio );
                pos.y = LerpOutExpo( startPosition.y, targetPosition.y, travelRatio );
                pos.z = LerpOutExpo( startPosition.z, targetPosition.z, travelRatio );

                transform.position = pos;
                */
                transform.position = ApplyEase( startPosition, targetPosition, travelRatio );
                transform.rotation = ApplyEase( startRotation, targetRotation, travelRatio );
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

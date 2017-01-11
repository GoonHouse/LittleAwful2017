using UnityEngine;
using System.Collections;

public class Tweener : MonoBehaviour {
    public EasingFunction.Ease easeType = EasingFunction.Ease.EaseInOutBack;
    private EasingFunction.Function easeFunc;

    public float defaultTime;

    public Vector3 startPosition;
    public Quaternion startRotation;
    public float startTime;

    public Transform targetTransform;
    //public Vector3 targetPosition;
    //public Quaternion targetRotation;
    public float targetTime;

    public bool done = true;
    public bool local = false;
    public bool childOnComplete = false;

	// Use this for initialization
	void Start () {
        easeFunc = EasingFunction.GetEasingFunction( easeType );
    }
	
	// Update is called once per frame
	void Update () {
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

                if( local ) {
                    transform.localPosition = ApplyEase( startPosition, targetTransform.localPosition, travelRatio );
                    transform.localRotation = ApplyEase( startRotation, targetTransform.localRotation, travelRatio );
                } else {
                    transform.position = ApplyEase( startPosition, targetTransform.position, travelRatio );
                    transform.rotation = ApplyEase( startRotation, targetTransform.rotation, travelRatio );
                }
            } else {
                if( local ) {
                    transform.localPosition = targetTransform.localPosition;
                    transform.localRotation = targetTransform.localRotation;
                } else {
                    transform.position = targetTransform.position;
                    transform.rotation = targetTransform.rotation;
                }

                if( childOnComplete && targetTransform != null ) {
                    gameObject.transform.SetParent( targetTransform );
                    transform.localPosition = Vector3.zero;
                    transform.localRotation = Quaternion.identity;
                }
                
                done = true;
            }
        }
    }

    public void SetTarget( GameObject target, float time = 1.0f ) {
        SetTarget( target.transform, time );
    }

    public void SetTarget( Transform target, float time = 1.0f ) {
        targetTransform = target;
        if( local ) {
            this.startPosition = transform.localPosition;
            this.startRotation = transform.localRotation;
        } else {
            this.startPosition = transform.position;
            this.startRotation = transform.rotation;
        }
        
        this.startTime = Time.time;
        this.targetTime = this.startTime + time;

        done = false;
        local = false;
    }
}

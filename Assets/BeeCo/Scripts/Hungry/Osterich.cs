using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Osterich : MonoBehaviour {
    /* when we press a button we need to:
     * 1) our mouths open
     * 2) extend our necks at a logarithmic rate until we reach our peak length
     * 
     * when we release a button we need to:
     * 1) close our mouth, trapping all marbles under our mouth
     * 
     * 
     * some considerations:
     * 1) have a small grace phunnel that lerps marbles to the center of our mouth
     * 2) have a maximum capacity for concurrent marbles in mouth?
     * 3) do not deliver marbles to cache until neck fully retracts
     * 4) inverting our neck's velocity will cause us to lose our payload
     */
    public Transform extendedTarget;
    public Transform retractedTarget;

    public KeyCode watchKey;

    public Vector3 extendStart;
    public Vector3 extendEnd;
    public float extendSpeed = 1.0f;
    public float extendTime;

    public Transform anchorHead;
    public Transform anchorNeck;
    public Transform anchorBody;
    public Transform anchorBeak;

    public bool isMouthOpen = false;
    public bool isExtending = false;

    public List<Marble> stomach = new List<Marble>();

    public float score = 0;

    // Use this for initialization
    void Start () {
        LerpHeadTowards(retractedTarget.position);
    }
	
	// Update is called once per frame
	void Update () {
	    if( Input.GetKeyDown( watchKey ) ) {
            OpenMouth();
        } else if( Input.GetKeyUp( watchKey ) ) {
            CloseMouth();
        }


        UpdateHeadPosition();

        if( ! isMouthOpen ) {

            // if we are not extending then:
            /* 
             * we can extend and eat marbles
             */
        }
	}

    public void Eat(Marble thing) {
        stomach.Add(thing);
        // thing.eater = this;

        var renderers = thing.gameObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers) {
            renderer.enabled = false;
        }

        thing.transform.SetParent(anchorBeak, true);

        score += thing.value;
    }

    void UpdateHeadPosition() {
        float distCovered = (Time.time - extendTime) * extendSpeed;
        float fracJourney = distCovered / Vector3.Distance(extendStart, extendEnd);
        anchorHead.position = Vector3.Lerp(extendStart, extendEnd, fracJourney);
    }

    public void OpenMouth() {
        isMouthOpen = true;
        anchorHead.localRotation = Quaternion.Euler(0.0f, 0.0f, 45.0f);
        LerpHeadTowards(extendedTarget.position);
    }

    public void CloseMouth() {
        isMouthOpen = false;
        anchorHead.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        LerpHeadTowards(retractedTarget.position);
    }

    public void LerpHeadTowards(Vector3 target) {
        extendStart = anchorHead.position;
        extendEnd = target;
        extendTime = Time.time;
    }
}

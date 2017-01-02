using UnityEngine;
using System.Collections;

public class Marble : MonoBehaviour {
    public float value = 1.00f;

    public Osterich eater;

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    void OnTriggerEnter(Collider obj) {
        var ost = obj.gameObject.GetComponentInParent<Osterich>();
        if( ost && ost.isMouthOpen && eater == null ) {
            ost.Eat(this);
        }
    }
}

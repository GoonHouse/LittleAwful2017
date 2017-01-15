using UnityEngine;
using System.Collections;

public class SpawnArea : MonoBehaviour {
    public bool shouldSpawn;
    public bool randomRotation;
    public float rateOfSpawn;
    public GameObject thingToSpawn;
    public float scaleRange = 2.0f;
    public UnityEngine.Events.UnityEvent onSpawn;

    private float nextSpawn;

    // Use this for initialization
    void Start() {
        nextSpawn = 0.0f;
    }

    // Update is called once per frame
    void Update() {
        if( Time.time > nextSpawn && shouldSpawn ) {
            nextSpawn = Time.time + rateOfSpawn;
            
            SpawnA( thingToSpawn );
        }
    }

    public GameObject SpawnA( Object objToSpawn ) {
        Vector3 position = getRandomPosition();
        GameObject go = (GameObject) Instantiate( objToSpawn, position, transform.rotation );
        go.GetComponent<Collider>().isTrigger = true;
        go.GetComponent<Rigidbody>().angularDrag = 1.0f;
        go.GetComponent<Rigidbody>().drag = 1.0f;
        go.GetComponent<Rigidbody>().mass = 50.0f;
        if( randomRotation ) {
            go.transform.localRotation = Random.rotation;
        }
        if( onSpawn != null ) {
            onSpawn.Invoke();
        }
        return go;
    }

    public Vector3 getRandomPosition() {
        BoxCollider bc = GetComponent<BoxCollider>();
        Bounds bound = bc.bounds;
        Vector3 ex = bound.extents;

        Vector3 newPos = new Vector3(
            Random.Range( -ex.x / scaleRange, ex.x / scaleRange ),
            Random.Range( -ex.y / scaleRange, ex.y / scaleRange ),
            Random.Range( -ex.z / scaleRange, ex.z / scaleRange )
        );
        //Debug.Log (newPos + transform.position);
        return newPos + transform.position;
    }
}
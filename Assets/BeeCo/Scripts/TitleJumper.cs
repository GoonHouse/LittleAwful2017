using UnityEngine;
using System.Collections;

public class TitleJumper : MonoBehaviour {

    private float dt = 0;
    public float t = 1;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        this.dt += Time.deltaTime;
        if( dt > t ) {
            UnityEngine.SceneManagement.SceneManager.LoadScene( "Main" );
        }
    }
}

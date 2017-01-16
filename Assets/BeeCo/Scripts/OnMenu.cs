using UnityEngine;
using System.Collections;

public class OnMenu : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GameObject.Find("UI").GetComponent<PlayMusic>().Track("cutscene");
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}

using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class HandleRayClick : MonoBehaviour {

    private Material default_mat = null;
    public Material hover_mat = null;
    public Material click_mat = null;

    public string target_state = "";
    public bool quit = false;
    public GameObject toggle_panel = null;

    // Use this for initialization
    void Start () {
        default_mat = GetComponent<Renderer>().material;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnMouseDown() {
        if(!toggle_panel.activeSelf) {
            GetComponent<Renderer>().material = click_mat;
        }
    }

    public void OnMouseUp() {
        if (!toggle_panel.activeSelf) {
            if (target_state != "") {  
                SceneManager.LoadScene(target_state);
            }
            if(quit) {
                Application.Quit();
            }
            if(toggle_panel) {
                toggle_panel.SetActive(!toggle_panel.activeSelf);
            }
        }
    }

    public void OnMouseEnter() {
        GetComponent<Renderer>().material = hover_mat;
    }

    public void OnMouseExit() {
        GetComponent<Renderer>().material = default_mat;
    }

}

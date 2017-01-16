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
    public GameObject [] disable_panels;

    // Use this for initialization
    void Start () {
        default_mat = GetComponent<Renderer>().material;
	}
	
	// Update is called once per frame
	void Update () {
        if (!this.IsActive()) {
            GetComponent<Renderer>().material = default_mat;
        }
    }

    public bool IsActive() {
        foreach (GameObject panel in disable_panels) {
            if(panel.activeSelf) {
                return false;
            }
        }
        return true;
    }

    public void OnMouseDown() {
        if(this.IsActive()) {
            GetComponent<Renderer>().material = click_mat;
        }
    }

    public void OnMouseUp() {
        if(this.IsActive()) {
            if (target_state != "") {
                // HACKS HACKS HACKS HACKS 
                var ts = God.main.GetComponent<SaveData>();
                if( ts.loadedSave.showCutscene && target_state == "Race" ) {
                    ts.loadedSave.showCutscene = false;
                    ts.SaveFile();
                    SceneManager.LoadScene( "Cutscene" );
                } else {
                    SceneManager.LoadScene( target_state );
                }
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
        if (this.IsActive()) {
            GetComponent<Renderer>().material = hover_mat;
        }
    }

    public void OnMouseExit() {
        if (this.IsActive()) {
            GetComponent<Renderer>().material = default_mat;
        }
    }

}

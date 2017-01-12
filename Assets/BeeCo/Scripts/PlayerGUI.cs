using UnityEngine;
using System.Collections;

public class PlayerGUI : MonoBehaviour {

    public int player = 0;
    public int score = 0;
    public int score_max = 0;

    private float tween_speed = 0.25f;

    private float score_visual_max = 5.0f;

    public int current_score = 0;
    private float score_dt = 0.0f;
    private float score_t = 1.0f;

    public GameObject score_text = null;
    public Material [] material_for_players = null;
    public Material [] material_for_hats = null;
    public GameObject hatlololol = null;
    public GameObject [] material_change = null;
    public GameObject score_bounce = null;
    public GameObject [] score_bounce_simple = null;

    public GameObject tween_location = null;

    // Use this for initialization
    void Start () {
        foreach(GameObject target in material_change) {
            target.GetComponent<Renderer>().material = material_for_players [player];
        }
        hatlololol.GetComponent<Renderer>().material = material_for_hats [player];

    }
	
	// Update is called once per frame
	void Update () {
        score_text.GetComponent<TextMesh>().text = current_score.ToString()+"/"+ score_max.ToString();
        // Bwahahaha, I laugh at anyone trying to debug this! ~josefnpat
        var temp_s = (Mathf.Min(current_score + score_dt, score_visual_max - 1) + Mathf.Sin(score_dt * Mathf.PI)) / score_visual_max;
        score_bounce.transform.localScale = new Vector3(temp_s, temp_s*0.666f, temp_s); // MAGIC SATANIC NUMBERS
        foreach( GameObject target in score_bounce_simple) {
            var s = 1 + Mathf.Sin(score_dt * Mathf.PI)/4;
            target.transform.localScale = new Vector3(s, s, s);
        }
        if(score == current_score) {
            score_dt = 0.0f;
        } else {

            score_dt += Time.deltaTime/this.tween_speed;
            
            if (score_dt > score_t) {
                score_dt -= score_t;
                if (current_score < score) {
                    current_score += 1;
                } else {
                    current_score -= 1;
                }

            }
        }

    }
}

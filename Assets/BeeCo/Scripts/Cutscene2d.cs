using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Cutscene2d : MonoBehaviour {

    public enum direction { None, Up, Right, Down, Left};

    [Header("Global Cutscene Variables")]
    public bool debugInspectorRun = false;
    public GUIStyle subtitleStyle;
    public float subtitleStylePadding = 32;
    private bool run = false;
    public string onDoneScene = "";
    public AudioSource music = null;

    [System.Serializable]
    public struct slide {
        [TextArea(5, 10)]
        public string sub;
        public float sub_delay;
        public AudioClip voiceover;
        public int override_voicever_length;
        public Texture2D [] backgrounds;
        public direction pan;
        public float music_volume;
    }
    [Header("Data")]
    public List<slide> slides = new List<slide>();

    private int current_slide;
    private AudioSource current_voiceover_source;
    private float dt;
    private float music_init;

    // Use this for initialization
    void Start () {
        music = GameObject.Find("UI").GetComponent<AudioSource>();
        music_init = music.volume;

        foreach ( slide cslide in this.slides) {
            foreach( Texture2D background in cslide.backgrounds) {
                if( background.format != TextureFormat.RGB24) {
                    Debug.LogError("Texture2d `"+background.name+"` is not type `"+ TextureFormat.RGB24.ToString() + "` (Is currently `"+ background.format + "` - Change \"Texture Type\" to \"Sprite (2D and UI)\")");
                }
            }
        }
	}

    void OnGUI() {
        if(this.isRunning()) {
            var cs = this.getCurrentSlide();
            foreach ( Texture2D ct in cs.backgrounds) {
                var w = 0.0f; var h = 0.0f; var x = 0.0f; var y = 0.0f;
                var tween = this.dt / this.getCurrentSlideTime();
                if (cs.pan == direction.Up) {
                    var hs = (float) Screen.width / (float) ct.width;
                    w = ct.width * hs;
                    h = ct.height * hs;
                    y = (Screen.height - h) - (Screen.height - w) * tween;
                } else if (cs.pan == direction.Right) {
                    var ws = (float) Screen.height / (float) ct.height;
                    w = ct.width * ws;
                    h = ct.height * ws;
                    x = (Screen.width - w) * tween;
                } else if (cs.pan == direction.Down) {
                    var hs = (float) Screen.width / (float) ct.width;
                    w = ct.width * hs;
                    h = ct.height * hs;
                    y = (Screen.height - h) * tween;
                } else if (cs.pan == direction.Left) {
                    var ws = (float) Screen.height / (float) ct.height;
                    w = ct.width * ws;
                    h = ct.height * ws;
                    x = (Screen.width - w) - (Screen.width - w) * tween;
                } else { // if (cs.pan == direction.None) {
                    var ws = (float) Screen.height / (float) ct.height;
                    w = ct.width * ws;
                    h = ct.height * ws;
                }
                //Debug.Log("screen:" + Screen.width + "x" + Screen.height + " image:" + ct.width + "x" + ct.height + " rendered at:" + w + "x" + h + " scale:" + ws);
                GUI.DrawTexture( new Rect(x,y,w,h), ct);
            }
            if (this.dt > this.getCurrentSlide().sub_delay) {
                GUI.Label(
                    new Rect(
                        subtitleStylePadding, subtitleStylePadding,
                        Screen.width-subtitleStylePadding*2, Screen.height-subtitleStylePadding*2),
                    this.getCurrentSlide().sub.Replace("\\n", "\n"), this.subtitleStyle);
            }
        }
    }

    // Update is called once per frame
    void Update () {
	    if(debugInspectorRun) {
            Debug.Log("Running via inspector toggle.");
            this.Run();
            this.debugInspectorRun = false;
        }
        if(this.isRunning()) {
            this.dt += Time.deltaTime;
            if(this.dt > this.getCurrentSlideTime() ) {
                this.current_slide++;
                if (this.current_slide >= this.slides.Count) {
                    this.Stop();
                    music.volume = music_init;
                } else {
                    this.cleanupCurrentSlide();
                    this.initCurrentSlide();
                }
            }
        }
    }

    public bool isRunning() {
        return this.run;
    }

    private slide getCurrentSlide() {
        return this.slides[this.current_slide];
    }

    private float getCurrentSlideTime() {
        var cs = this.getCurrentSlide();
        if (cs.override_voicever_length == 0) {
            return cs.voiceover.length;
        } else {
            return cs.override_voicever_length;
        }
    }

    public void Run() {
        Debug.Log("Start of cutscene.");
        this.run = true;
        this.current_slide = 0;
        this.initCurrentSlide();
    }

    private void initCurrentSlide() {
        music.volume = this.getCurrentSlide().music_volume;
        //Debug.Log("Setting volume to: " + music.volume);
        this.dt = 0;
        if (this.getCurrentSlide().voiceover != null) {
            this.current_voiceover_source = gameObject.AddComponent<AudioSource>();
            this.current_voiceover_source.clip = this.getCurrentSlide().voiceover;
            this.current_voiceover_source.Play();
        }
    }

    private void cleanupCurrentSlide() {
        this.current_voiceover_source.Stop();
        Destroy(this.current_voiceover_source);
    }

    public void Stop() {
        this.run = false;
        this.cleanupCurrentSlide();
        this.current_slide = 0;
        Debug.Log("End of cutscene.");
        if(this.onDoneScene != "") {
            var sd = God.main.GetComponent<SaveData>();
            sd.loadedSave.showCutscene = false;
            sd.SaveFile();
            SceneManager.LoadScene(this.onDoneScene);
        }
    }

}

using UnityEngine;
using System.Collections;

[System.Serializable]
public class HasteResponse : System.Object {
    public string key;
}

public class God : MonoBehaviour {
    public static God main;
    private bool doLock = false;

    // == game object stuff ==
    void Awake() {
        CheckLock();

        if (main == null) {
            DontDestroyOnLoad(gameObject);
            main = this;
        } else if (main != this) {
            DestroyImmediate(gameObject);
        }
    }

    // Use this for initialization
    void Start () {
        Application.runInBackground = true;

    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.F11) || Input.GetKeyDown(KeyCode.Escape)) {
            doLock = !doLock;
            CheckLock();
        }
    }

    public static void SetClipboard(string text) {
        TextEditor te = new TextEditor();
        te.text = text;
        te.SelectAll();
        te.Copy();
    }

    // == necessary == 
    void CheckLock() {
        if (doLock) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        } else {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    // == helpful public methods because why not ==
    public static GameObject SpawnChild(GameObject go, GameObject self, bool extra = true) {
        var i = Instantiate(go, self.transform.position, Quaternion.identity) as GameObject;
        i.transform.SetParent(self.transform, extra);
        return i;
    }

    public static GameObject SpawnAt(GameObject go, Vector3 pos) {
        return Instantiate(go, pos, Quaternion.identity) as GameObject;
    }

    public static string FormatTime(float time) {
        System.TimeSpan timeSpan = System.TimeSpan.FromSeconds(time);
        return string.Format("{0:D2}:{1:D2}.{2:D3}", timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);
    }

    public static float Scale(float valueIn, float baseMin, float baseMax, float limitMin, float limitMax) {
        return ((limitMax - limitMin) * (valueIn - baseMin) / (baseMax - baseMin)) + limitMin;
    }

    public static float KineticEnergy(Rigidbody2D rb) {
        // mass in kg, velocity in meters per second, result is joules
        return 0.5f * rb.mass * Mathf.Pow(rb.velocity.magnitude, 2);
    }

    public static float Round(float value, int digits) {
        float mult = Mathf.Pow(10.0f, (float)digits);
        return Mathf.Round(value * mult) / mult;
    }
}

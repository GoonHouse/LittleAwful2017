using UnityEngine;
using System.Collections;

[System.Serializable]
public class HasteResponse : System.Object {
    public string key;
}

public class God : MonoBehaviour {
    public static God main;
    public static float distanceToTheMoonInFeet = 1.255f * Mathf.Pow(10, 9);
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
        if( Input.GetKeyDown( KeyCode.F8 ) ) {
            Debug.Log( "fuck you deleting your save lol" );
            var sd = GetComponent<SaveData>();
            sd.loadedSave = new SaveDatum();
            sd.SaveFile();
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
        //return string.Format("{0:D2}:{1:D2}.{2:D3}", timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);
        return string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
    }

    public static string FormatMoney( float amount, bool showPlus = false ) {
        var prefix = "";

        if( amount < 0 ) {
            prefix = "-";
        } else if( amount > 0 && showPlus ) {
            prefix = "+";
        }

        

        return prefix + "$" + System.String.Format( "{0:n}", Mathf.Abs( amount ) );
    }

    public static string FormatNumber( float amount, bool showPlus = false ) {
        var prefix = "";

        if( amount < 0 ) {
            prefix = "-";
        } else if( amount > 0 && showPlus ) {
            prefix = "+";
        }

        return prefix + System.String.Format( "{0:n}", Mathf.Abs( amount ) );
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

    // http://stackoverflow.com/questions/15522347/c-sharp-function-for-converting-string-to-int-with-default-value-parameter
    public static int StrToIntDef(string s, int @default) {
        int number;
        if (int.TryParse(s, out number))
            return number;
        return @default;
    }
}


public static class BeeCoExtensions {
    public static float Round( this float value, int digits = 2 ) {
        float mult = Mathf.Pow( 10.0f, (float) digits );
        return Mathf.Round( value * mult ) / mult;
    }

    public static void Shuffle<T>( this System.Collections.Generic.IList<T> list ) {
        int n = list.Count;
        while( n > 1 ) {
            n--;
            int k = Random.Range( 0, n + 1 );
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static Transform FindAllChildren( this Transform parent, string name ) {
        var allTransforms = parent.gameObject.GetComponentsInChildren<Transform>();
        foreach( Transform child in allTransforms ) {
            if( child.gameObject.name == name ) {
                return child;
            }
        }
        return null;
    }
}
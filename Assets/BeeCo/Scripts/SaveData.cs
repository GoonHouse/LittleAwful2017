using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[System.Serializable]
public class SaveDatum {

    // Do not add defaults here, use Unity Editor!
    // "These are sane values!" ~@EntranceJew

    // Meta Save Info
    public bool isAutoSave = false;
    public System.DateTime created = System.DateTime.UtcNow;
    public System.DateTime modified = System.DateTime.UtcNow;
    public string name = "etc";

    // misc
    public int version = 100;
    public string playerName;

    // Game Settings
    // Changes from the UI are not reflected in the God object, but via SetAudiolevels.SetMusicLevel()
    public float musicVolume = 1.0f;
    public float effectsVolume = 1.0f;

    // Player Settings
    // 0 .. twitch, 1 .. AI, 2 .. human
    // player 1 is defaulted to human in LoadData.cs
    public int [] playerUse = new int[4];

    // Twitch Settings
    public string ircNick = "CocaineOstrich";
    public string ircPass = "iL0v3Coca1ne!";
    public string ircChannel = "sagamedev";
    public string ircServer = "irc.twitch.tv";
    public int    ircPort = 6667;
}

public class SaveData : MonoBehaviour {
    public SaveDatum loadedSave;
    public string defaultSaveName = "slot_1";
    public string[] savesFound;

    // Use this for initialization
    void Start () {
	    
	}

    // Update is called once per frame
    void Update() {

    }

    public List<string> ScanForFiles(int maxDays) {
        List<string> possibleNames = new List<string>();
        possibleNames.Add(defaultSaveName);
        for (int i = 0; i < maxDays + 1; i++) {
            possibleNames.Add("autosave_day_" + i.ToString());
        }

        List<string> foundNames = new List<string>();
        foreach (string possibleName in possibleNames) {
            if (Application.isWebPlayer) {
                if (PlayerPrefs.HasKey(possibleName + ".save")) {
                    foundNames.Add(possibleName);
                }
            } else {
                if (File.Exists(Application.persistentDataPath + "/" + possibleName + ".save")) {
                    foundNames.Add(possibleName);
                }
            }
        }

        return foundNames;
    }

    public void SaveFile() {
        SaveFile(defaultSaveName);
    }

    public void SaveFile(string filename) {
        BinaryFormatter bf = new BinaryFormatter();

        if (Application.isWebPlayer) {
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, this.loadedSave);
            string filedata = System.Convert.ToBase64String(ms.ToArray());
            PlayerPrefs.SetString(filename + ".save", filedata);
            PlayerPrefs.Save();
        } else {
            FileStream file = File.Create(Application.persistentDataPath + "/" + filename + ".save");
            bf.Serialize(file, this.loadedSave);
            file.Close();
        }
    }

    public void DeleteFile(string filename) {
        if (Application.isWebPlayer) {
            if (PlayerPrefs.HasKey(filename + ".save")) {
                PlayerPrefs.DeleteKey(filename + ".save");
                PlayerPrefs.Save();
            }
        } else {
            if (File.Exists(Application.persistentDataPath + "/" + filename + ".save")) {
                File.Delete(Application.persistentDataPath + "/" + filename + ".save");
            }
        }
    }

    public bool LoadFile() {
        return LoadFile(defaultSaveName);
    }

    public bool LoadFile(string filename) {
        BinaryFormatter bf = new BinaryFormatter();

        if (Application.isWebPlayer) {
            if (PlayerPrefs.HasKey(filename + ".save")) {
                string data = PlayerPrefs.GetString(filename + ".save");
                MemoryStream ms = new MemoryStream(System.Convert.FromBase64String(data));
                this.loadedSave = (SaveDatum)bf.Deserialize(ms);
            } else {
                Debug.LogError("Could not load web file: " + filename);
                return false;
            }
        } else {
            if (File.Exists(Application.persistentDataPath + "/" + filename + ".save")) {
                FileStream file = File.Open(Application.persistentDataPath + "/" + filename + ".save", FileMode.Open);
                this.loadedSave = (SaveDatum)bf.Deserialize(file);
                file.Close();
            } else {
                Debug.LogError("Could not load file: " + filename);
                return false;
            }
        }
        return true;
    }
}

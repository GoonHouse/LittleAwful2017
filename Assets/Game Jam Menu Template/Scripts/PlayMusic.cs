using UnityEngine;
using System.Collections;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class PlayMusic : MonoBehaviour {

    [System.Serializable]
    public struct MusicClip {
        public string name;
        public AudioClip music;
    }

    public MusicClip [] MusicClips;

    private AudioSource current_from;
    private AudioSource current_to;

    private bool crossfade;
    private float crossfade_t = 1;
    private float crossfade_dt = 0;
    private float crossfade_volume_from = 0;
    private float crossfade_volume_to = 0;

    public void Start() {
    }

    public void Track(string track) {
        Debug.Log("Starting track " + track);
        foreach(MusicClip mc in MusicClips) {
            if(mc.name == track) {
                if(current_to == null) {
                    current_to = gameObject.AddComponent<AudioSource>();
                    current_to.loop = true;
                }
                if(current_from == null) {
                    current_from = gameObject.AddComponent<AudioSource>();
                    current_from.loop = true;
                }
                Debug.Log("Found track, crossfading to "+track);
                current_from.clip = current_to.clip;
                crossfade_volume_from = God.main.GetComponent<SaveData>().loadedSave.musicVolume;
                current_to.clip = mc.music;
                crossfade_volume_to = God.main.GetComponent<SaveData>().loadedSave.musicVolume;

                current_to.Play();

                crossfade = true;
                crossfade_dt = 0;
                return;
            }
        }
        Debug.Log("No such track `"+track+"`");
    }

    public void forceVolume(float val) {
        current_to.volume = val;
    }

    public void Update() {
        if (crossfade) {
            crossfade_dt = Mathf.Min(1, crossfade_dt + Time.deltaTime);
            current_from.volume = crossfade_volume_from * (1 - crossfade_dt) / crossfade_t;
            current_to.volume = crossfade_volume_to * crossfade_dt / crossfade_t;

            if(current_from.volume <= 0) {
                current_from.Stop();
                crossfade = false;
            }
        }
    }

}

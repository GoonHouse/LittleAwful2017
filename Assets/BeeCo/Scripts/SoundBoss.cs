using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public struct SoundList {
    public string name;
    public List<AudioClip> clips;
}

public class SoundBoss : MonoBehaviour {
    public UnityEngine.Audio.AudioMixerGroup targetMixer;
    public List<SoundList> channelsToMake = new List<SoundList>();
    public Dictionary<string, RandomNoises> noiseLookup = new Dictionary<string, RandomNoises>();

	// Use this for initialization
	void Start () {
        foreach( SoundList sl in channelsToMake ) {
            var aus = gameObject.AddComponent<AudioSource>();
            aus.playOnAwake = false;
            aus.outputAudioMixerGroup = targetMixer;

            var rn = gameObject.AddComponent<RandomNoises>();
            rn.soundsToPlay = sl.clips;
            rn.audioSource = aus;

            noiseLookup[sl.name] = rn;
        }
	}

    public void Play( string routeName ) {
        RandomNoises rn;
        noiseLookup.TryGetValue( routeName, out rn );
        if( rn ) {
            rn.PlayAnySound();
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}

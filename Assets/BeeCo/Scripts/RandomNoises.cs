using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RandomNoises : MonoBehaviour {
    public AudioSource audioSource;

    public bool playOnTime = false;

    public float minTimeToWait;
    public float maxTimeToWait;

    public List<AudioClip> soundsToPlay;
    public int lastPlayedIndex;
    public float nextPlayTime;

    public void PlayAnySound() {
        PlaySound( PickASound() );
    }

    void PlaySound( int index ) {
        audioSource.Stop();
        audioSource.clip = soundsToPlay[index];
        audioSource.Play();
    }

    // Use this for initialization
    void Start() {
        SetNextTime();
    }

    void SetNextTime() {
        nextPlayTime = Time.time + Random.Range( minTimeToWait, maxTimeToWait );
    }

    int PickASound() {
        int newSound = lastPlayedIndex;
        while( newSound == lastPlayedIndex ) {
            newSound = Random.Range( 0, soundsToPlay.Count );
        }
        return newSound;
    }

    // Update is called once per frame
    void Update() {
        if( Time.time >= nextPlayTime && playOnTime ) {
            int soundIndex = PickASound();
            PlaySound( soundIndex );
            lastPlayedIndex = soundIndex;
            SetNextTime();
        }
    }
}
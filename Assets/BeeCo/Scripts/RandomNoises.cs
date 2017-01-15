using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RandomNoises : MonoBehaviour {
    public AudioSource audioSource;

    public bool playOnTime = false;
    public bool forceUnique = false;

    public float minTimeToWait;
    public float maxTimeToWait;

    public List<AudioClip> soundsToPlay;
    public int lastPlayedIndex = -1;
    public float nextPlayTime;

    public void PlayAnySound() {
        var s = PickASound();
        if( s >= 0 ) {
            PlaySound( s );
        } else {
            Debug.LogWarning( "tried to play an empty noise" );
        }
        
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
        int newSound = -1;
        if( forceUnique ) {
            newSound = lastPlayedIndex;
            while( newSound == lastPlayedIndex ) {
                newSound = Random.Range( 0, soundsToPlay.Count - 1 );
            }
        } else if( soundsToPlay.Count >= 1 ){
            newSound = Random.Range( 0, soundsToPlay.Count - 1 );
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
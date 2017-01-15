using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RaceBrainTwitch : RaceBrain {
    public List<bool> doLeft = new List<bool>();
    public List<bool> doRight = new List<bool>();
    public List<bool> doGo = new List<bool>();
    public List<bool> doStop = new List<bool>();

    public float chanceToLeft = 0.10f;
    public float chanceToRight = 0.10f;
    public float chanceToGo = 0.07f;
    public float chanceToStop = 0.00f;

    public override bool BrainLeft() {
        if( doLeft.Count > 0 ) {
            doLeft.RemoveAt( 0 );
            return true;
        } else {
            return Random.value < chanceToLeft;
        }
    }

    public override bool BrainRight() {
        if( doRight.Count > 0 ) {
            doRight.RemoveAt( 0 );
            return true;
        } else {
            return Random.value < chanceToRight;
        }
    }

    public override bool BrainGo() {
        if( doGo.Count > 0 ) {
            doGo.RemoveAt( 0 );
            return true;
        } else {
            return Random.value < chanceToGo;
        }
    }

    public override bool BrainStop() {
        if( doStop.Count > 0 ) {
            doStop.RemoveAt( 0 );
            return true;
        } else {
            return Random.value < chanceToStop;
        }
    }

    public override void DoSignal( string signal ) {
        Debug.Log( gameObject.name + " got a signal saying: " + signal );
        switch( signal ) {
            case "eat":
            case "go":
                doGo.Add( true );
                break;
            case "left":
                doLeft.Add( true );
                break;
            case "right":
                doRight.Add( true );
                break;
            case "back":
            case "stop":
                doStop.Add( true );
                break;
        }
    }
}

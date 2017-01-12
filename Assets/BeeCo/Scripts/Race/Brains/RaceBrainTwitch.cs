using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RaceBrainTwitch : RaceBrain {
    public List<bool> doLeft = new List<bool>();
    public List<bool> doRight = new List<bool>();
    public List<bool> doGo = new List<bool>();
    public List<bool> doStop = new List<bool>();

    public override bool BrainLeft() {
        if( doLeft.Count > 0 ) {
            doLeft.RemoveAt( 0 );
            return true;
        }
        return false;
    }

    public override bool BrainRight() {
        if( doRight.Count > 0 ) {
            doRight.RemoveAt( 0 );
            return true;
        }
        return false;
    }

    public override bool BrainGo() {
        if( doGo.Count > 0 ) {
            doGo.RemoveAt( 0 );
            return true;
        }
        return false;
    }

    public override bool BrainStop() {
        if( doStop.Count > 0 ) {
            doStop.RemoveAt( 0 );
            return true;
        }
        return false;
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

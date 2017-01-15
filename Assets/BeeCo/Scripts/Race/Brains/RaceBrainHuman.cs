using UnityEngine;
using System.Collections;

public class RaceBrainHuman : RaceBrain {

    public int playerID;

    public override bool BrainLeft() {
        return Input.GetButtonDown("Player" + playerID + "Left");
    }

    public override bool BrainRight() {
        return Input.GetButtonDown( "Player" + playerID + "Right" );
    }

    public override bool BrainGo() {
        return Input.GetButtonDown( "Player" + playerID + "Go" );
    }

    public override bool BrainStop() {
        return Input.GetButtonDown( "Player" + playerID + "Stop" );
    }

    public override void DoSignal( string signal ) {
        ///throw new System.NotImplementedException();
    }
}

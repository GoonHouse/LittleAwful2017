using UnityEngine;
using System.Collections;

public class RaceBrainHuman : RaceBrain {

    public int playerID = 1;

    public override bool BrainLeft() {
        return Input.GetButtonDown("Player" + playerID + "Left");
    }

    public override bool BrainRight() {
        return Input.GetButtonDown( "Player" + playerID + "Right" );
    }

    public override bool BrainGo() {
        return Input.GetButtonDown( "Player" + playerID + "Action" );
    }

    public override bool BrainStop() {
        return Input.GetButtonDown( "Player" + playerID + "Back" );
    }

    public override void DoSignal( string signal ) {
        ///throw new System.NotImplementedException();
    }
}

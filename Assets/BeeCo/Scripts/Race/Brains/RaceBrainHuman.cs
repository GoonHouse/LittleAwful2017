using UnityEngine;
using System.Collections;

public class RaceBrainHuman : RaceBrain {

    public override bool BrainLeft() {
        return Input.GetKeyDown(KeyCode.W);
    }

    public override bool BrainRight() {
        return Input.GetKeyDown(KeyCode.S);
    }

    public override bool BrainGo() {
        return Input.GetKeyDown(KeyCode.D);
    }

    public override bool BrainStop() {
        return Input.GetKeyDown(KeyCode.A);
    }
}

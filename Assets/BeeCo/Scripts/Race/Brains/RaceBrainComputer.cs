using UnityEngine;
using System.Collections;

public class RaceBrainComputer : RaceBrain {
    public float chanceToLeft = 0.10f;
    public float chanceToRight = 0.10f;
    public float chanceToGo = 0.07f;
    public float chanceToStop = 0.07f;

    public override bool BrainLeft() {
        return Random.value <= chanceToLeft;
    }

    public override bool BrainRight() {
        return Random.value <= chanceToRight;
    }

    public override bool BrainGo() {
        return Random.value <= chanceToGo;
    }

    public override bool BrainStop() {
        return Random.value <= chanceToStop;
    }
}

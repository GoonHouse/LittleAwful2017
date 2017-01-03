using UnityEngine;
using System.Collections;

public abstract class RaceBrain : MonoBehaviour {

    public abstract bool BrainLeft();
    public abstract bool BrainRight();
    public abstract bool BrainGo();
    public abstract bool BrainStop();

}

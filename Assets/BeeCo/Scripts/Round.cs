using UnityEngine;
using System.Collections;

[CreateAssetMenu]
public class Round : ScriptableObject {
    public string roundType = "";
    public float timeLimit = 60.0f;
    public int marbles = 30;
}
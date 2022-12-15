using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossInput : MonoBehaviour {

    public KeyCode Forward1 = KeyCode.W, Forward2 = KeyCode.UpArrow;
    public KeyCode Backward1 = KeyCode.S, Backward2 = KeyCode.DownArrow;
    public KeyCode Left1 = KeyCode.A, Left2 = KeyCode.LeftArrow;
    public KeyCode Right1 = KeyCode.D, Right2 = KeyCode.RightArrow;
    public KeyCode PauseMenu = KeyCode.Escape;

    public float Vertical;
    public float Horizontal;
    
    void Update()
    {
        Vertical = Input.GetKey(Forward1) || Input.GetKey(Forward2) || Input.GetKey(Backward1) || Input.GetKey(Backward2) ? 1 : 0;
        Horizontal = Input.GetKey(Left1) || Input.GetKey(Left2) || Input.GetKey(Right1) || Input.GetKey(Right2) ? 1 : 0;
    }
}

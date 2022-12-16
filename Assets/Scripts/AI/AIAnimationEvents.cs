using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAnimationEvents : MonoBehaviour
{
    public Animator animator;

    public void EndSlightJump() {
        animator.SetBool("SlightJump", false);
    }
}

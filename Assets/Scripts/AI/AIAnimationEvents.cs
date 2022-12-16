using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAnimationEvents : MonoBehaviour
{
    public Animator animator;
    public PhysicsAIController physicsAIController;

    public void EndSlightJump() {
        animator.SetBool("SlightJump", false);
        physicsAIController.isPlayingParkourAnimation = false;
    }

    public void EndClimbHigh() {
        animator.SetBool("ClimbHigh", false);
        physicsAIController.isPlayingParkourAnimation = false;
    }
}

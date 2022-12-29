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

    public void EndClimbMedPlus() {
        animator.SetBool("ClimbMed+", false);
        physicsAIController.isPlayingParkourAnimation = false;
    }

    public void EndClimbMed() {
        animator.SetBool("ClimbMed", false);
        physicsAIController.isPlayingParkourAnimation = false;
    }

    public void EndSlide() {
        physicsAIController.isPlayingParkourAnimation = false;
        physicsAIController.ReturnCCToDefault();
    }

    public void EndJumpGap() {
        animator.SetBool("JumpGap", false);
        physicsAIController.isPlayingParkourAnimation = false;
    }

    public void EndFalling() {
        physicsAIController.isPlayingParkourAnimation = false;
    }
}
